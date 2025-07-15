using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DrumstickHit : MonoBehaviour
{
    [Tooltip("How long the drum stays red (seconds).")]
    public float flashDuration = 0.12f;

    private color _originalColor;

    void Awake()
    {
        // Grab the first renderer on the drum object (or its children)
        var renderer = drumCol.GetComponent<Renderer>() ?? drumCol.GetComponentInChildren<Renderer>();
        if (renderer == null) yield break;

        _originalColor = renderer.material.color;
    }

    void OnTriggerEnter(Collider other)
    {
        // Only react to objects tagged "drum"
        if (!other.CompareTag("Drum")) return;

        StartCoroutine(FlashDrum(other));
    }

    IEnumerator FlashDrum(Collider drumCol)
    {
        renderer.material.color = Color.red;
        transform.localScale *= 1.1f;

        yield return new WaitForSeconds(flashDuration);

        renderer.material.color = _originalColor;
        transform.localScale /= 1.1f;
    }
}
