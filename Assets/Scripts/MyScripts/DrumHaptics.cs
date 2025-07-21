using System.Collections;
using UnityEngine;

public class DrumHaptics : MonoBehaviour
{
    public enum Hand { Left, Right }
    public Hand controllerHand = Hand.Right;

    public float vibrationDuration = 0.1f;
    public float frequency = 1.0f;
    public float amplitude = 0.5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Drum"))
        {
            StartCoroutine(HapticPulse());
        }
    }

    private IEnumerator HapticPulse()
    {
        var controller = controllerHand == Hand.Left ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch;

        // Start vibration
        OVRInput.SetControllerVibration(frequency, amplitude, controller);
        yield return new WaitForSeconds(vibrationDuration);
        OVRInput.SetControllerVibration(0, 0, controller); // Stop vibration
    }
}
