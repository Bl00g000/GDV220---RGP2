using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public static CardManager instance;

    // Teddy was here (hi teddy :3)
    public List<CardData> deck;
    public List<CardData> discardPile;
    public GameObject pf_baseCard;

    // Teddy was here againnn oopies
    [HideInInspector] public CardBase cardToSummon;
    public List<CardBase> hand = new List<CardBase>();
    public List<CardBase> onField = new List<CardBase>();

    public int iMaxHandSize;
    public List<Transform> handPositions;
	public AudioClip soundClip;
    AudioSource audioSource;
  
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        Battlefield.instance.onSummonInLane += SummonCard;
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = soundClip;

        iMaxHandSize = handPositions.Count;
        ShuffleDeck();
        //StartCoroutine(DrawStartingHand());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            DrawCard();
        }
    }

    public void DiscardHand()
    {
        while (hand.Count > 0)
        {
            DiscardCard(hand[0]);
        }
    }

    public void UpdateCardPosInHand()
    {
        for (int i = 0; i < hand.Count; i++)
        {
            if (hand[i] == null) { continue; }
            CardBase card = hand[i];
            if (i < handPositions.Count)
            {
                card.gameObject.transform.position = handPositions[i].position;
                card.gameObject.transform.rotation = handPositions[i].rotation;
            }
        }
    }

    public void DrawCard()
    {
        if (deck.Count >= 1)
        {
            var newCard = Instantiate(pf_baseCard, this.gameObject.transform);
            newCard.GetComponent<CardBase>().card = deck[0];
            deck.RemoveAt(0);

            CardBase drawnCard = newCard.GetComponent<CardBase>();

            AddCardToHand(drawnCard);
        }
        else
        {
            ShuffleDiscardToDeck();
            DrawCard();
        }

        UpdateCardPosInHand();
    }

    // Draw full hand at start of game
    public IEnumerator DrawStartingHand()
    {
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < iMaxHandSize; i++)
        {
            DrawCard();

            yield return new WaitForSeconds(0.5f);
        }

        yield return null;
    }

    // Takes a card and puts it into the players hand
    public void AddCardToHand(CardBase card)
    {
        if (hand.Count < iMaxHandSize)
        {
            hand.Add(card);
        }
        else
        {
            // TODO: discuss if we should just not draw a card or force the player to discard a card to make space for new card
            DiscardCard(hand[0]);
            hand.Add(card);
        }
    }

    public void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            CardData temp = deck[i];
            int randomIndex = UnityEngine.Random.Range(i, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    // Shuffles the cards in the discard pile back into the deck
    public void ShuffleDiscardToDeck()
    {
        deck = new List<CardData>(discardPile);
        discardPile.Clear();
        ShuffleDeck();
    }

    public void DiscardCard(CardBase card)
    {
        discardPile.Add(card.card);
        hand.Remove(card);
        Destroy(card.gameObject);
    }

    public void SummonCard(int _iLaneIndex)
    {
        // If it's the player's turn, tops player from summoning cards in enemy lane
        // Player lanes are index 1-3
        if (_iLaneIndex > 3 && !GameManager.instance.bTurnFinished)
        {
            Debug.Log("Returned at line 143 CardManager");
            return;
        }
        
        GameObject objTargetLane = Battlefield.instance.allLanes[_iLaneIndex - 1].gameObject;

        // Check if lane is full
        if (!objTargetLane.GetComponent<Lane>().HasSpace())
        {
            Debug.Log("Returned at line 153 CardManager");
            return;
        }

        // Remove card from hand and add to field
        if (_iLaneIndex < 4)
        {
            hand.Remove(cardToSummon);
            onField.Add(cardToSummon);
        }
        
        // Disable all children to make card invisible
        foreach (Transform child in cardToSummon.transform)
        {
            child.gameObject.SetActive(false);
        }
        UpdateCardPosInHand();

        // Set summon obj as child of selected lane
        cardToSummon.transform.SetParent(objTargetLane.transform, true);
        cardToSummon.transform.localScale = Vector3.one;

        //HI GEORGE WAS HERE FLIPPING THE SPRITE
        //cardToSummon.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);

        // Summon object
        objTargetLane.GetComponent<Lane>().AddSummon(cardToSummon.cardSummonObj);

        Battlefield.instance.allLanes[_iLaneIndex - 1].AdvanceRow();

        // Bloo was here for combat stuff, say hi bloo :3
        // Resolve On player effects?

        cardToSummon.card.attack.ActivateEffect(cardToSummon.iLanePosIndex, _iLaneIndex - 1,
            Battlefield.instance.allyLanes, Battlefield.instance.enemyLanes, Attack.effectType.ON_PLAY);

        // TO TEDDY: Die here if you have 0 health please and thank you <<< 
            // Teddy was here (say "DIE TEDDY!") >>> all done you are welcome
        Battlefield.instance.CheckLanesForDeath();

        cardToSummon.cardSummonObj.SetActive(true);
        cardToSummon.SetTurnIndicator(false);
		audioSource.Play();

        cardToSummon = null;
    }
}