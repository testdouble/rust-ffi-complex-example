using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class FFI
{
    private static IntPtr _baton;
    private static UInt32 _id = (UInt32)(new System.Random()).Next();

    [StructLayout(LayoutKind.Sequential)]
    public class PositionUpdate {
        public UInt32 id;
        public Int32 x;
        public Int32 y;
    }

    public enum StatusUpdate
    {
        Ping = 0,
        Connect,
        Disconnect,
    }

    public enum UpdateType {
        None = 0,
        Connect,
        Disconnect,
        Position,
    }

    [StructLayout(LayoutKind.Sequential)]
    public class IncomingUpdate {
        public byte type = (byte)UpdateType.None;
        public UInt32 id = 0;
        public Int32 x = 0;
        public Int32 y = 0;
    }

    [DllImport("libffi_example")]
    private static extern bool connect_to_server(out IntPtr baton, byte[] url);
    public static bool connectToServer(string url)
    {
        return connect_to_server(out _baton, Encoding.UTF8.GetBytes(url));
    }

    [DllImport("libffi_example")]
    private static extern void disconnect_from_server(out IntPtr baton);
    public static void disconnectFromServer()
    {
        disconnect_from_server(out _baton);
    }

    [DllImport("libffi_example")]
    private static extern void send_status_update(IntPtr baton, UInt32 id, byte status);
    public static void sendStatusUpdate(StatusUpdate status)
    {
        send_status_update(_baton, _id, (byte)status);
    }

    [DllImport("libffi_example")]
    private static extern void send_position_update(IntPtr baton, PositionUpdate data);
    public static void sendPositionUpdate(Vector3 position)
    {
        PositionUpdate update = new PositionUpdate();

        update.id = _id;
        update.x = (Int32)Mathf.RoundToInt(position.x * 10000);
        update.y = (Int32)Mathf.RoundToInt(position.y * 10000);

        send_position_update(_baton, update);
    }

    [DllImport("libffi_example")]
    private static extern void read_next_update(IntPtr baton, IncomingUpdate update);
    public static IncomingUpdate readNextUpdate()
    {
        IncomingUpdate update = new IncomingUpdate();

        read_next_update(_baton, update);

        return update;
    }
}
