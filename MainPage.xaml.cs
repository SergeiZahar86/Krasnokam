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

        Ellipse elipsa;
        TextBlock textBlock;

        Point? lastCenterPositionOnTarget;
        Point? lastMousePositionOnTarget;
        Point? lastDragPoint;
        UIElementCollection arrr;
        List<Ellipse> www;
        public MainPage()
        {

            InitializeComponent();
            elipsa = new Ellipse();
            textBlock = new TextBlock();

            scrollViewer.ScrollChanged += OnScrollViewerScrollChanged;  // Происходит при обнаружении изменений в положении прокрутки, экстенте или размере окна просмотра.
            scrollViewer.MouseLeftButtonUp += OnMouseLeftButtonUp;
            scrollViewer.PreviewMouseLeftButtonUp += OnMouseLeftButtonUp;
            scrollViewer.PreviewMouseWheel += OnPreviewMouseWheel;
            scrollViewer.PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
            scrollViewer.MouseMove += OnMouseMove;
            slider.ValueChanged += OnSliderValueChanged;
            cnv.PreviewMouseRightButtonDown += OnCanvasPreviewMouseRightButtonDown;
            elipsa.PreviewMouseRightButtonDown += OnElipseMouseRightButtonDown;

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
        private void OnCanvasPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)                  // ПКМ по эллипсу
        {
            double x = e.GetPosition(cnv).X; //get mouse coordinates over canvas
            double y = e.GetPosition(cnv).Y;

            elipsa = new Ellipse(); //create ellipse
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
            arrr = cnv.Children;
           
        }               
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
