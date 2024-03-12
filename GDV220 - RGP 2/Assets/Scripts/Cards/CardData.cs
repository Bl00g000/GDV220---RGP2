using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "New Card")]
public class CardData : ScriptableObject
{
    public string cardName;
    public string cardDescription;
    public Effects cardEffect;
    public Rarities rarity;
    public Sprite sprite;

    public RuntimeAnimatorController animController;

    public int health;
    public int damage;
    public int cost;

    public enum Rarities
    {
        Common,
        Rare,
        Mythic
    }

    public enum Effects
    {
        None,
        Battlecry,
        Slayer,
        Rampage,
        Rally
    }

    public Attack attack;
}
