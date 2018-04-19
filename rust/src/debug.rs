#[macro_export]
macro_rules! debug {
    () => (debug!(""));
    ($fmt:expr) => (match ::std::fs::OpenOptions::new()
        .append(true)
        .create(true)
        .open("ffi_log") {
            Ok(mut file) => {
                use std::io::Write;
                file.write_all(concat!($fmt, "\n").as_bytes()).ok();
            }
            Err(_) => {},
        });
    ($fmt:expr, $($arg:tt)*) => (match ::std::fs::OpenOptions::new()
        .append(true)
        .create(true)
        .open("ffi_log") {
            Ok(mut file) => {
                use std::io::Write;
                file.write_all(format!(concat!($fmt, "\n"), $($arg)*).as_bytes()).ok();
            }
            Err(_) => {},
        });
}

macro_rules! try_or_string {
    ($operation:expr) => (if let Err(error) = $operation {
        return Err(format!("{:}", error));
    })
}
