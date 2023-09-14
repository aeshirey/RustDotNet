#[no_mangle]
/// Returns the sum of all values within a Rust slice (C# array), or zero if the slice is null.
extern "C" fn slice_sum(slice: *const u32, len: u32) -> u32 {
    if slice.is_null() {
        return 0;
    }

    let slice = unsafe { std::slice::from_raw_parts(slice, len as usize) };
    slice.iter().sum()
}

#[no_mangle]
/// Increments every value within a Rust slice (C# array).
extern "C" fn slice_increment(slice: *mut u32, len: u32) {
    if slice.is_null() {
        return;
    }

    let slice = unsafe { std::slice::from_raw_parts_mut(slice, len as usize) };

    for val in slice.iter_mut() {
        *val += 1;
    }
}
