using UnityEngine;
using extOSC;

public class QuestOSCClient : MonoBehaviour
{
    [Header("OSC Settings")]
    [Tooltip("The port this Quest device will listen on.")]
    public int listenPort = 8100;

    [Tooltip("Remote IP to send OSC messages to (e.g., Game Manager PC).")]
    public string remoteHost = "192.168.68.61"; // Replace with your PC's IP
    public int remotePort = 8000;               // Replace with your PC's listening port

    public GameObject testPrefab;

    private OSCReceiver receiver;
    private OSCTransmitter transmitter;

    void Start()
    {
        // Setup OSC receiver
        receiver = gameObject.AddComponent<OSCReceiver>();
        receiver.LocalPort = listenPort;
        receiver.Bind("/StartGame", OnStartGame);


        // Setup OSC transmitter
        transmitter = gameObject.AddComponent<OSCTransmitter>();
        transmitter.RemoteHost = remoteHost;
        transmitter.RemotePort = remotePort;

        //InvokeRepeating(nameof(SendTestMessage), 0f, 1f);
    }

    void SendTestMessage()
    {
        SendOSCMessage("/PlayerAction", "WE HAVE MADE CONTACT");
    }

    void OnStartGame(OSCMessage message)
    {
        Debug.Log("[QuestOSCReceiver] Received /StartGame message!");

        GameObject sphere = Instantiate(testPrefab);
        sphere.GetComponent<Renderer>().material.color = Color.red;

        Debug.Log("[QuestOSCReceiver] Starting game now!");
    }

    // Public method to send OSC message
    public void SendOSCMessage(string address, string content)
    {
        if (transmitter == null)
        {
            Debug.LogWarning("Transmitter not initialized.");
            return;
        }

        var message = new OSCMessage(address);
        message.AddValue(OSCValue.String(content));
        transmitter.Send(message);

        Debug.Log($"[QuestOSCClient] Sent: {address} | {content}");
    }
}
