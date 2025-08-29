using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using extOSC;

public class GameManager : MonoBehaviour
{
    private enum GameState
    {
        Waiting,
        Joined,
        Calibrating,
        Calibrated,
        GameOn
    }

    [Header("Current Game State")]
    [Tooltip("Waiting: No signal / Joined: Waiting for calibration / Calibrated: Ready / Game On: Notes falling")]
    [SerializeField] private GameState currentState = GameState.Waiting;

    private GameState CurrentState => currentState; // read-only public accessor

    private int calibrationCounter = 0;
    private float countDown;

    [SerializeField] QuestOSCClient OSC;
    [SerializeField] NotesManager notesManager;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI statusText;

    public void JoinGame()
    {
        currentState = GameState.Joined;
        OSC.SendOSCMessage("/DrumHit", $"{IP.LocalIPAddress}");
    }

    public void Calibrate()
    {
        currentState = GameState.Calibrating;
        statusText.text = "Hit Your Drum 3 Times\n";
    }

    public void StartGame(OSCMessage message)
    {
        currentState = GameState.GameOn;

        // Extract floats (sequence) from the OSC message
        List<float> receivedSequence = new List<float>();

        int index = 0;
        foreach (var val in message.Values)
        {
            // get the countdown
            if (index == 0)
            {
                countDown = val.FloatValue;
            }
            // get the sequence
            else if (val.Type == OSCValueType.Float)
            {
                receivedSequence.Add(val.FloatValue);
            }

            index++;
        }

        if (receivedSequence.Count > 0)
        {
            Debug.Log("[Quest] Received startGame sequence with " + receivedSequence.Count + " intervals");
            StartCoroutine(StartGameAfterDelay(receivedSequence, countDown));
        }
        else
        {
            Debug.LogWarning("[Quest] No sequence data received in /StartGame message.");
        }
    }
    private IEnumerator StartGameAfterDelay(List<float> sequence, float delay)
    {
        // Show countdown numbers each second
        if (statusText != null)
        {
            statusText.gameObject.SetActive(true);
            for (int i = Mathf.CeilToInt(delay); i > 0; i--)
            {
                statusText.text = i.ToString();
                yield return new WaitForSeconds(1f);
            }

            statusText.text = "GO!";
            yield return new WaitForSeconds(0.5f);
            statusText.gameObject.SetActive(false);
        }
        else
        {
            // fallback wait if TMP isn't assigned
            yield return new WaitForSeconds(delay);
        }

        notesManager.startGame(sequence);
    }

    public void EndGame()
    {
        currentState = GameState.Joined;
        statusText.text = "";
    }

    void OnEnable() { Drumstick.OnDrumHit += HandleDrumHit; }
    void OnDisable() { Drumstick.OnDrumHit -= HandleDrumHit; }

    void HandleDrumHit(bool inSync)
    {
        OSC.SendOSCMessage("/DrumHit", inSync ? "insync" : "outsync");

        if (currentState == GameState.Calibrating)
        {
            calibrationCounter++;
            statusText.text += "0";
            if (calibrationCounter >= 3)
            {
                // CALIBRATED
                currentState = GameState.Calibrated;
            }
        }
    }
}
