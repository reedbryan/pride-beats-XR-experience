using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotesManager : MonoBehaviour
{
    public GameObject NotePrefab;
    public GameObject NoteSpawningAnchor;
   
    // A sequence of integers representing the pattern in which the notes will appear
    public List<int> noteIntervals = new List<int>();

    // Number of notes in the noteIntervals list
    public int noteCount;
    private int _noteIndex = 0;

    // Flag to prevent multiple overlapping coroutines
    private bool isSpawning = false;

    void Awake()
    {
        noteCount = noteIntervals.Count;
    }

    // Start the routine which spawns the notes, based on the noteIntervals list
    // - is activated via OSC, via the PC build, which transmits to all headsets
    public void startGame()
    {
        if (!isSpawning)
        {
            _noteIndex = 0;  // Reset index for replayability
            StartCoroutine(SpawnNotes());
        }
    }

    IEnumerator SpawnNotes()
    {
        isSpawning = true;

        // Loop through each note interval and wait accordingly
        while (_noteIndex < noteCount)
        {
            // TODO: spawn the note here
            SpawnNote();

            // Get the interval of the current note
            int waitTime = noteIntervals[_noteIndex];

            // Wait for the specified time before spawning the next note
            yield return new WaitForSeconds(waitTime);

            // Move to the next note in the sequence
            _noteIndex++;
        }

        // All notes have been spawned — end routine
        isSpawning = false;
    }

    private void SpawnNote()
    {
        Instantiate(NotePrefab, NoteSpawningAnchor.transform.position, NoteSpawningAnchor.transform.rotation);
    }
}
