# Rust.NET
Sample code for calling Rust from .NET (C# specifically). Rust code exports unmangled C-ABI functions that C# declares using [P/Invokes](https://learn.microsoft.com/en-us/dotnet/standard/native-interop/pinvoke).

See the sections below for more detail. 

## TODO
- [ ] Other .NET languages
- [ ] Call .NET from Rust


# `rust-dot-net`
The Rust project is declared as a [`cdylib`](https://doc.rust-lang.org/reference/linkage.html) with a name of "rdn"; this compiles on Windows to "rdn.dll".

```toml
[lib]
name = "rdn"
crate-type = ["cdylib"]
```

This project makes use of [`libc`](https://crates.io/crates/libc) for easy interop -- in particular, with .NET strings.

The [default marshalling behavior for strings in .NET](https://learn.microsoft.com/en-us/dotnet/framework/interop/default-marshalling-for-strings#strings-used-in-platform-invoke) is `LPStr`, a pointer to a null-terminated array of ANSI characters. This ends up working seamlessly for ASCII/ANSI[*](https://stackoverflow.com/a/700221/1191181) strings but can fail for other strings that can't be interpreted in Rust as UTF8. Therefore, this project demonstrates both this default behavior (in the `say_hi` function) and the extended `LPUTF8Str`, a pointer to a null-terminated array of UTF-8 encoded characters (in the `say_hi_lputf8str` function):

```
Saying hi to 'Adam':
Hi, Adam!
[LPUTF8] Hi, Adam!

Saying hi to 'Agnès':
Hi, invalid utf value! (Agn�s)
[LPUTF8] Hi, Agnès!

Saying hi to '_\_(?)_/_':
Hi, invalid utf value! (�\_(?)_/�)
[LPUTF8] Hi, ¯\_(ツ)_/¯!
```

To compile this library, simply run `cargo build --release` from a Windows console. This creates `./target/release/rdn.dll`. Alternately, you can cross-compile from Linux:

```bash
$ rustup target add x86_64-pc-windows-gnu
$ cargo build --release --target x86_64-pc-windows-gnu
```

This creates `./target/x86_64-pc-windows-gnu/release/rdn.dll`. In both cases, the resulting DLL is approximately 5 MiB. You can strip this to about 1 MiB:

```bash
$ ls -lh rdn.dll
-rwxr-xr-x 2 adam adam 5.2M Sep 14 07:36 rdn.dll
$ strip rdn.dll
$ ls -lh rdn.dll
-rwxr-xr-x 2 adam adam 1003K Sep 14 07:48 rdn.dll
```


# `RustDotNet`
The `RustDotNet` project is a sample C# console app that calls into the above Rust library. This example uses .NET Framework 4.8 and is intended to be run on Windows. I have not yet evaluated how to run .NET Core in Linux to call into a .so library.

As of 2023-09-14, I am not aware of any Rust projects that automatically generate the C# P/Invokes for Rust code. Instead, we need to write them manually. From within C#, we make use of [`System.Runtime.InteropServices.DllImportAttribute`](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.dllimportattribute?view=net-7.0). A simple example looks like this:

```csharp
[DllImport("rdn")] private static extern ulong add(ulong a, ulong b);
```

Notes:
* We're importing the "rdn.dll" file (the file suffix is omitted, as it's understood to be ".dll"). 
* The P/Invoke is declared insde a class, but class instances aren't relevant to the function call, so it's marked as `static`.
* The function is external to our .NET code, so no body is specified.
* The function name, parameter types, and return type are declared similarly to other C# functions. The _names_ of the parameters do not matter. CLR types must match Rust types; for example, a CLR `ulong` matches Rust's `u64`.
