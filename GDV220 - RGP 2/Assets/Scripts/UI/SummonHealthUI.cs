using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonHealthUI : MonoBehaviour
{
    public CardBase card;
    void Update()
    {
        transform.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = card.GetComponent<CardBase>().iHealth.ToString();
    }
}
