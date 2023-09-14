using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RustDotNet
{
    internal static class Functions
    {
        [DllImport("rdn")] private static extern ulong add(ulong a, ulong b);
        public static void Add(ulong a, ulong b)
        {
            Console.WriteLine($"Adding {a} + {b}:");
            ulong sum = add(a, b);
            Console.WriteLine($"    sum = {sum}");
            Console.WriteLine();
        }

        [DllImport("rdn")] private static extern void add_in_place(ref int num);
        public static void MutateInt(int num)
        {
            Console.WriteLine($"Num starts off as {num}");
            add_in_place(ref num);
            Console.WriteLine($"After mutating, num is {num}");
        }
    }
}
