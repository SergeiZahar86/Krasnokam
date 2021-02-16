using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Krasnokam
{
    public class StackTab
    {
        public StackTab(int stack_id, string material, double cinder, double humidity, string task, 
            double weight_now, double weight_actually, string status)
        {
            Stack_id = stack_id;
            Material = material;
            Cinder = cinder;
            Humidity = humidity;
            Task = task;
            Weight_now = weight_now;
            Weight_actually = weight_actually;
            Status = status;
        }


        public int Stack_id { get; set; }              // Номер штабеля
        public string Material { get; set; }           // Сырьё
        public double Cinder { get; set; }             // Зольность
        public double Humidity { get; set; }           // Влажность
        public string Task { get; set; }               // Задание на смену
        public double Weight_now { get; set; }         // Текущий вес
        public double Weight_actually { get; set; }    // Факт за смену
        public string Status { get; set; }             // Статус

    }
}
