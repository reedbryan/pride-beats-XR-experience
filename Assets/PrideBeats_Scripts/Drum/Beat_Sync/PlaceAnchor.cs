using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceAnchor : MonoBehaviour
{
    public Transform drumIntersect;

    void Start()
    {
        if (drumIntersect != null)
        {
            // Rotate this object to look at the drumIntersect
            transform.LookAt(drumIntersect);
        }
    }
}
