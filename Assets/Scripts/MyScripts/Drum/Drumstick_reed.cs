using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Drumstick_reed : MonoBehaviour
{
    private DrumEffects _drumEffects;
    private DrumListener _drumListener;


    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Drum")) return;

        if (_drumEffects == null) _drumEffects = other.gameObject.GetComponent<DrumEffects>();
        if (_drumListener == null) _drumListener = other.gameObject.GetComponent<DrumListener>();

        _drumListener.DrumHit();
        StartCoroutine(_drumEffects.FlashDrum());
    }
}
