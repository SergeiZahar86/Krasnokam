using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Krasnokam
{
    class Global
    {
        private static Global instance;
        public List<int> stackNumber;              // номера штабелей
        public List<StackTab> stackTabs;           // список штабелей
        public List<UIElement> Bord;           // список меток на canvas
        public bool deleetBorder;                 // флаг для удаления штабеля

        public int Stack_id { get; set; }              // Номер штабеля
        public string Material { get; set; }           // Сырьё
        public double Cinder { get; set; }             // Зольность
        public double Humidity { get; set; }           // Влажность
        public string Task { get; set; }               // Задание на смену
        public double Weight_now { get; set; }         // Текущий вес
        public double Weight_actually { get; set; }    // Факт за смену
        public string Status { get; set; }             // Статус
        public double Margin_left { get; set; }        // Координата метки с левого края
        public double Margin_top { get; set; }         // Координата метки с верхнего края
        public double Margin_right { get; set; }       // Координата метки с правого края
        public double Margin_bottom { get; set; }      // Координата метки с нижнего края

        Global()
        {
            stackNumber = new List<int>();
            stackTabs = new List<StackTab>();
            Bord = new List<UIElement>();
        }

        public static Global getInstance() // возвращает singleton объекта Global
        {
            if (instance == null)
                instance = new Global();
            return instance;
        }
    }
}
