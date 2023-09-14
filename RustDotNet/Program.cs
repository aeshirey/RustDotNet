using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RustDotNet
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Functions.Add(2, 3);
            Functions.MutateInt(27);

            Strings.SayHi("Adam");
            Strings.SayHi("Agnès");
            Strings.SayHi(@"¯\_(ツ)_/¯");
            Strings.NumToString(-123);
            Strings.NumToString(0);
            Strings.NumToString(12345);

            Structs.PointsExample();
            Structs.CounterExample();

            Arrays.SumNumbers(2, 4, 6, 8);
        }
    }
}
