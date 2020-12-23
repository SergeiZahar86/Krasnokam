using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Krasnokam
{
    class SquareVM : VM
    {
        // выставляем команду, которая занимается перемещением
        public ICommand RequestMove { get; }
        public SquareVM()
        {
            RequestMove = new SimpleCommand<Point>(MoveTo);
        }

        // стандартное свойство
        Point position;
        Color color;
        public Color Color
        {
            get { return color; }
            set
            {
                if (color != value)
                {
                    color = value; NotifyPropertyChanged();
                }
            }
        }
        public Point Position
        {
            get { return position; }
            set
            {
                if (position != value)
                {
                    position = value; NotifyPropertyChanged();
                }
            }
        }
        void MoveTo(Point newPosition)
        {
            // в реальности тут могут быть всякие проверки, конечно
            Position = newPosition;
        }
    }
}
