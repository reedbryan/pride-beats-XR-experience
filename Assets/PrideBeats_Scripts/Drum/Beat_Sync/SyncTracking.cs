using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncTracking : MonoBehaviour
{
    private int _streak;
    public int streak
    {
        get { return _streak; }
        private set { _streak = value; }
    }


    public void SyncUpdate(bool inSync)
    {
        if (inSync) _streak++;
        else _streak = 0;
    }
}
