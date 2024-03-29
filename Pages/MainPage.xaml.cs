﻿using Krasnokam.DialogWindows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Krasnokam
{
    public partial class MainPage : Page
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        double FirstXPos, FirstYPos;
        Border MovingObject;           // внешний бордер метки
        bool boolBorder;               // флаг наведения на бордер
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private Global global;
        int stckNmbr;                        // номер штабеля
        Border border;
        TextBlock textBlock;
        Point? lastCenterPositionOnTarget;
        Point? lastMousePositionOnTarget;
        Point? lastDragPoint;                // представляет пару координат X и Y в двухмерном пространстве
        public MainPage()
        {
            // //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            stckNmbr = 1;
            global = Global.getInstance();
            InitializeComponent();
            textBlock = new TextBlock();
            border = new Border();
            // //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            scrollViewer.ScrollChanged += OnScrollViewerScrollChanged;  // Происходит при обнаружении изменений в положении прокрутки, экстенте или размере окна просмотра.
            scrollViewer.MouseLeftButtonUp += OnMouseLeftButtonUp;
            //scrollViewer.PreviewMouseLeftButtonUp += OnMouseLeftButtonUp;
            scrollViewer.PreviewMouseWheel += OnPreviewMouseWheel;
            scrollViewer.PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
            scrollViewer.MouseMove += OnMouseMove;
            slider.ValueChanged += OnSliderValueChanged;
            cnv.PreviewMouseRightButtonDown += OnCanvasPreviewMouseRightButtonDown;
            // //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            cnv.PreviewMouseMove += this.MouseMove;

            DataGridMain.ItemsSource = null;
            DataGridMain.ItemsSource = global.stackTabs;
        }
        void OnMouseMove(object sender, MouseEventArgs e) // перемещение курсора над картой. Сдвиг картинки.
        {
            if (!boolBorder)
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
        }
        void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)  // ЛКМ по карте для записи координат клика и изменения вида курсора
        {
            if (!boolBorder)
            {
                var mousePos = e.GetPosition(scrollViewer);
                if (mousePos.X <= scrollViewer.ViewportWidth && mousePos.Y < scrollViewer.ViewportHeight) // проверка того что клик был в границах объекта
                {
                    scrollViewer.Cursor = Cursors.SizeAll;    // меняем курсор
                    lastDragPoint = mousePos;                 // записать координаты клика
                    Mouse.Capture(scrollViewer);              // метод Capture привязывает мышь к объекту
                }
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
                MovingObject = null;
        }
        void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) // событие слайдера. 
        {
            
                scaleTransform.ScaleX = e.NewValue;  // Возвращает новое значение свойства, указанное событием
                scaleTransform.ScaleY = e.NewValue;  // Возвращает новое значение свойства, указанное событием
                var centerOfViewport = new Point(scrollViewer.ViewportWidth / 2, scrollViewer.ViewportHeight / 2);  // получаем середину
                lastCenterPositionOnTarget = scrollViewer.TranslatePoint(centerOfViewport, cnv); // сохраняем значение координат середины
            
        }
        void OnScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e)  //Происходит при обнаружении
            //изменений в положении прокрутки, экстенте или размере окна просмотра.
            
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
                        // Преобразует точку относительно данного элемента в координаты относительно указанного элемента.
                        Point centerOfTargetNow = scrollViewer.TranslatePoint(centerOfViewport, cnv); 
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
        private void OnCanvasPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)   // добавление элемента на canvas
        {
            //MovingObject = (Border)sender; // Получаем объект по клику
            //var ddr = sender; // Получаем объект по клику
            if (!boolBorder)
            {
                if (global.stackNumber.Count > 1)
                {
                    for (int f = 1; f < global.stackNumber.Count; f++)
                    {
                        if ((global.stackNumber[f] - global.stackNumber[f - 1]) > 1)
                        {
                            stckNmbr = f + 1;
                            global.stackNumber.Insert(f, stckNmbr);
                            break;
                        }
                        if ((global.stackNumber.Count - f) == 1)
                        {
                            stckNmbr = f + 2;
                            global.stackNumber.Add(stckNmbr);
                            break;
                        }
                    }
                }
                else if (global.stackNumber.Count == 1)
                {
                    stckNmbr = 2;
                    global.stackNumber.Add(stckNmbr);
                }
                else
                {
                    stckNmbr = 1;
                    global.stackNumber.Add(stckNmbr);
                }


                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                double x = e.GetPosition(cnv).X; //get mouse coordinates over canvas
                double y = e.GetPosition(cnv).Y;

                border = new Border();
                border.Width = 25;
                border.Height = 25;
                border.Margin = new Thickness(x - 12, y - 12, 0, 0);
                var ddd = border.Margin.Left;
                border.CornerRadius = new CornerRadius(15);
                border.BorderBrush = Brushes.Red;
                border.Background = Brushes.LightPink;
                border.BorderThickness = new Thickness(2);
                border.Focusable = true;
                border.Tag = stckNmbr.ToString(); // для поиска метки по клику правой кнопки мыши



                textBlock = new TextBlock();
                textBlock.FontSize = 10;
                textBlock.Inlines.Add(new Bold(new Run(stckNmbr.ToString())));
                textBlock.Foreground = Brushes.Black;
                textBlock.TextAlignment = TextAlignment.Center;
                textBlock.VerticalAlignment = VerticalAlignment.Center;
                textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                textBlock.Padding = new Thickness(0, 0, 0, 1);

                // добавляем объект метки в список
                StackTab stktb = new StackTab(stckNmbr, "hz", 0.0, 0.0, "hz", 0.0, 0.0, "hz", border.Margin.Left,
                    border.Margin.Top, border.Margin.Right, border.Margin.Bottom);
                global.stackTabs.Add(stktb);
                // обновляем таблицу
                DataGridMain.ItemsSource = null;
                DataGridMain.ItemsSource = global.stackTabs;

                // добавляем метку на канвас
                border.Child = textBlock;
                cnv.Children.Add(border);
                global.Bord.Clear();
                for(int i = 0; i < cnv.Children.Count; i++)
                {
                    global.Bord.Add(cnv.Children[i]);
                }
                // назначение обработчиков событий ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                border.PreviewMouseLeftButtonDown += MouseLeftButtonDown;
                border.PreviewMouseRightButtonDown += MouseRightButtonDown;
                border.PreviewMouseLeftButtonUp += PreviewMouseLeftButtonUp;
                border.PreviewMouseMove += MouseMoveBorder;
                border.MouseLeave += MouseLeave;
                border.Cursor = Cursors.Hand;
            }
            else
            {

            }
        }
        // //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //bool remove_border = true;
            MovingObject = (Border)sender; // Получаем объект по клику

            foreach(StackTab stack in global.stackTabs) // ищем в табличном списке
            {
                if(stack.Stack_id.ToString() == MovingObject.Tag.ToString())
                {
                    for ( int i = 1; i < cnv.Children.Count; i++) // ищем на canvas
                    {
                        Border border = (Border)cnv.Children[i];
                        if(border.Tag.ToString() == MovingObject.Tag.ToString())
                        {
                            global.Stack_id = stack.Stack_id;
                            global.Material = stack.Material;
                            global.Cinder = stack.Cinder;
                            global.Humidity = stack.Humidity;
                            global.Task = stack.Task;
                            global.Weight_now = stack.Weight_now;
                            global.Weight_actually = stack.Weight_actually;
                            global.Status = stack.Status;
                            global.Margin_left = border.Margin.Left;
                            global.Margin_top = border.Margin.Top;
                            global.Margin_right = border.Margin.Right;
                            global.Margin_bottom = border.Margin.Bottom;

                            ChangeDataStack dataStack = new ChangeDataStack();
                            dataStack.ShowDialog();

                            if (global.deleetBorder)
                            {
                                cnv.Children.Remove(border);
                                global.Bord.Remove(border);
                                global.stackTabs.Remove(stack);

                                DataGridMain.ItemsSource = null;
                                DataGridMain.ItemsSource = global.stackTabs;
                                //remove_border = false;
                                //break;
                                goto m1;
                            }
                        }
                    }
                }
            }
            m1:
            DataGridMain.ItemsSource = null;
            DataGridMain.ItemsSource = global.stackTabs;

            //MessageBox.Show("Была нажата правая клавиша мыши");
        }
        private void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // В этом случае мы получаем текущую позицию мыши на элементе управления, чтобы использовать его в событии MouseMove.
            FirstXPos = e.GetPosition(cnv).X;
            FirstYPos = e.GetPosition(cnv).Y;

            var v = (sender as FrameworkElement).Parent;
           
            MovingObject = (Border)sender; // Получаем объект по клику
        }
        private void MouseMoveBorder(object sender, MouseEventArgs e) // курсор находится над бордером
        {
            var v = sender;
            boolBorder = true;
        }
        private void MouseLeave(object sender, MouseEventArgs e) // курсор уходит за пределы бордера
        {
            var v = sender;
            boolBorder = false;
        }
        private void MouseMove(object sender, MouseEventArgs e) // перемещение бордера по канвасу
        {
            /*
             * В этом случае сначала проверяем состояние левой кнопки мыши. Если он нажат и
             * объект отправителя события похож на наш движущийся объект, мы можем перемещать наш элемент управления с помощью
             * некоторые эффекты.
             */
            if (e.LeftButton == MouseButtonState.Pressed)  // если нажата левая клавиша мыши
            {
                    /*
                     * Для изменения положения элемента управления мы должны использовать метод SetValue для установки
                     * зависимости Canvas.LeftProperty и Canvas.TopProperty.
                     *
                     * Для расчета текущего положения элемента управления необходимо:
                     * Текущая позиция курсора мыши на родительском объекте -
                     * Положение мыши на элементе управления в начале движения -
                     * позиция родителя элемента управления.
                     */
                var vv = FirstXPos;
               
                (MovingObject as FrameworkElement).SetValue(Canvas.LeftProperty,
                        e.GetPosition((MovingObject as FrameworkElement).Parent as FrameworkElement).X - FirstXPos);

                var ss = FirstYPos;

                (MovingObject as FrameworkElement).SetValue(Canvas.TopProperty,
                    e.GetPosition((MovingObject as FrameworkElement).Parent as FrameworkElement).Y - FirstYPos);

                MovingObject.Margin = new Thickness(vv - 12, ss - 12, 0, 0);
            }
        }
        private void PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MovingObject = null;  //  Освобождаем объект 
        }
        // ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void DataGridMain_Loaded(object sender, RoutedEventArgs e)                                       // загрузка данных в DataGrid
        {
            DataGridMain.ItemsSource = null;
            DataGridMain.ItemsSource = global.stackTabs;
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
