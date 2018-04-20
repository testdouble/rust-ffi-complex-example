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
    private static extern void send_ding(IntPtr baton);
    public static void sendDing()
    {
        send_ding(_baton);
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
}
