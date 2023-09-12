#[no_mangle]
extern "C" fn add(left: u64, right: u64) -> u64 {
    left + right
}

#[no_mangle]
extern "C" fn add_in_place(num: &mut i32) {
    *num += 1;
}
