using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteEffects : MonoBehaviour
{
    private NoteID _ID;

    [Tooltip("Death Effect Particles")]
    public GameObject deathPSPrefab;

    void Awake()
    {
        _ID = GetComponent<NoteID>();

        //Invoke("GotHit", 3f); // Example timing
    }

    public void StartDeathEffects()
    {
        if (deathPSPrefab != null)
        {
            GameObject psInstance = Instantiate(deathPSPrefab, transform.position, Quaternion.identity);
            psInstance.transform.eulerAngles = new Vector3(-90, 0, 0);
        }
    }
}
