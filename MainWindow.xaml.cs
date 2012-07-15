using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Windows.Media.Animation;

namespace FunctionalWpf
{

    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int Count = 100;
        LambdaCollection<Ellipse> circles;
        public MainWindow()
        {
            InitializeComponent();

            circles = new LambdaCollection<Ellipse>(Count)
            .SetProperty(WidthProperty, i => i *.3 + 1)
            .SetProperty(HeightProperty, i => i * .3 + 1)
            .SetProperty(Shape.FillProperty, i => new SolidColorBrush(Color.FromArgb(255, (byte)((i+150) % (i + 10)), (byte)(255 % (i * 1.1 + 9)), (byte)(255 % (i + 1)))))

            .WithXY(x => 500 + Math.Pow(x, 1.2) * Math.Sin(x / 4.0 * Math.PI),
                    y => 500 + Math.Pow(y, 1.2) * Math.Cos(y / 4.0 * Math.PI));
            

            foreach (var item in circles)
            {
                MainCanvas.Children.Add(item);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            var animationCollection = new LambdaBuobleAniamtionCollection(Count,
                x => 500 + Math.Pow(x, 1.2) * Math.Sin(x / 4.0 * Math.PI),
                x => x * 10,
                i => new Duration(TimeSpan.FromMilliseconds(i*10%2000)),
                i => j => 100 / j);
            animationCollection.BeginAnimation(circles.Cast<UIElement>().ToArray(), Canvas.LeftProperty);
        }
    }
    public class LambdaCollection<T> : Collection<T> where T : DependencyObject, new()
    {
        public LambdaCollection(int count)
        {
            //save number of objects
            int shapeCount = count;
            //create shapeCount objects, add to collection
            while (shapeCount-- > 0)
                Add(new T());
        }

        public LambdaCollection<T> SetProperty<U>(DependencyProperty property, Func<int, U> generator)
        {
            for (int i = 0; i < Count; i++)
            {
                this[i].SetValue(property, generator(i));
            }
            //fluent initialization
            return this;
        }

        public LambdaCollection<T> WithXY<U>(Func<int, U> xGenerator, Func<int, U> yGenerator)
        {
            for (int i = 0; i < Count; i++)
            {
                this[i].SetValue(Canvas.LeftProperty, xGenerator(i));
                this[i].SetValue(Canvas.TopProperty, yGenerator(i));
            }
            return this;
        }

    }
    public class LambdaDoubleAnimation : DoubleAnimation
    {
        public Func<double, double> ValueGenerator { get; set; }
        protected override double GetCurrentValueCore(double defaultOriginValue, double defaultDestinationValue, AnimationClock animationClock)
        {
            return ValueGenerator(base.GetCurrentValueCore(defaultOriginValue, defaultDestinationValue, animationClock));
        }
    }
    public class LambdaBuobleAniamtionCollection : Collection<LambdaDoubleAnimation>
    {
        public LambdaBuobleAniamtionCollection(int count,
            Func<int, double> from,
            Func<int, double> to,
            Func<int, Duration> duration,
            Func<int, Func<double, double>> valueGenerator)
        {
            for (int i = 0; i < count; i++)
            {
                var lda = new LambdaDoubleAnimation()
                {
                    From = from(i),
                    To = to(i),
                    Duration = duration(i),
                    ValueGenerator = valueGenerator(i)
                };
                Add(lda);
            }

        }

        public void BeginAnimation(UIElement[] elements, DependencyProperty property)
        {
            for (int i = 0; i < Count; i++)
            {
                elements[i].BeginAnimation(property, Items[i]);
            }
        }
    }
}
