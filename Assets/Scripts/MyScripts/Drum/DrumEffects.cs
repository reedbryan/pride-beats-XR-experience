using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrumEffects : MonoBehaviour
{
    [Tooltip("How long the drum stays red (seconds).")]
    public float flashDuration = 0.12f;
    
    private Renderer   _drumRenderer;
    private Color      _originalColor;
    private ParticleSystem _hitParticles;

    void Awake()
    {
        _drumRenderer = GetComponent<Renderer>();
        _hitParticles = GetComponentInChildren<ParticleSystem>();
        _originalColor = _drumRenderer.material.color;
    }
    
    public IEnumerator FlashDrum()
    {
        // Colour + scale feedback
        _drumRenderer.material.color = Color.red;
        transform.localScale *= 1.1f;

        // One‑shot particle burst
        if (_hitParticles != null)
        {
            _hitParticles.Emit(1);   // single particle
        }

        yield return new WaitForSeconds(flashDuration);

        _drumRenderer.material.color = _originalColor;
        transform.localScale /= 1.1f;
    }
}
