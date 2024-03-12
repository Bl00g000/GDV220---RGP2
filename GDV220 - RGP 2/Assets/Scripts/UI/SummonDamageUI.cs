using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonDamageUI : MonoBehaviour
{
    public CardBase card;
    // Update is called once per frame
    void Update()
    {
        transform.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = card.GetComponent<CardBase>().iDamage.ToString();
    }
}
