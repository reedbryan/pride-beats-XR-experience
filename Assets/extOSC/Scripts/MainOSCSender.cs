using UnityEngine;
using extOSC;

public class MainOSCSender : MonoBehaviour
{
    [Header("Connection Settings")]
    public string remoteHost = "127.0.0.1"; // IP of Project B (Test)
    public int remotePort = 7001;           // Test's listening port
    public int localPort = 7000;            // Main's listening port

    private OSCTransmitter transmitter;
    private OSCReceiver receiver;

    void Start()
    {
        // Setup transmitter → sends to Test
        transmitter = gameObject.AddComponent<OSCTransmitter>();
        transmitter.RemoteHost = remoteHost;
        transmitter.RemotePort = remotePort;

        // Setup receiver → listens for Test's reply
        receiver = gameObject.AddComponent<OSCReceiver>();
        receiver.LocalPort = localPort;

        receiver.Bind("/Test", OnReceiveTest);
        receiver.Bind("/", OnAnyMessage);

        Debug.Log($"[Main] Ready. Listening on {localPort}, sending to {remoteHost}:{remotePort}.");
    }

    void Update()
    {
        SendInitialMessage();
    }

    void SendInitialMessage()
    {
        var message = new OSCMessage("/Main");
        message.AddValue(OSCValue.String("Hi, how are you?"));

        transmitter.Send(message);
        Debug.Log("[Main] → Sent: Hi, how are you?");
    }

    void OnReceiveTest(OSCMessage message)
    {
        Debug.Log($"[Main] ← Received from Test: {message.Address} | {message.Values[0].StringValue}");

        if (message.Values[0].StringValue == "Good, how are you?")
        {
            var reply = new OSCMessage("/Main");
            reply.AddValue(OSCValue.String("Doing just fine thank you."));
            transmitter.Send(reply);
            Debug.Log("[Main] → Sent: Doing just fine thank you.");
        }
    }

    void OnAnyMessage(OSCMessage message)
    {
        Debug.Log($"[Main] ← Catch-All: {message.Address} | {message.Values[0].StringValue}");
    }
}
