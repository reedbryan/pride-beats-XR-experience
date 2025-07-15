using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Drumstick : MonoBehaviour
{
    private DrumEffects _drumEffects;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Drum")) return;

        if (_drumEffects == null) _drumEffects = other.gameObject.GetComponent<DrumEffects>();

        StartCoroutine(_drumEffects.FlashDrum());
    }
}
