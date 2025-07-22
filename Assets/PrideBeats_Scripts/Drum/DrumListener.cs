using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrumListener : MonoBehaviour
{
    private QuestOSCClient OSCClient;

    public GameObject SpatialAnchorPrefab;

    void Awake()
    {
        OSCClient = GetComponent<QuestOSCClient>();
    }
    
    public void DrumHit()
    {
        //OSCClient.SendPlayerAction("DRUM HIT");
        Instantiate(SpatialAnchorPrefab);
    }
}
