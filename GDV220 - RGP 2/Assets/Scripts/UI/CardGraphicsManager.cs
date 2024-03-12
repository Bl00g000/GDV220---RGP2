using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGraphicsManager : MonoBehaviour
{
    public static CardGraphicsManager instance;
    // Start is called before the first frame update
    [System.Serializable]
    public class CardRarityBackground
    {
        public CardData.Rarities rarity;
        public Sprite sprite;
        public Material material;
    }

    [SerializeField] private List<CardRarityBackground> cardRarityBackgrounds;

    public Dictionary<CardData.Rarities, CardRarityBackground> cardRarityDict = new Dictionary<CardData.Rarities, CardRarityBackground>();

    private void Awake()
    {
        instance = this;
        foreach(CardRarityBackground cardRarity in cardRarityBackgrounds)
        {
            cardRarityDict.Add(cardRarity.rarity, cardRarity);
        }
    }
}
