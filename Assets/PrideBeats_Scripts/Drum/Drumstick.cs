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

    private bool canHit = true;

    // Declare a global event for drum hits
    public delegate void DrumHitEvent(bool inSync);
    public static event DrumHitEvent OnDrumHit;

    void OnTriggerEnter(Collider other)
    {
        if (!canHit) return;
        if (!other.CompareTag("Drum")) return;

        canHit = false;
        StartCoroutine(HitCooldown());

        bool inSync = notesManager.CheckForNoteInSync();

        // Fire the event instead of calling other scripts directly
        OnDrumHit?.Invoke(inSync);

        Debug.Log($"[Drumstick] Drum hit. In sync? {inSync}");
    }

    IEnumerator HitCooldown()
    {
        yield return new WaitForSeconds(cooldownDuration);
        canHit = true;
    }
}
