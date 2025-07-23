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

    // Movement mechanics
    [SerializeField] private float _smoothTime = 0.1f;
    private Vector3 velocity = Vector3.zero;

    private NoteID _ID;
    private Vector3 _destination;
    private bool _isMoving = true;

    void Start()
    {        
        _ID = GetComponent<NoteID>();
        
        if (target != null)
        {
            // Calculate the direction from this object to the target
            Vector3 toTarget = (target.position - transform.position).normalized;

            // Set the _destination to 5 units beyond the target in that direction
            _destination = target.position + toTarget * distanceToDespawn;
        }
    }


    void Update()
    {
        // Move
        if (_isMoving)
        {
            transform.position = Vector3.SmoothDamp(transform.position, _destination, ref velocity, _smoothTime);


            // Check if we've reached the _destination
            if (Vector3.Distance(transform.position, _destination) < 0.1f)
            {
                _isMoving = false;
                Done();
            }
        }

        // Track position
        if (target != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            _ID.inSync = distanceToTarget <= 1f;
        }
    }


    public delegate void NoteDone(NoteID note);
    public static event NoteDone OnNoteDone;
    // Note is done the track it was on
    private void Done()
    {
        Debug.Log("Note reached _destination — firing death event.");
        OnNoteDone?.Invoke(_ID);
    }
}
