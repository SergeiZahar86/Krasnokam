using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Krasnokam
{

    public partial class DraggableSquare : UserControl
    {

        Vector relativeMousePos;                // смещение мыши от левого верхнего угла квадрата
        Canvas container;                       // канвас-контейнер

        public DraggableSquare()
        {
            InitializeComponent();
            // устанавливаем Binding RequestMove из VM на свойство RequestMoveCommand:
            SetBinding(RequestMoveCommandProperty, new Binding("RequestMove"));
        }


        #region dp Shape DraggedImageContainer
        public Shape DraggedImageContainer
        {
            get
            {
                return (Shape)GetValue(DraggedImageContainerProperty);
            }
            set
            {
                SetValue(DraggedImageContainerProperty, value);
            }
        }

        public static readonly DependencyProperty DraggedImageContainerProperty = DependencyProperty.Register("DraggedImageContainer", typeof(Shape), typeof(DraggableSquare));
        #endregion


        // стандартное DependencyProperty
        #region dp ICommand RequestMoveCommand
        public ICommand RequestMoveCommand
        {
            get
            {
                return (ICommand)GetValue(RequestMoveCommandProperty);
            }
            set
            {
                SetValue(RequestMoveCommandProperty, value);
            }
        }

        public static readonly DependencyProperty RequestMoveCommandProperty = DependencyProperty.Register("RequestMoveCommand", typeof(ICommand), typeof(DraggableSquare));
        #endregion





        void OnMouseDown(object sender, MouseButtonEventArgs e)        // по нажатию на левую клавишу начинаем следить за мышью
        {
            container = FindParent<Canvas>(this);
            relativeMousePos = e.GetPosition(this) - new Point();
            MouseMove += OnDragMove;
            LostMouseCapture += OnLostCapture;
            Mouse.Capture(this);
        }


        void OnMouseUp(object sender, MouseButtonEventArgs e)          // клавиша отпущена - завершаем процесс
        {
            FinishDrag(sender, e);
            Mouse.Capture(null);
        }


        void OnLostCapture(object sender, MouseEventArgs e)           // потеряли фокус (например, юзер переключился в другое окно) - завершаем тоже
        {
            FinishDrag(sender, e);
        }

        void OnDragMove(object sender, MouseEventArgs e)
        {
            UpdateDraggedSquarePosition(e);
        }

        void FinishDrag(object sender, MouseEventArgs e)
        {
            MouseMove -= OnDragMove;
            LostMouseCapture -= OnLostCapture;
            UpdatePosition(e);
            UpdateDraggedSquarePosition(null);
        }

        // требуем у VM обновить позицию через команду
        void UpdatePosition(MouseEventArgs e)
        {
            var point = e.GetPosition(container);
            // не забываем проверку на null
            RequestMoveCommand?.Execute(point - relativeMousePos);
        }
        void UpdateDraggedSquarePosition(MouseEventArgs e)
        {
            var dragImageContainer = DraggedImageContainer;

            if (dragImageContainer == null)
                return;

            var needVisible = e != null;
            var wasVisible = dragImageContainer.Visibility == Visibility.Visible;

            // включаем/выключаем видимость перемещаемой картинки
            dragImageContainer.Visibility = needVisible ? Visibility.Visible : Visibility.Collapsed;

            if (!needVisible) // если мы выключились, нам больше нечего делать
                return;

            if (!wasVisible) // а если мы были выключены и включились,
            {                // нам надо привязать изображение себя
                dragImageContainer.Fill = new VisualBrush(this);
                dragImageContainer.SetBinding(Shape.WidthProperty, new Binding(nameof(ActualWidth)) { Source = this });  // а также ширину/высоту
                dragImageContainer.SetBinding(Shape.HeightProperty, new Binding(nameof(ActualHeight)) { Source = this });
                // Binding нужен потому, что наш размер может по идее измениться
            }

            // перемещаем картинку на нужную позицию
            var parent = FindParent<Canvas>(dragImageContainer);
            var position = e.GetPosition(parent) - relativeMousePos;
            Canvas.SetLeft(dragImageContainer, position.X);
            Canvas.SetTop(dragImageContainer, position.Y);
        }

        static private T FindParent<T>(FrameworkElement from) where T : FrameworkElement    // это вспомогательная функция, ей место в общей библиотеке
        {
            FrameworkElement current = from;
            T t;
            do
            {
                t = current as T;
                current = (FrameworkElement)VisualTreeHelper.GetParent(current);
            }
            while (t == null && current != null);
            return t;
        }
    }
}
