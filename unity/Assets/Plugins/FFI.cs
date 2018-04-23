using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class FFI
{
    private static IntPtr _baton;
    private static UInt32 _id = (UInt32)(new System.Random()).Next();

    public class Update {
        public UpdateType type;
        public UInt32 id;
        public Vector3 position;
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
    private class rIncomingUpdate {
        public byte type = (byte)UpdateType.None;
        public UInt32 id = 0;
        public Int32 x = 0;
        public Int32 y = 0;
    }

    [StructLayout(LayoutKind.Sequential)]
    private class rPositionUpdate {
        public UInt32 id;
        public Int32 x;
        public Int32 y;
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
    private static extern void send_position_update(IntPtr baton, rPositionUpdate data);
    public static void sendPositionUpdate(Vector3 position)
    {
        rPositionUpdate update = new rPositionUpdate();

        update.id = _id;
        update.x = (Int32)Mathf.RoundToInt(position.x * 10000);
        update.y = (Int32)Mathf.RoundToInt(position.y * 10000);

        send_position_update(_baton, update);
    }

    [DllImport("libffi_example")]
    private static extern void read_next_update(IntPtr baton, rIncomingUpdate update);
    public static Update readNextUpdate()
    {
        rIncomingUpdate incoming = new rIncomingUpdate();

        read_next_update(_baton, incoming);

        Update update = new Update();
        update.type = (UpdateType)incoming.type;
        update.id = incoming.id;
        update.position = new Vector3(incoming.x / 10000.0f, incoming.y / 10000.0f, 0.0f);

        return update;
    }
}
