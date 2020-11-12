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
        private bool canvas1_dragged = false;
        private Point PointMousePressed = new Point();
        private Thickness position;





        //private ScaleTransform st = new ScaleTransform();
        //private ScaleTransform cn = new ScaleTransform();
        UIElementCollection arrr;
        public MainWindow()
        {
            
            InitializeComponent();

            canvas1.LayoutTransform = st;

            //viewB.RenderTransform = st;
            //can.RenderTransform = cn;
        }



        private void canvas1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            canvas1_dragged = canvas1.CaptureMouse();
            PointMousePressed = e.GetPosition(canvas1);
        }

        private void canvas1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            canvas1_dragged = false;
            canvas1.ReleaseMouseCapture();
        }

        private void canvas1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!canvas1_dragged) return;
            Point PointMouseMoved = e.GetPosition(canvas1);
            position = canvas1.Margin;
            position.Left += PointMouseMoved.X - PointMousePressed.X;
            position.Top += PointMouseMoved.Y - PointMousePressed.Y;
            canvas1.Margin = position;
        }

        private void canvas1_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
            Point PointMouseWheel = e.GetPosition(canvas1);
            position = canvas1.Margin;
            //формулу точную не выводил, но центр массштабирования теперь находится приблизительно там, где указатель мыши
            if (e.Delta > 0) { st.ScaleX *= 1.1; position.Left -= PointMouseWheel.X * 0.1 * st.ScaleX; position.Top -= PointMouseWheel.Y * 0.1 * st.ScaleY; }
            if (e.Delta < 0) { st.ScaleX /= 1.1; position.Left += PointMouseWheel.X * 0.1 * st.ScaleX; position.Top += PointMouseWheel.Y * 0.1 * st.ScaleY; }
            canvas1.Margin = position;
            st.ScaleY = st.ScaleX;
        }








        private void can_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var vvvv = arrr;
        }

        private void viewB_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            /*
            if (e.Delta > 0) cn.ScaleX = cn.ScaleX *= 1.1;
            if (e.Delta < 0) cn.ScaleX = cn.ScaleX /= 1.1;
            cn.ScaleY = cn.ScaleX;
            */
        }

        private void ss2_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            double x = e.GetPosition(canvas1).X; //get mouse coordinates over canvas
            double y = e.GetPosition(canvas1).Y;

            Ellipse elipsa = new Ellipse(); //create ellipse
            elipsa.StrokeThickness = 3;
            elipsa.Stroke = Brushes.Red;
            elipsa.Margin = new Thickness(x - 10, y - 10, 0, 0);
            elipsa.Width = 20;
            elipsa.Height = 20;

            TextBlock textBlock = new TextBlock();
            textBlock.Text = "1";
            textBlock.Margin = new Thickness(x - 10, y - 10, 0, 0);
            textBlock.Width = 15;
            textBlock.Height = 15;

            //add (draw) ellipse to canvas  
            canvas1.Children.Add(elipsa);
            canvas1.Children.Add(textBlock);
            arrr = canvas1.Children;
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
