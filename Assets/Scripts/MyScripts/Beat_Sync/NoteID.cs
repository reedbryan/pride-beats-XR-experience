using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteID : MonoBehaviour
{
    // True if this note is correctly placed over the drum
    // - accessed by drumstick to confirm an in sync drumhit
    public bool inSync;
}
