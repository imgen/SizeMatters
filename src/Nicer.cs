using System;
using System.Globalization;

namespace SizeMatters
{
    public static class Nicer
    {
        private static readonly string[] Prefixes = { "f", "a", "p", "n", "μ", "m", string.Empty, "k", "M", "G", "T", "P", "E" };

        public static string Nice(this double x, int significantDigits)
        {
            if (x < 1000)
            {
                return x.ToString(CultureInfo.InvariantCulture);
            }
            //Check for special numbers and non-numbers
            if(double.IsInfinity(x)||double.IsNaN(x)||x==0||significantDigits<=0)
            {
                return x.ToString(CultureInfo.InvariantCulture);
            }
            // extract sign so we deal with positive numbers only
            var sign=Math.Sign(x);
            x=Math.Abs(x);
            // get scientific exponent, 10^3, 10^6, ...
            int sci= x==0? 0 : (int)Math.Floor(Math.Log(x, 10)/3)*3;
            // scale number to exponent found
            x=x*Math.Pow(10, -sci);
            // find number of digits to the left of the decimal
            int dg= x==0? 0 : (int)Math.Floor(Math.Log(x, 10))+1;
            // adjust decimals to display
            int decimals=Math.Min(significantDigits-dg, 15);
            // format for the decimals
            var fmt=new string('0', decimals);
            if(sci==0)
            {
                //no exponent
                return string.Format("{0}{1:0."+fmt+"}",
                    sign<0?"-":string.Empty,
                    Math.Round(x, decimals));
            }
            // find index for prefix. every 3 of sci is a new index
            int index=sci/3+6;
            if(index>=0&&index<Prefixes.Length)
            {
                // with prefix
                return string.Format("{0}{1:0."+fmt+"}{2}",
                    sign<0?"-":string.Empty,
                    Math.Round(x, decimals),
                    Prefixes[index]);
            }
            // with 10^exp format
            return string.Format("{0}{1:0."+fmt+"}·10^{2}",
                sign<0?"-":string.Empty,
                Math.Round(x, decimals),
                sci);
        }
    }
}