use libc::c_char;

/// The `Point` struct is stack-based -- no heap allocation required, so we can read/create
/// without any concern for passing pointers.
#[repr(C)]
struct Point {
    x: f64,
    y: f64,
}

#[no_mangle]
extern "C" fn point_new(x: f64, y: f64) -> Point {
    Point { x, y }
}

#[no_mangle]
extern "C" fn point_distance_origin(point: Point) -> f64 {
    (point.x.powi(2) + point.y.powi(2)).sqrt()
}

#[no_mangle]
extern "C" fn point_distance(point1: Point, point2: Point) -> f64 {
    ((point1.x - point2.x).powi(2) + (point1.y - point2.y).powi(2)).sqrt()
}

#[repr(C)]
pub struct Counter {
    count: u32,
    last_seen: Option<String>,
}

#[no_mangle]
extern "C" fn counter_new() -> *mut Counter {
    Box::into_raw(Box::new(Counter {
        count: 0,
        last_seen: None,
    }))
}

#[no_mangle]
extern "C" fn counter_track(counter: *mut Counter, name: *const c_char) {
    if counter.is_null() || name.is_null() {
        return;
    }

    let counter = unsafe { &mut *counter };
    let name = unsafe { std::ffi::CStr::from_ptr(name) };

    if let Ok(name) = name.to_str() {
        counter.count += 1;
        counter.last_seen = Some(name.to_string());
    }
}

#[no_mangle]
extern "C" fn counter_get_count(counter: *const Counter) -> u32 {
    if counter.is_null() {
        return 0;
    }

    let counter = unsafe { &*counter };

    counter.count
}

#[no_mangle]
extern "C" fn counter_print_last_seen(counter: *const Counter) {
    if counter.is_null() {
        eprintln!("Null pointer passed to counter_print_last_seen");
        return;
    }

    let counter = unsafe { &*counter };

    match &counter.last_seen {
        Some(name) => println!("I most recently saw {name}"),
        None => println!("I haven't seen anyone yet!"),
    }
}

#[no_mangle]
extern "C" fn counter_free(counter: *mut Counter) {
    if counter.is_null() {
        return;
    }

    unsafe {
        drop(Box::from_raw(counter));
    }
}
