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

    void Start()
    {
        // Setup OSC receiver
        receiver = gameObject.AddComponent<OSCReceiver>();
        receiver.LocalPort = listenPort;
        receiver.Bind("/StartGame", OnStartGame);
        //receiver.Bind("/JoinGame", OnJoinGame);

        // Setup OSC transmitter (remoteHost will be updated when StartGame is received)
        transmitter = gameObject.AddComponent<OSCTransmitter>();
        transmitter.RemoteHost = remoteHost; // Initial fallback
        transmitter.RemotePort = remotePort;

        //InvokeRepeating(nameof(SendTestMessage), 0f, 1f);
    }

    void OnEnable()
    {
        Drumstick.OnDrumHit += HandleDrumHit;
    }

    void OnDisable()
    {
        Drumstick.OnDrumHit -= HandleDrumHit;
    }

    void HandleDrumHit(bool inSync)
    {
        if (inSync)
        {
            SendOSCMessage("/DrumHit", "Drum hit in sync");            
        }
        else
        {
            SendOSCMessage("/DrumHit", "Drum hit out of sync");  
        }
    }

    void OnJoinGame()
    {

    }

    void OnStartGame(OSCMessage message)
    {
        GameObject sphere = Instantiate(testPrefab);
        sphere.GetComponent<Renderer>().material.color = Color.red;

        // Extract floats and string (IP) from the OSC message
        List<float> receivedSequence = new List<float>();
        string managerIP = null;

        foreach (var val in message.Values)
        {
            if (val.Type == OSCValueType.Float)
            {
                receivedSequence.Add(val.FloatValue);
            }
            else if (val.Type == OSCValueType.String)
            {
                managerIP = val.StringValue;
            }
        }

        Debug.Log("[Quest] Received startGame sequence with " + receivedSequence.Count + " intervals");

        // Update transmitter RemoteHost if IP was received
        if (!string.IsNullOrEmpty(managerIP))
        {
            transmitter.RemoteHost = managerIP;
            Debug.Log("[Quest] Updated RemoteHost to " + managerIP);
        }
        else
        {
            Debug.LogWarning("[Quest] No IP address received in /StartGame message.");
        }

        // Start the game with the received sequence
        notesManager.startGame(receivedSequence);
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
}
