using UnityEngine;
using System.Net;
using System.Net.Sockets;

public class IP : MonoBehaviour
{
    public static string LocalIPAddress { get; private set; }

    void Awake()
    {
        if (string.IsNullOrEmpty(LocalIPAddress))
        {
            LocalIPAddress = GetLocalIPAddress();
        }
    }

    private string GetLocalIPAddress()
    {
        string ipAddress = "0.0.0.0";
        try
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                ipAddress = endPoint.Address.ToString();
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"Could not determine local IP: {ex.Message}");
        }

        return ipAddress;
    }
}
