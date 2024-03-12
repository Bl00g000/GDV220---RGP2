using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SummonObjOnClick : MonoBehaviour
{
    private CardBase cardBase;

    private void Start()
    {
        cardBase = transform.GetComponentInParent<CardBase>();
    }

    private void Update()
    {
        // check if right clicking on summon obj
        if (Input.GetMouseButtonDown(1))
        {
            // Teddy was here say WATER YOU DOIN TEDDLES?!?!
            CardDetails.instance.SetDetailsValues(cardBase.card);
        }
    }
}
