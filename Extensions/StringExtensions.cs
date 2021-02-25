using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Develover.WebUI.Extensions
{
    public static class StringExtensions
    {
        public static string ToLowerFirstChar(this string input)
        {
            string newString = input;
            if (!String.IsNullOrEmpty(newString) && Char.IsUpper(newString[0]))
                newString = Char.ToLower(newString[0]) + newString.Substring(1);
            return newString;
        }
        public static string CreateNoByStructureNo(this string @this, int noLenght, string structureNo)
        {
            int no = 0;

            if (@this != null)
            {
                if (!int.TryParse(@this, out no))
                {
                    try
                    {
                        int vitrino = structureNo.LastIndexOf("{NO}");

                        string affterNo = structureNo[..vitrino];
                        int countChartInaffterNo  = affterNo.CountChar('{') + affterNo.CountChar('}');

                        @this = @this.Substring(structureNo.LastIndexOf("{NO}") - countChartInaffterNo, noLenght);
                        int.TryParse(@this, out no);
                    }
                    catch { }
                }
            }

            return structureNo.Replace("{YYYY}", $"{ DateTime.Now:yyyy}").Replace("{YY}", $"{ DateTime.Now:yy}").Replace("{MM}", $"{ DateTime.Now:MM}").Replace("{M}", $"{ DateTime.Now.Month}").Replace("{DD}", $"{ DateTime.Now:dd}").Replace("{D}", $"{ DateTime.Now.Day}").Replace("{NO}", (++no).ToString().PadLeft(noLenght, '0'));
        }
        public static int CountChar(this string @this, char _char)
        {
            int count = 0;
            foreach (char c in @this)
            {
                if (c == _char) count++;
            }
            return count;
        }
    }
}
