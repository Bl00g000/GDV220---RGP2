using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaneOnClick : MonoBehaviour
{
    Lane lane;

    private void Start()
    {
        lane = GetComponent<Lane>();
    }

    void OnMouseDown()
    {
        lane.LanePicked();
    }
}
