using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "New Card")]
public class CardData : ScriptableObject
{
    public string cardName;
    public string cardDescription;
    public Rarities rarity;
    public Sprite sprite;

    public int health;
    public int damage;
    public int cost;

    public enum Rarities
    {
        Common,
        Rare,
        Mythic
    }

    public Attack attack;
}
