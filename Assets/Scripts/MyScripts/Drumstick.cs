using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Drumstick : MonoBehaviour
{
    [Tooltip("How long the drum stays red (seconds).")]
    public float flashDuration = 0.12f;

    [Tooltip("Drum GameObject that will flash and hold the particle system.")]
    public GameObject drum;

    private Renderer drumRenderer;
    private Color _originalColor;
    private ParticleSystem red;
    private ParticleSystem orange;
    private ParticleSystem yellow;
    private ParticleSystem green;
    private ParticleSystem blue;
    private ParticleSystem purple;

    private DrumEffects _drumEffects;
    private QuestOSCClient _OSCtransmitter;


    void Awake()
    {
        // ---- Renderer ----------------------------------------------------
        drumRenderer = drum.GetComponent<Renderer>() ?? drum.GetComponentInChildren<Renderer>();
        if (drumRenderer == null)
        {
            Debug.LogError("Drumstick: No Renderer found on the drum or its children.");
            enabled = false;
            return;
        }
        _originalColor = drumRenderer.material.color;

        // ---- Particle System --------------------------------------------
        red = drum.transform.Find("Red").GetComponent<ParticleSystem>();
        orange = drum.transform.Find("Orange").GetComponent<ParticleSystem>();
        yellow = drum.transform.Find("Yellow").GetComponent<ParticleSystem>();
        green = drum.transform.Find("Green").GetComponent<ParticleSystem>();
        blue = drum.transform.Find("Blue").GetComponent<ParticleSystem>();
        purple = drum.transform.Find("Purple").GetComponent<ParticleSystem>();
        if (red == null || orange == null || yellow == null || green == null || blue == null || purple == null)
        {
            Debug.LogWarning("Drumstick: One or more ParticleSystems not found under the drum.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Only react to objects tagged "Drum"
        if (!other.CompareTag("Drum")) return;

        // Get OSC component
        if (_OSCtransmitter == null) _OSCtransmitter = other.gameObject.GetComponentInChildren<QuestOSCClient>();

        // check for beat sync
        if (IsAnyCylinderNoteNearby())
        {
            StartCoroutine(FlashDrum());
            _OSCtransmitter.SendOSCMessage("/DrumHit", "Drum hit in sync");
        }
        else
        {
            _OSCtransmitter.SendOSCMessage("/DrumHit", "Drum hit out of sync");
        }
    }

    bool IsAnyCylinderNoteNearby()
    {
        // Get all active Notes prefabs in the scene
        GameObject[] notesPrefabs = GameObject.FindGameObjectsWithTag("Notes");

        foreach (GameObject notes in notesPrefabs)
        {
            // Check each child of the Notes object
            foreach (Transform child in notes.transform)
            {
                float distance = Vector3.Distance(child.position, drum.transform.position);
                if (distance <= 1f)
                {
                    return true;
                }
            }
        }

        return false;
    }

    IEnumerator FlashDrum()
    {
        // Colour + scale feedback
        drumRenderer.material.color = Color.red;

        // One‑shot particle burst
        red?.Emit(1);
        orange?.Emit(1);
        yellow?.Emit(1);
        green?.Emit(1);
        blue?.Emit(1);
        purple?.Emit(1);

        yield return new WaitForSeconds(flashDuration);

        drumRenderer.material.color = _originalColor;
    }
}