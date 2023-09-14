use libc::c_char;
use std::ffi::CString;

#[no_mangle]
fn say_hi(name: *const c_char) {
    let name = unsafe {
        assert!(!name.is_null());
        std::ffi::CStr::from_ptr(name)
    };

    if let Ok(name) = name.to_str() {
        println!("Hi, {name}!");
    } else {
        println!("Hi, invalid utf value! ({})", name.to_string_lossy());
    }
}

#[no_mangle]
fn say_hi_lputf8str(name: *const c_char) {
    let name = unsafe {
        assert!(!name.is_null());
        std::ffi::CStr::from_ptr(name)
    };

    if let Ok(name) = name.to_str() {
        println!("[LPUTF8] Hi, {name}!");
    } else {
        println!("[LPUTF8] Hi, invalid utf value! ({})", name.to_string_lossy());
    }
}

#[no_mangle]
/// This function will use Rust's allocator to allocate a string on the heap, and it
/// converts it to a null-terminated C string, returning a pointer to _that_ memory.
///
/// The caller is responsible for calling `free_string_ptr` to free the memory.
extern "C" fn string_num_new(num: i32) -> *const c_char {
    if num < 0 {
        std::ptr::null()
    } else if num == 0 {
        let cstr = CString::new("zero").unwrap();
        cstr.into_raw()
    } else {
        let cstr = CString::new(num.to_string()).unwrap();
        cstr.into_raw()
    }
}

#[no_mangle]
/// This function will free the memory allocated by `alloc_string_ptr`.
extern "C" fn string_num_free(s: *mut c_char) {
    if s.is_null() {
        return;
    }

    let s = unsafe { CString::from_raw(s) };

    if let Ok(s) = s.into_string() {
        drop(s);
    }
}
