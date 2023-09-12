using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RustDotNet
{
    internal static class Arrays
    {
        [DllImport("rdn")] private static extern UInt32 slice_sum(UInt32[] nums, UInt32 len);
        [DllImport("rdn")] private static extern void slice_increment(UInt32[] nums, UInt32 len);

        public static void SumNumbers(params uint[] nums)
        {
            uint sum = slice_sum(nums, (uint)nums.Length);
            Console.WriteLine($"The sum of these {nums.Length} numbers is {sum}");

            slice_increment(nums, (uint)nums.Length);
            sum = slice_sum(nums, (uint)nums.Length);
            Console.WriteLine($"After incrementing, the sum is {sum}");
        }
    }
}
