using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CardBase))]
public class CardGraphics : MonoBehaviour
{
    private CardBase cardBase;
    private CardData cardData;

    public Image cardFrame;

    void Start()
    {
        cardBase = GetComponent<CardBase>();
        cardData = cardBase.card;

        if (cardFrame == null)
        {
            cardFrame = transform.Find("CardFrame").GetComponent<Image>();
        }

        AssignCardRarity();
    }
    public void AssignCardRarity()
    {
        if (!cardData) { Debug.LogError("Card data not found PLEASE FIX!!!"); return; }
        if (!CardGraphicsManager.instance) { Debug.LogError("card graphics manager is NOT PRESENT, GO TO nathaniel hunters scene and GET IT NOW!!!"); return; }

        CardGraphicsManager.CardRarityBackground cardRarityBG = CardGraphicsManager.instance.cardRarityDict[cardData.rarity];
        Debug.Log(cardData.rarity);
        cardFrame.sprite = cardRarityBG.sprite;
        cardFrame.material = cardRarityBG.material;
    }
}
