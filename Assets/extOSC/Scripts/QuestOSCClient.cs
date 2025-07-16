using UnityEngine;
using extOSC;

public class QuestOSCClient : MonoBehaviour
{
    [Header("OSC Settings")]
    [Tooltip("The port this Quest device will listen on.")]
    public int listenPort = 8100;

    public GameObject testPrefab;

    private OSCReceiver receiver;

    void Start()
    {
        // Create and configure the OSC receiver
        receiver = gameObject.AddComponent<OSCReceiver>();
        receiver.LocalPort = listenPort;

        // Bind a handler for the /StartGame address
        receiver.Bind("/StartGame", OnStartGame);

        // Optional: catch-all to see any message
        receiver.Bind("/", OnAnyMessage);

        Debug.Log($"[QuestOSCReceiver] Listening for OSC on port {listenPort}...");
    }

    void OnStartGame(OSCMessage message)
    {
        Debug.Log("[QuestOSCReceiver] Received /StartGame message!");

        Instantiate(testPrefab);

        // Example action: start game logic
        // You can replace this with your own functionality
        Debug.Log("[QuestOSCReceiver] Starting game now!");
    }

    void OnAnyMessage(OSCMessage message)
    {
        // Useful for debugging unexpected messages
        Debug.Log($"[QuestOSCReceiver] Message received: {message.Address} | {message.Values[0]}");
    }
}
