using UnityEngine;
using extOSC;
using System.Collections.Generic;

public class QuestOSCClient : MonoBehaviour
{
    [Header("OSC Settings")]
    [Tooltip("The port this Quest device will listen on.")]
    public int listenPort = 8100;

    [Tooltip("Remote IP to send OSC messages to (e.g., Game Manager PC).")]
    public string remoteHost; 
    public int remotePort = 8000;

    public GameObject testPrefab;
    public NotesManager notesManager;

    private OSCReceiver receiver;
    private OSCTransmitter transmitter;

    [Tooltip("Script that keeps track of the current state of the game")] // Not joined, joined but unCalibrated, joined and Calibrated, game on
    public GameManager GM;

    void Start()
    {
        // Setup OSC receiver
        receiver = gameObject.AddComponent<OSCReceiver>();
        receiver.LocalPort = listenPort;
        receiver.Bind("/StartGame", OnStartGame);
        receiver.Bind("/EndGame", OnEndGame);
        receiver.Bind("/SessionOpen", OnSessionOpen);
        receiver.Bind("/Calibrate", OnCalibrate);

        // Setup OSC transmitter (remoteHost will be updated when StartGame is received)
        transmitter = gameObject.AddComponent<OSCTransmitter>();
        transmitter.RemoteHost = remoteHost; // Initial fallback
        transmitter.RemotePort = remotePort;
    }

    void OnSessionOpen(OSCMessage message)
    {
        GM.JoinGame();
        GetIPFromMessage(message);
    }

    void OnCalibrate(OSCMessage message)
    {
        GM.Calibrate();
    }

    void OnStartGame(OSCMessage message)
    {
        GM.StartGame(message);
    }

    void OnEndGame(OSCMessage message)
    {
        GM.EndGame();
    }

    public void SendOSCMessage(string address, string content)
    {
        if (transmitter == null)
        {
            Debug.LogWarning("Transmitter not initialized.");
            return;
        }

        var message = new OSCMessage(address);
        message.AddValue(OSCValue.String(content));
        message.AddValue(OSCValue.String(IP.LocalIPAddress)); // Include Quest's own local IP
        transmitter.Send(message);
    }

    private void GetIPFromMessage(OSCMessage message)
    {
        // Get IP of the PC (from the message)
        string ip = null;
        foreach (var val in message.Values)
        {
            if (val.Type == OSCValueType.String)
            {
                ip = val.StringValue;
            }
        }
        // Assign that IP as the remote host
        if (!string.IsNullOrEmpty(ip))
        {
            transmitter.RemoteHost = ip;
            Debug.Log("[Quest] Updated RemoteHost to " + ip);
        }
        else
        {
            Debug.LogWarning("[Quest] No IP found in /JoinGame message.");
        }        
    }
}
