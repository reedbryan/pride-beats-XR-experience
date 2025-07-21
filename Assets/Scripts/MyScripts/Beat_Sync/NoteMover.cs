using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteMover : MonoBehaviour
{
    public float speed = 6f;

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}
