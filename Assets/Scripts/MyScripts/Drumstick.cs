using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Drumstick : MonoBehaviour
{
    [Tooltip("How long the drum stays red (seconds).")]
    public float flashDuration = 0.12f;

    [Tooltip("Drum GameObject that will flash and hold the particle system.")]
    public GameObject drum;

    private Renderer   drumRenderer;
    private Color      _originalColor;
    private ParticleSystem hitParticles;

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
        hitParticles = drum.GetComponentInChildren<ParticleSystem>();
        if (hitParticles == null)
        {
            Debug.LogWarning("Drumstick: No ParticleSystem found under the drum. Particle burst will be skipped.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Only react to objects tagged "Drum"
        if (!other.CompareTag("Drum")) return;

        StartCoroutine(FlashDrum());
    }

    IEnumerator FlashDrum()
    {
        // Colour + scale feedback
        drumRenderer.material.color = Color.red;
        transform.localScale *= 1.1f;

        // One‑shot particle burst
        if (hitParticles != null)
        {
            hitParticles.Emit(1);   // single particle
        }

        yield return new WaitForSeconds(flashDuration);

        drumRenderer.material.color = _originalColor;
        transform.localScale /= 1.1f;
    }
}
