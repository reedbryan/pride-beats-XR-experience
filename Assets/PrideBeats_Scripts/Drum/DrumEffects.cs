using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrumEffects : MonoBehaviour
{
    [Tooltip("How long the drum stays red (seconds).")]
    public float flashDuration = 0.12f;
    
    private AudioSource _hitAudio;
    private Renderer   _drumRenderer;
    private Color      _originalColor;

    [SerializeField] private ParticleSystem red;
    [SerializeField] private ParticleSystem orange;
    [SerializeField] private ParticleSystem yellow;
    [SerializeField] private ParticleSystem green;
    [SerializeField] private ParticleSystem blue;
    [SerializeField] private ParticleSystem purple;

    void Awake()
    {
        _drumRenderer = GetComponent<Renderer>();
        _hitAudio = GetComponentInChildren<AudioSource>();

        _originalColor = _drumRenderer.material.color;
    }

    // Event listening
    void OnEnable()
    {
        Drumstick.OnDrumHit += TriggerEffects;
    }
    void OnDisable()
    {
        Drumstick.OnDrumHit -= TriggerEffects;
    }
    public void TriggerEffects(bool inSync)
    {
        if (inSync) {StartCoroutine(InSyncEffects()); }
        else { StartCoroutine(OutSyncEffects()); }
    }

    IEnumerator InSyncEffects()
    {
        // One‑shot particle burst
        red?.Emit(1);
        orange?.Emit(1);
        yellow?.Emit(1);
        green?.Emit(1);
        blue?.Emit(1);
        purple?.Emit(1);

        _hitAudio.Play();

        yield return new WaitForSeconds(flashDuration);
    }

    IEnumerator OutSyncEffects()
    {
        red?.Emit(1);
        orange?.Emit(1);
        yellow?.Emit(1);

        _hitAudio.Play();
        
        yield return new WaitForSeconds(flashDuration);
    }
}
