using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class FFI
{
    private static IntPtr _baton;

    [StructLayout(LayoutKind.Sequential)]
    public class PositionUpdate {
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
        Debug.Log(String.Format("TEST 1: {0}, {1}", position.x, position.y));
        update.x = (Int32)Mathf.RoundToInt(position.x * 10000);
        update.y = (Int32)Mathf.RoundToInt(position.y * 10000);
        Debug.Log(String.Format("TEST 2: {0}, {1}", update.x, update.y));
        send_position_update(_baton, update);
    }
}
