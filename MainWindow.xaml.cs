using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Krasnokam
{
    public partial class MainWindow : Window
    {
        private ScaleTransform st = new ScaleTransform();
        UIElementCollection arrr;
        public MainWindow()
        {
            
            InitializeComponent();
            viewB.RenderTransform = st;
        }

        private void InkCanvas_ActiveEditingModeChanged(object sender, RoutedEventArgs e)
        {

        }
        private void grid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //MyCanvasComponent.Children.Clear(); //removes previously drawed objects


            double x = e.GetPosition(can).X; //get mouse coordinates over canvas
            double y = e.GetPosition(can).Y;

            Ellipse elipsa = new Ellipse(); //create ellipse
            elipsa.StrokeThickness = 3;
            elipsa.Stroke = Brushes.Red;
            elipsa.Margin = new Thickness(x-10, y-10, 0, 0);
            elipsa.Width = 20;
            elipsa.Height = 20;

            TextBlock textBlock = new TextBlock();
            textBlock.Text = "1";
            textBlock.Margin = new Thickness(x - 10, y - 10, 0, 0);
            textBlock.Width = 15;
            textBlock.Height = 15;

            //add (draw) ellipse to canvas  
            can.Children.Add(elipsa);
            can.Children.Add(textBlock);
            arrr = can.Children;
        }

        private void can_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var vvvv = arrr;
        }

        private void viewB_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0) st.ScaleX = st.ScaleX *= 1.1;
            if (e.Delta < 0) st.ScaleX = st.ScaleX /= 1.1;
            st.ScaleY = st.ScaleX;
        }
        /*
private void MyCanvasComponent_MouseDown(object sender, MouseButtonEventArgs e)
{
MyCanvasComponent.Children.Clear(); //removes previously drawed objects


double x = e.GetPosition(MyCanvasComponent).X; //get mouse coordinates over canvas
double y = e.GetPosition(MyCanvasComponent).Y;

Ellipse elipsa = new Ellipse(); //create ellipse
elipsa.StrokeThickness = 3;
elipsa.Stroke = Brushes.Red;
elipsa.Margin = new Thickness(x, y, 0, 0);
elipsa.Width = 20;
elipsa.Height = 20;

//add (draw) ellipse to canvas  
MyCanvasComponent.Children.Add(elipsa);
}

private void MyCanvasComponent_PreviewMouseDown(object sender, MouseButtonEventArgs e)
{
//MyCanvasComponent.Children.Clear(); //removes previously drawed objects


double x = e.GetPosition(MyCanvasComponent).X; //get mouse coordinates over canvas
double y = e.GetPosition(MyCanvasComponent).Y;

Ellipse elipsa = new Ellipse(); //create ellipse
elipsa.StrokeThickness = 3;
elipsa.Stroke = Brushes.Red;
elipsa.Margin = new Thickness(x, y, 0, 0);
elipsa.Width = 20;
elipsa.Height = 20;

//add (draw) ellipse to canvas  
MyCanvasComponent.Children.Add(elipsa);
}
*/
    }
}
