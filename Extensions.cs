using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComPortReader
{
    public static class EnumExt
    {
        public static string ToFriendlyString(this Parity me)
        {
            switch (me)
            {
                case Parity.Even:
                    return "Число битов четное";
                case Parity.Odd:
                    return "Число битов нечетное";
                case Parity.None:
                    return "Нет";
                case Parity.Mark:
                    return "Бит четности 1";
                case Parity.Space:
                    return "Бит четности 0";
                default:
                    return "???";


            }
        }

        public static string ToFriendlyString(this StopBits me)
        {
            switch (me)
            {
                case StopBits.None:
                    return "Нет";
                case StopBits.One:
                    return "1";
                case StopBits.OnePointFive:
                    return "1.5";
                case StopBits.Two:
                    return "2";
                default:
                    return "???";


            }
        }
    }
}
