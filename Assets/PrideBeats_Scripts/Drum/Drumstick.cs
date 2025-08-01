using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Drumstick : MonoBehaviour
{
    [Tooltip("How long the drum stays red (seconds).")]
    public float flashDuration = 0.12f;

    [Tooltip("Drum GameObject that will flash and hold the particle system.")]
    public GameObject drum;

    public NotesManager notesManager;

    private Color _originalColor;
    private ParticleSystem red;
    private ParticleSystem orange;
    private ParticleSystem yellow;
    private ParticleSystem green;
    private ParticleSystem blue;
    private ParticleSystem purple;

    private DrumEffects _drumEffects;
    private QuestOSCClient _OSCtransmitter;


    void Update()
    {
        // Debug.Log(CheckForNoteInSync());
    }

    void OnTriggerEnter(Collider other)
    {
        // Only react to objects tagged "Drum"
        if (!other.CompareTag("Drum")) return;

        // Get OSC component
        if (_OSCtransmitter == null) _OSCtransmitter = other.gameObject.GetComponentInChildren<QuestOSCClient>();

        // check for beat sync
        if (notesManager.CheckForNoteInSync())
        {
            //StartCoroutine(FlashDrum());
            other.gameObject.GetComponent<DrumEffects>().DrumHitInSync();
            _OSCtransmitter.SendOSCMessage("/DrumHit", "Drum hit in sync");
        }
        else
        {
            other.gameObject.GetComponent<DrumEffects>().DrumHitOutSync();
            _OSCtransmitter.SendOSCMessage("/DrumHit", "Drum hit out of sync");
        }
    }
}