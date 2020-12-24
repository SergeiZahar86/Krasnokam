using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Krasnokam
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        double FirstXPos, FirstYPos, FirstArrowXPos, FirstArrowYPos;
        object MovingObject;
        Line Path1, Path2, Path3, Path4;
        Rectangle FirstPosition, CurrentPosition;
        List<Double> Dots;  // Настройка линий, по которым мы хотим показать путь движения
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



        private Global global;

        int stckNmbr;                        // номер штабеля
        Border border;
        Ellipse elipsa;
        TextBlock textBlock;
        Point? lastCenterPositionOnTarget;
        Point? lastMousePositionOnTarget;
        Point? lastDragPoint;
        UIElementCollection arrr;
        List<Ellipse> www;
        public MainPage()
        {


            // //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            stckNmbr = 1;
            global = Global.getInstance();
            InitializeComponent();
            elipsa = new Ellipse();
            textBlock = new TextBlock();
            border = new Border();

            scrollViewer.ScrollChanged += OnScrollViewerScrollChanged;  // Происходит при обнаружении изменений в положении прокрутки, экстенте или размере окна просмотра.
            scrollViewer.MouseLeftButtonUp += OnMouseLeftButtonUp;
            scrollViewer.PreviewMouseLeftButtonUp += OnMouseLeftButtonUp;
            scrollViewer.PreviewMouseWheel += OnPreviewMouseWheel;
            scrollViewer.PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
            scrollViewer.MouseMove += OnMouseMove;
            slider.ValueChanged += OnSliderValueChanged;
            cnv.PreviewMouseRightButtonDown += OnCanvasPreviewMouseRightButtonDown;
            elipsa.PreviewMouseRightButtonDown += OnElipseMouseRightButtonDown;

            cnv.PreviewMouseMove += this.MouseMove;
        }
        void OnMouseMove(object sender, MouseEventArgs e) // перемещение курсора над картой. Сдвиг картинки.
        {
            if (lastDragPoint.HasValue)                        // проверяем наличие значения 
            {
                Point posNow = e.GetPosition(scrollViewer);    // заносим координаты курсора в объект Point
                double dX = posNow.X - lastDragPoint.Value.X;
                double dY = posNow.Y - lastDragPoint.Value.Y;
                lastDragPoint = posNow;
                scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - dX);  // Прокручивает содержимое в ScrollViewer до указанной позиции горизонтального смещения.
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - dY);      // Прокручивает содержимое в ScrollViewer до указанной позиции вертикального смещения.
            }
        }
        void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)  // ЛКМ по карте для записи координат клика и изменения вида курсора
        {
            var mousePos = e.GetPosition(scrollViewer);
            if (mousePos.X <= scrollViewer.ViewportWidth && mousePos.Y < scrollViewer.ViewportHeight) // проверка того что клик был в границах объекта
            {
                scrollViewer.Cursor = Cursors.SizeAll;    // меняем курсор
                lastDragPoint = mousePos;                 // записать координаты клика
                Mouse.Capture(scrollViewer);              // метод Capture привязывает мышь к объекту
            }
        }
        void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e) // перемещение ползунка зума
        {
            lastMousePositionOnTarget = Mouse.GetPosition(cnv); // позиция мыши во время зума
            if (e.Delta > 0)
            {
                slider.Value += 0.2;
            }
            if (e.Delta < 0)
            {
                slider.Value -= 0.2;
            }
            e.Handled = true;
        }
        void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e) // отжатие ЛКМ
        {
            scrollViewer.Cursor = Cursors.Arrow;   // меняем вид курсора
            scrollViewer.ReleaseMouseCapture();    // Освобождает мышь, если элемент произвел ее захват.
            lastDragPoint = null;
        }
        void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) // событие слайдера. 
        {
            scaleTransform.ScaleX = e.NewValue;  // Возвращает новое значение свойства, указанное событием
            scaleTransform.ScaleY = e.NewValue;  // Возвращает новое значение свойства, указанное событием
            var centerOfViewport = new Point(scrollViewer.ViewportWidth / 2, scrollViewer.ViewportHeight / 2);  // получаем середину
            lastCenterPositionOnTarget = scrollViewer.TranslatePoint(centerOfViewport, cnv); // сохраняем значение координат середины
        }
        void OnScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e)  // Происходит при обнаружении изменений в положении прокрутки, экстенте или размере окна просмотра.
        {
            if (e.ExtentHeightChange != 0 || e.ExtentWidthChange != 0)  // Возвращает значение, которое задает изменение ширины и высоты экстента ScrollViewer.
            {
                Point? targetBefore = null;
                Point? targetNow = null;

                if (!lastMousePositionOnTarget.HasValue) // если приближение производится ползунком
                {
                    if (lastCenterPositionOnTarget.HasValue)
                    {
                        var centerOfViewport = new Point(scrollViewer.ViewportWidth / 2, scrollViewer.ViewportHeight / 2);  // центр объекта
                        Point centerOfTargetNow = scrollViewer.TranslatePoint(centerOfViewport, cnv); // Преобразует точку относительно данного элемента в координаты относительно указанного элемента.
                        targetBefore = lastCenterPositionOnTarget;
                        targetNow = centerOfTargetNow;
                    }
                }
                else
                {
                    targetBefore = lastMousePositionOnTarget;
                    targetNow = Mouse.GetPosition(cnv);
                    lastMousePositionOnTarget = null;
                }

                if (targetBefore.HasValue)
                {
                    double dXInTargetPixels = targetNow.Value.X - targetBefore.Value.X;
                    double dYInTargetPixels = targetNow.Value.Y - targetBefore.Value.Y;
                    double multiplicatorX = e.ExtentWidth / cnv.Width;                        // ExtentWidth Получает обновленную ширину экстента ScrollViewer.
                    double multiplicatorY = e.ExtentHeight / cnv.Height;                      // ExtentHeight Получает обновленную высоту экстента ScrollViewer.
                    double newOffsetX = scrollViewer.HorizontalOffset - dXInTargetPixels * multiplicatorX; // HorizontalOffset Получает значение, содержащее горизонтальное смещение прокручиваемого содержимого.
                    double newOffsetY = scrollViewer.VerticalOffset - dYInTargetPixels * multiplicatorY;   // VerticalOffset Получает значение, которое содержит вертикальное смещение прокручиваемого содержимого.
                    if (double.IsNaN(newOffsetX) || double.IsNaN(newOffsetY))
                    {
                        return;
                    }
                    scrollViewer.ScrollToHorizontalOffset(newOffsetX); // Выполняет прокрутку содержимого в ScrollViewer до указанной позиции горизонтального смещения.
                    scrollViewer.ScrollToVerticalOffset(newOffsetY);   // Выполняет прокрутку содержимого в ScrollViewer до указанной позиции вертикального смещения.
                }
            }
        }
        /*
        private void can_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var vvvv = arrr;
        }
        */
        private void OnCanvasPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)                  // добавление элемента на canvas
        {
            if (global.stackNumber.Count > 1)
            {
                for (int f = 1; f < global.stackNumber.Count; f++)
                {
                    if((global.stackNumber[f] - global.stackNumber[f-1]) > 1)
                    {
                        stckNmbr = f + 1;
                        global.stackNumber.Insert(f, stckNmbr);
                        break;
                    }
                    if((global.stackNumber.Count - f) == 1)
                    {
                        stckNmbr = f + 2;
                        global.stackNumber.Add(stckNmbr);
                        break;
                    }
                }
            }
            else if(global.stackNumber.Count == 1)
            {
                stckNmbr = 2;
                global.stackNumber.Add(stckNmbr);
            }
            else
            {
                stckNmbr = 1;
                global.stackNumber.Add(stckNmbr);
            }
            double x = e.GetPosition(cnv).X; //get mouse coordinates over canvas
            double y = e.GetPosition(cnv).Y;

            border = new Border();
            border.Width = 25;
            border.Height = 25;
            border.Margin = new Thickness(x - 10, y - 10, 0, 0);
            border.CornerRadius = new CornerRadius(15);
            border.BorderBrush = Brushes.Red;
            border.BorderThickness = new Thickness(2);
            border.Focusable = true;


            textBlock = new TextBlock();
            textBlock.FontSize = 10;
            textBlock.Inlines.Add(new Bold(new Run(stckNmbr.ToString())));
            textBlock.Foreground = Brushes.Black;
            textBlock.TextAlignment = TextAlignment.Center;
            textBlock.VerticalAlignment = VerticalAlignment.Center;
            textBlock.HorizontalAlignment = HorizontalAlignment.Center;
            textBlock.Padding = new Thickness(0, 0, 0, 1);


            border.Child = textBlock;
            cnv.Children.Add(border);
            // ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            border.PreviewMouseLeftButtonDown += this.MouseLeftButtonDown;
            border.PreviewMouseLeftButtonUp += this.PreviewMouseLeftButtonUp;
            border.Cursor = Cursors.Hand;
               
            
            // Настройка линий, по которым мы хотим показать путь движения
            Dots = new List<double>();

            Dots.Add(1);
            Dots.Add(2);
            Path1 = new Line();
            Path1.Width = cnv.Width;
            Path1.Height = cnv.Height;
            Path1.Stroke = Brushes.DarkGray;
            Path1.StrokeDashArray = new DoubleCollection(Dots);

            Path2 = new Line();
            Path2.Width = cnv.Width;
            Path2.Height = cnv.Height;
            Path2.Stroke = Brushes.DarkGray;
            Path2.StrokeDashArray = new DoubleCollection(Dots);

            Path3 = new Line();
            Path3.Width = cnv.Width;
            Path3.Height = cnv.Height;
            Path3.Stroke = Brushes.DarkGray;
            Path3.StrokeDashArray = new DoubleCollection(Dots);

            Path4 = new Line();
            Path4.Width = cnv.Width;
            Path4.Height = cnv.Height;
            Path4.Stroke = Brushes.DarkGray;
            Path4.StrokeDashArray = new DoubleCollection(Dots);

            FirstPosition = new Rectangle();
            FirstPosition.Stroke = Brushes.DarkGray;
            FirstPosition.StrokeDashArray = new DoubleCollection(Dots);

            CurrentPosition = new Rectangle();
            CurrentPosition.Stroke = Brushes.DarkGray;
            CurrentPosition.StrokeDashArray = new DoubleCollection(Dots);

            // Добавление линий на главную панель проектирования (холст)
            cnv.Children.Add(Path1);
            cnv.Children.Add(Path2);
            cnv.Children.Add(Path3);
            cnv.Children.Add(Path4);
            cnv.Children.Add(FirstPosition);
            cnv.Children.Add(CurrentPosition);








            /*elipsa = new Ellipse(); //create ellipse
            elipsa.StrokeThickness = 2;
            elipsa.Stroke = Brushes.Red;
            elipsa.Margin = new Thickness(x - 10, y - 10, 0, 0);
            elipsa.Width = 20;
            elipsa.Height = 20;
            elipsa.Focusable = true;

            //SolidColorBrush mySolidColorBrush = new SolidColorBrush();
            //mySolidColorBrush.Color = Color.FromArgb(255, 255, 255, 0);
            //elipsa.Fill = mySolidColorBrush;




            textBlock = new TextBlock();
            textBlock.FontSize = 7;
            textBlock.Inlines.Add(new Bold(new Run("245")));
            textBlock.TextAlignment = TextAlignment.Center;
            textBlock.Margin = new Thickness(x - 15, y - 6, 0, 0);
            textBlock.Width = 30;
            textBlock.Height = 15;

            //add (draw) ellipse to canvas  
            cnv.Children.Add(elipsa);
            cnv.Children.Add(textBlock);
            arrr = cnv.Children;*/



        }
        // //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // В этом случае мы получаем текущую позицию мыши на элементе управления, чтобы использовать его в событии MouseMove.
            FirstXPos = e.GetPosition(sender as Control).X;
            FirstYPos = e.GetPosition(sender as Control).Y;
            var v = (sender as FrameworkElement).Parent;
            /*FirstArrowXPos = e.GetPosition((sender as Control).Parent as Control).X - FirstXPos;
            FirstArrowYPos = e.GetPosition((sender as Control).Parent as Control).Y - FirstYPos;*/
            FirstArrowXPos = e.GetPosition((sender as FrameworkElement).Parent as FrameworkElement).X - FirstXPos;
            FirstArrowYPos = e.GetPosition((sender as FrameworkElement).Parent as FrameworkElement).Y - FirstYPos;
            MovingObject = sender;
        }
        void PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // В этом случае мы должны установить видимость линий на Скрытый
            MovingObject = null;
            Path1.Visibility = System.Windows.Visibility.Hidden;
            Path2.Visibility = System.Windows.Visibility.Hidden;
            Path3.Visibility = System.Windows.Visibility.Hidden;
            Path4.Visibility = System.Windows.Visibility.Hidden;
            FirstPosition.Visibility = System.Windows.Visibility.Hidden;
            CurrentPosition.Visibility = System.Windows.Visibility.Hidden;
        }
        private void MouseMove(object sender, MouseEventArgs e)
        {
            /*
             * В этом случае сначала проверяем состояние левой кнопки мыши. Если он нажат и
             * объект отправителя события похож на наш движущийся объект, мы можем перемещать наш элемент управления с помощью
             * некоторые эффекты.
             */
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // Перемещение объектов начинаем с задания позиций линий.
                Path1.X1 = FirstArrowXPos;
                Path1.Y1 = FirstArrowYPos;
                Path1.X2 = e.GetPosition((MovingObject as FrameworkElement).Parent as FrameworkElement).X - FirstXPos;
                Path1.Y2 = e.GetPosition((MovingObject as FrameworkElement).Parent as FrameworkElement).Y - FirstYPos;

                Path2.X1 = Path1.X1 + (MovingObject as FrameworkElement).ActualWidth;
                Path2.Y1 = Path1.Y1;
                Path2.X2 = Path1.X2 + (MovingObject as FrameworkElement).ActualWidth;
                Path2.Y2 = Path1.Y2;

                Path3.X1 = Path1.X1;
                Path3.Y1 = Path1.Y1 + (MovingObject as FrameworkElement).ActualHeight;
                Path3.X2 = Path1.X2;
                Path3.Y2 = Path1.Y2 + (MovingObject as FrameworkElement).ActualHeight;

                Path4.X1 = Path1.X1 + (MovingObject as FrameworkElement).ActualWidth;
                Path4.Y1 = Path1.Y1 + (MovingObject as FrameworkElement).ActualHeight;
                Path4.X2 = Path1.X2 + (MovingObject as FrameworkElement).ActualWidth;
                Path4.Y2 = Path1.Y2 + (MovingObject as FrameworkElement).ActualHeight;

                FirstPosition.Width = (MovingObject as FrameworkElement).ActualWidth;
                FirstPosition.Height = (MovingObject as FrameworkElement).ActualHeight;
                FirstPosition.SetValue(Canvas.LeftProperty, FirstArrowXPos);
                FirstPosition.SetValue(Canvas.TopProperty, FirstArrowYPos);

                CurrentPosition.Width = (MovingObject as FrameworkElement).ActualWidth;
                CurrentPosition.Height = (MovingObject as FrameworkElement).ActualHeight;
                CurrentPosition.SetValue(Canvas.LeftProperty, Path1.X2);
                CurrentPosition.SetValue(Canvas.TopProperty, Path1.Y2);

                Path1.Visibility = System.Windows.Visibility.Visible;
                Path2.Visibility = System.Windows.Visibility.Visible;
                Path3.Visibility = System.Windows.Visibility.Visible;
                Path4.Visibility = System.Windows.Visibility.Visible;
                FirstPosition.Visibility = System.Windows.Visibility.Visible;
                CurrentPosition.Visibility = System.Windows.Visibility.Visible;

                /*
                 * Для изменения положения элемента управления мы должны использовать метод SetValue для установки
                 * зависимости Canvas.LeftProperty и Canvas.TopProperty.
                 *
                 * Для расчета текущего положения элемента управления необходимо:
                 * Текущая позиция курсора мыши на родительском объекте -
                 * Положение мыши на элементе управления в начале движения -
                 * позиция родителя элемента управления.
                 */
                (MovingObject as FrameworkElement).SetValue(Canvas.LeftProperty,
                    e.GetPosition((MovingObject as FrameworkElement).Parent as FrameworkElement).X - FirstXPos);

                (MovingObject as FrameworkElement).SetValue(Canvas.TopProperty,
                    e.GetPosition((MovingObject as FrameworkElement).Parent as FrameworkElement).Y - FirstYPos);
            }
        }
        // ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////






        private void OnElipseMouseRightButtonDown(object sender, MouseButtonEventArgs e)                         // ПКМ по эллипсу
        {
            Point posNow = e.GetPosition(cnv);
        }
        private void UIElement_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            /*
                var name = (e.OriginalSource as FrameworkElement).Name;
                if (name.Length > 0)
                {
                    MessageBox.Show(name);
            */
        }

        private void DataGridMain_Loaded(object sender, RoutedEventArgs e)                                       // загрузка данных в DataGrid
        {
            //DataGridMain.ItemsSource = null;
            //DataGridMain.ItemsSource = global.ROWS;
        }

        private void DataGridMain_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)   // изменение значений строки
        {
            /*
            Change_of_Data_on_the_Wagon change_Of_Data = new Change_of_Data_on_the_Wagon();
            global.Idx = DataGridMain.SelectedIndex;

            change_Of_Data.Number.Text = global.ROWS[global.Idx].Car_id.ToString();            // порядковый номер вагона в шапке окна
            change_Of_Data.oldVagNum.Content = global.ROWS[global.Idx].Num;                    // старый номер вагона
            change_Of_Data.oldTara.Content = global.ROWS[global.Idx].Tara;                     // вес тары
            change_Of_Data.oldTara_e.Content = global.ROWS[global.Idx].Tara_e;                 // прежний вес тары НСИ
            change_Of_Data.oldTara_delta.Content = global.ROWS[global.Idx].Tara_delta;         // дельта тары
            change_Of_Data.oldCarrying.Content = global.ROWS[global.Idx].Carrying;             // прежняя грузоподъемность
            change_Of_Data.old_zona.Content = global.ROWS[global.Idx].Zone_eString;            // прежнее значение "зона"
            change_Of_Data.old_shipper.Content = global.ROWS[global.Idx].Shipper_String;       // прежнее значение Грузоотправитель
            change_Of_Data.old_consigner.Content = global.ROWS[global.Idx].Consigner_String;   // прежнее значение Грузополучателя
            change_Of_Data.old_mat.Content = global.ROWS[global.Idx].Mat_String;               // прежнее значение материала
            switch (global.ROWS[global.Idx].Att_codeString )
            {
                case "CheckCircle":
                    change_Of_Data.old_isOk.Content = "Аттестован"; break;
                case "WindowClose":
                    change_Of_Data.old_isOk.Content = "Не аттестован"; break;
                case "Asterisk":
                    change_Of_Data.old_isOk.Content = "Условно аттестован"; break;
            }
            change_Of_Data.old_cause.Content = global.ROWS[global.Idx].Cause_idString;         // прежнее значение причины неаттестации

            change_Of_Data.ShowDialog();
            DataGridMain.ItemsSource = null;
            DataGridMain.ItemsSource = global.ROWS;
            */
        }
        private void matButton_Click(object sender, RoutedEventArgs e)                                           // изменение материала 
        {
            /*
            ShowChange_Mat_String Change_Mat_String = new ShowChange_Mat_String();
            global.Idx = DataGridMain.SelectedIndex;
            Change_Mat_String.ShowDialog();
            DataGridMain.ItemsSource = null;
            DataGridMain.ItemsSource = global.ROWS;
            */
        }









    }
}
