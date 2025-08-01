using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteEffects : MonoBehaviour
{
    private NoteID _ID;

    [Tooltip("Assign a ParticleSystem prefab here (not a child object).")]
    public GameObject deathPSPrefab; // Prefab, not an instance

    void Awake()
    {
        _ID = GetComponent<NoteID>();

        //Invoke("GotHit", 3f); // Example timing
    }

    public delegate void NoteDone(NoteID note);
    public static event NoteDone OnNoteDone;

    public void DoneTrack()
    {
        Debug.Log("Note reached _destination — firing death event.");
        OnNoteDone?.Invoke(_ID);
    }

    public void GotHit()
    {
        Debug.Log("Got hit — firing death event.");
        OnNoteDone?.Invoke(_ID);

        if (deathPSPrefab != null)
        {
            GameObject psInstance = Instantiate(deathPSPrefab, transform.position, Quaternion.identity);
            psInstance.transform.eulerAngles = new Vector3(-90, 0, 0);
        }

        Destroy(gameObject); // Remove the note object
    }
}
