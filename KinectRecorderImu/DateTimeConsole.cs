using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectImuRecorder
{
    class DateTimeConsole
    {
        public static void WriteLine(string msg)
        {
            Console.Write(String.Format("{0:HH:mm:ss.fff} {1}\n", DateTime.Now, msg));
        }

        public static void ReadLine()
        {
            Console.ReadLine();
        }


    }
}
