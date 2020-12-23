using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Krasnokam
{
    class MainVM : VM
    {
        public ObservableCollection<SquareVM> Squares { get; } =
        new ObservableCollection<SquareVM>()
        {
            new SquareVM() { Position = new Point(30, 30),
                             Color = Color.FromRgb(0x3D, 0x31, 0x5B) },
            new SquareVM() { Position = new Point(100, 70),
                             Color = Color.FromRgb(0x44, 0x4B, 0x6E) },
            new SquareVM() { Position = new Point(80, 0),
                             Color = Color.FromRgb(0x70, 0x8B, 0x75) },
            new SquareVM() { Position = new Point(90, 180),
                             Color = Color.FromRgb(0x9A, 0xB8, 0x7A) },
            new SquareVM() { Position = new Point(200, 200),
                             Color = Color.FromRgb(0xF8, 0xF9, 0x91) }
        };
    }
}
