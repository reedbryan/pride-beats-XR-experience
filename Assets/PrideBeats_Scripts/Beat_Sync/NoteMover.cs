using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteMover : MonoBehaviour
{
    // target movement position
    public Transform target;

    // Movement speed
    public float speed = 6f;

    // The distance after the note passes over the drum before it despawns
    public float distanceToDespawn;

    private NoteID ID;
    private Vector3 destination;
    private bool isMoving = true;

    void Start()
    {
        ID = GetComponent<NoteID>();
        
        if (target != null)
        {
            // Calculate the direction from this object to the target
            Vector3 toTarget = (target.position - transform.position).normalized;

            // Set the destination to 5 units beyond the target in that direction
            destination = target.position + toTarget * distanceToDespawn;
        }
    }

    void Update()
    {
        // Move
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                destination,
                speed * Time.deltaTime
            );

            // Check if we've reached the destination
            if (Vector3.Distance(transform.position, destination) < 0.01f)
            {
                isMoving = false;
                Done();
            }
        }

        // Track position
        if (target != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            ID.inSync = distanceToTarget <= 1f;
        }
    }


    public delegate void NoteDone(NoteID note);
    public static event NoteDone OnNoteDone;
    // Note is done the track it was on
    private void Done()
    {
        Debug.Log("Note reached destination — firing death event.");
        OnNoteDone?.Invoke(ID);
    }
}
