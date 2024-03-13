using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SummonObjOnClick : MonoBehaviour
{
    private CardBase cardBase;
    private Collider2D boxCollider;
    private bool bClicked = false;

    private void Start()
    {
        cardBase = transform.GetComponentInParent<CardBase>();
        boxCollider = transform.GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);
        
            if (hit.collider != null && hit.collider.transform == this.transform)
            {
                CardDetails.instance.SetDetailsValues(cardBase.card);
            }
        }
    }
}
