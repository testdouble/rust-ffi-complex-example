extern crate byteorder;

#[macro_use]
mod debug;

use self::byteorder::{NetworkEndian, ReadBytesExt, WriteBytesExt};
use std::ffi::CStr;
use std::io::Cursor;
use std::mem::{drop, transmute};
use std::net::UdpSocket;
use std::os::raw::c_char;
use std::ptr;
use std::result::Result;

pub struct Baton {
    socket: UdpSocket,
    server_url: String,
}

impl<'a> Baton {
    fn to_ptr(self) -> *mut Baton {
        unsafe { transmute(Box::new(self)) }
    }

    fn from_ptr(ptr: *mut Baton) -> &'a mut Baton {
        unsafe { &mut *ptr }
    }

    fn connect(url: &str) -> Result<Baton, String> {
        debug!("Connecting to {:}...", url);

        let socket = match UdpSocket::bind("0.0.0.0:0") {
            Ok(socket) => socket,
            Err(error) => return Err(format!("{:}", error)),
        };

        Ok(Baton { socket: socket, server_url: String::from(url) })
    }

    fn disconnect(ptr: *mut *mut Baton) {
        let baton: Box<Baton> = unsafe { transmute(*ptr) };

        drop(baton);
    }

    fn send_ding(&mut self) -> Result<(), String> {
        match self.socket.send_to(&[0; 10], &self.server_url) {
            Ok(_) => Ok(()),
            Err(error) => Err(format!("{:}", error)),
        }
    }

    fn send_position_update(&mut self, data: &mut PositionUpdate) -> Result<(), String> {
        debug!("Position update received: {:}, {:}", data.x, data.y);

        let mut cursor = Cursor::new(Vec::new());
        try_or_string!(cursor.write_u8(1));
        try_or_string!(cursor.write_i32::<NetworkEndian>(data.x));
        try_or_string!(cursor.write_i32::<NetworkEndian>(data.y));

        match self.socket.send_to(&cursor.into_inner(), &self.server_url) {
            Ok(_) => Ok(()),
            Err(error) => Err(format!("{:}", error)),
        }
    }
}

#[derive(Clone, Debug, PartialEq)]
#[repr(C)]
pub struct PositionUpdate {
    pub x: i32,
    pub y: i32,
}

impl<'a> PositionUpdate {
    fn to_ptr(self) -> *mut PositionUpdate {
        unsafe { transmute(Box::new(self)) }
    }

    fn from_ptr(ptr: *mut PositionUpdate) -> &'a mut PositionUpdate {
        unsafe { &mut *ptr }
    }
}

#[no_mangle]
pub extern "C" fn connect_to_server(ptr: *mut *const Baton, url: *const c_char) -> bool {
    let url = unsafe { CStr::from_ptr(url) };
    let url_str = match url.to_str() {
        Ok(slice) => slice,
        Err(_) => {
            debug!("Invalid UTF8 in URL.");
            return false;
        }
    };

    match Baton::connect(url_str) {
        Ok(baton) => {
            debug!("Connected.");

            unsafe {
                *ptr = baton.to_ptr();
            }

            true
        }
        Err(message) => {
            debug!("Failed to connect: {:}", message);

            unsafe {
                *ptr = ptr::null();
            }

            false
        }
    }
}

#[no_mangle]
pub extern "C" fn disconnect_from_server(ptr: *mut *mut Baton) {
    if !ptr.is_null() && unsafe { !(*ptr).is_null() } {
        Baton::disconnect(ptr);

        debug!("Disconnected.");

        unsafe {
            *ptr = ptr::null_mut();
        }
    }
}

#[no_mangle]
pub extern "C" fn send_ding(ptr: *mut Baton) -> bool {
    if !ptr.is_null() {
        match Baton::from_ptr(ptr).send_ding() {
            Ok(_) => true,
            Err(message) => {
                debug!("Error while sending: {:}", message);

                false
            }
        }
    } else {
        false
    }
}

#[no_mangle]
pub extern "C" fn send_position_update(ptr: *mut Baton, data: *mut PositionUpdate) -> bool {
    if !ptr.is_null() && !data.is_null() {
        match Baton::from_ptr(ptr).send_position_update(PositionUpdate::from_ptr(data)) {
            Ok(_) => true,
            Err(message) => {
                debug!("Error while sending: {:}", message);

                false
            }
        }
    } else {
        false
    }
}
