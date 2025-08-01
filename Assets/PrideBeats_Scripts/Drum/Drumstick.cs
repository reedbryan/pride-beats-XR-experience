using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Drumstick : MonoBehaviour
{
    [Tooltip("How long the drum stays red (seconds).")]
    public float flashDuration = 0.12f;

    [Tooltip("Cooldown between hits (seconds).")]
    public float cooldownDuration = 0.2f;

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

    private bool canHit = true;

    void OnTriggerEnter(Collider other)
    {
        if (!canHit) return;

        // Only react to objects tagged "Drum"
        if (!other.CompareTag("Drum")) return;

        canHit = false; // start cooldown
        StartCoroutine(HitCooldown());

        // Get OSC component
        if (_OSCtransmitter == null)
            _OSCtransmitter = other.gameObject.GetComponentInChildren<QuestOSCClient>();

        // check for beat sync
        if (notesManager.CheckForNoteInSync())
        {
            other.gameObject.GetComponent<DrumEffects>().DrumHitInSync();
            _OSCtransmitter.SendOSCMessage("/DrumHit", "Drum hit in sync");
        }
        else
        {
            other.gameObject.GetComponent<DrumEffects>().DrumHitOutSync();
            _OSCtransmitter.SendOSCMessage("/DrumHit", "Drum hit out of sync");
        }
    }

    IEnumerator HitCooldown()
    {
        yield return new WaitForSeconds(cooldownDuration);
        canHit = true;
    }
}
