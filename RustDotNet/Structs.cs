using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RustDotNet
{
    [StructLayout(LayoutKind.Sequential)]
    struct Point
    {
        public double x;
        public double y;
    }

    class Counter
    {
        private IntPtr ptr;
        [DllImport("rdn")] private static extern IntPtr counter_new();
        [DllImport("rdn")] private static extern void counter_track(IntPtr ptr, string name);
        [DllImport("rdn")] private static extern UInt32 counter_get_count(IntPtr ptr);
        [DllImport("rdn")] private static extern void counter_print_last_seen(IntPtr ptr);
        [DllImport("rdn")] private static extern void counter_free(IntPtr ptr);

        /// <summary>
        /// Gets the pointer to the memory that Rust allocates because the backing data structure
        /// is stored on the heap.
        /// </summary>
        public Counter() => ptr = counter_new();

        public void TrackPerson(string name) => counter_track(this.ptr, name);

        public UInt32 GetCount() => counter_get_count(this.ptr);

        public void PrintLastSeen() => counter_print_last_seen(this.ptr);

        /// <summary>
        /// We have to pass the pointer back to Rust so it can free the memory!
        /// </summary>
        ~Counter() => counter_free(this.ptr);
    }

    internal static class Structs
    {
        [DllImport("rdn")] private static extern Point point_new(double x, double y);
        [DllImport("rdn")] private static extern double point_distance_origin(Point p);
        [DllImport("rdn")] private static extern double point_distance(Point p1, Point p2);

        /// <summary>
        /// Uses a stack-based ("Copy") Rust struct, with C# having direct access to values.
        /// </summary>
        public static void PointsExample()
        {
            Point p1 = point_new(1.0, 1.0),
                p3 = point_new(3.0, 3.0);

            double p1_dist = point_distance_origin(p1),
                p1_p3_dist = point_distance(p1, p3);

            Console.WriteLine($"Point (1,1) is {p1_dist} from the origin");
            Console.WriteLine($"Points (1,1) and (3,3) are {p1_p3_dist} from each other");
        }

        /// <summary>
        /// Uses a heap-allocated Rust struct, where C# only has a pointer to the value.
        /// </summary>
        public static void CounterExample()
        {
            Counter c = new Counter();
            Console.WriteLine($"New counter has {c.GetCount()} hits");

            c.TrackPerson("Adam");
            Console.WriteLine($"After the first person, the count is {c.GetCount()}:");
            c.PrintLastSeen();

            c.TrackPerson("Shasta");
            Console.WriteLine($"The count is now {c.GetCount()}");
        }
    }
}
