using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RustDotNet
{
    internal static class Strings
    {
        [DllImport("rdn")] private static extern void say_hi(string name);
        [DllImport("rdn")] private static extern void say_hi_lputf8str([MarshalAs(UnmanagedType.LPUTF8Str)] string name);

        public static void SayHi(string name)
        {
            Console.WriteLine($"Saying hi to '{name}':");
            say_hi(name);
            say_hi_lputf8str(name);
            Console.WriteLine();
        }

        [DllImport("rdn")] private static extern IntPtr string_num_new(int num);
        [DllImport("rdn")] private static extern void string_num_free(IntPtr ptr);
        private class RustStringPtr : SafeHandle
        {
            public override bool IsInvalid => this.handle != IntPtr.Zero;

            protected override bool ReleaseHandle()
            {
                if (this.handle != IntPtr.Zero)
                    string_num_free(this.handle);
                return true;
            }

            public override string ToString()
            {
                if (this.handle == IntPtr.Zero)
                    return null;

                // Find the NULL byte that ends the c string
                int len = 0;
                while (Marshal.ReadByte(this.handle, len) != 0)
                    len++;

                byte[] buf = new byte[len];
                Marshal.Copy(this.handle, buf, 0, len);
                return Encoding.UTF8.GetString(buf);
            }

            public RustStringPtr() : base(IntPtr.Zero, true) { }

            public RustStringPtr(IntPtr ptr) : base(ptr, true) { }
        }

        public static void NumToString(int num)
        {
            IntPtr ptr = string_num_new(num);
            RustStringPtr rsp = new RustStringPtr(ptr);
            Console.WriteLine(rsp);
            rsp.Dispose();
        }
    }
}
