using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Krasnokam
{
    public partial class MainWindow : Window
    {
        UIElementCollection arrr;
        public MainWindow()
        {
            
            InitializeComponent();
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

            //add (draw) ellipse to canvas  
            can.Children.Add(elipsa);
            arrr = can.Children;
        }

        private void can_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var vvvv = arrr;
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
