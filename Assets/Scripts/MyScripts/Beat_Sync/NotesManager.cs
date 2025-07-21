using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotesManager : MonoBehaviour
{
    public GameObject NotePrefab;
    public GameObject NoteSpawningAnchor;
   
    // A sequence of floats representing the pattern in which the notes will appear
    public List<float> noteIntervals = new List<float>();

    // List of all notes CURRENTLY in the scene
    public List<NoteID> CurrentNotes;

    // Number of notes in the noteIntervals list
    private int noteCount;
    private int _noteIndex = 0;

    // Flag to prevent multiple overlapping coroutines
    [SerializeField] private bool isSpawning = false;

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
            float waitTime = noteIntervals[_noteIndex];

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
        Debug.Log("Spawning Note #" + _noteIndex);

        // Spawn the new note, at the anchor position, facing the same as the drum
        GameObject newNote = Instantiate(NotePrefab, 
                                        NoteSpawningAnchor.transform.position, 
                                        NoteSpawningAnchor.GetComponent<PlaceAnchor>().drumIntersect.rotation);

        // Assign movement target
        newNote.GetComponent<NoteMover>().target = NoteSpawningAnchor.GetComponent<PlaceAnchor>().drumIntersect;

        // Add to notes list
        CurrentNotes.Add(newNote.GetComponent<NoteID>());
    }

    // Listen for dead notes - - - - - - - - - - - - - - - - - - - 
    void OnEnable()
    {
        NoteMover.OnNoteDone += KillNote;
    }
    void OnDisable()
    {
        NoteMover.OnNoteDone -= KillNote;
    }
    void KillNote (NoteID note)
    {
        // Remove from the current notes list
        if (CurrentNotes.Contains(note))
        {
            CurrentNotes.Remove(note);
        }

        // Destroy the note GameObject
        Destroy(note.gameObject);
    }
}
