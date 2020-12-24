using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Krasnokam
{
    class Global
    {
        private static Global instance;
        public List<int> stackNumber;      // номера штабелей
        Global()
        {
            stackNumber = new List<int>();
        }

        public static Global getInstance() // возвращает singleton объекта Global
        {
            if (instance == null)
                instance = new Global();
            return instance;
        }
    }
}
