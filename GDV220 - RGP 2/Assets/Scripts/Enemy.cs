using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    //Oh this whole hting should probably be inheritable...

    //public Sprite _enemySprite;
    // public GameObject objEnemyImage;
    public GameObject prefab_EnemyCardVisuals;

    public string sEnemyName = "Pizza Guy";   //Default name

    public string sSpawnQuote = "My Grandpa's restuarant HAD no pathetic Pizzas!";
    public int iMaxHealth = 10;
    public int iCurrentHealth = 0;
    public int iCurrentSouls = 0;

    protected float fBaseSummonChance = 0.4f;
    protected float fSoulSavedScalar = 0.15f;

    public List<CardData> enemyDeck = new List<CardData>();
    public List<int> playableCardPositions = new List<int>();
    public List<int> HighestCostCardPositions = new List<int>();
    protected CardData cardDataToPlay = null;

    public Lane assignedLane = null;

    public GameObject objTurnIndicator;
    public bool bThinking = false;

    public event Action<float> OnTakeDamage;

    // Start is called before the first frame update
    void Start()
    {
        iCurrentHealth = iMaxHealth;

        StartCoroutine(InitialHealthCheck());
    }

    //checks if player is dead
    public bool IsDead()
    {
        if (iCurrentHealth <= 0)
        {
            return true;
        }
        return false;
    }

    //Hides the enemy sprite and canvases
    private void HideEnemy()
    {


        assignedLane.bActiveLane = false;
        assignedLane.HideLanePlane();
        Battlefield.instance.allLanes[assignedLane.iLaneIndex - 4].bActiveLane = false;
        Battlefield.instance.allLanes[assignedLane.iLaneIndex - 4].HideLanePlane();


        {
            var children = GetComponentsInChildren<SpriteRenderer>();

            foreach (var child in children)
            {
                child.enabled = false;
            }

        }
        {
            var children = GetComponentsInChildren<Canvas>();

            foreach (var child in children)
            {
                child.enabled = false;
            }

        }
    }

    //takes damage. Changes UI? Hides sprite
    public void TakeDamage(int _iDamage)
    {
        OnTakeDamage?.Invoke(_iDamage);
        iCurrentHealth -= _iDamage;

        //Enemy lives:
        if (_iDamage > 0.0f)
        {
            StartCoroutine(DamageEffect());
        }

        //Enemy dies:
        if (iCurrentHealth <= 0)
        {
            HideEnemy();
        }
    }

    //Plays the enemy's turn.
    public void PlayTurn()
    {

        bThinking = true;

        //Returns if the enemy is dead or if the enemy has no lane assigned
        //if (iCurrentHealth <= 0) { return; }
        if (!assignedLane)
        {
            Debug.Log("ERROR: no lane assigned to " + sEnemyName);
            bThinking = false;
            objTurnIndicator.gameObject.SetActive(false);
            return;
        }

        objTurnIndicator.gameObject.SetActive(true);

        //Add a soul
        iCurrentSouls++;

        //Start of turn animation?



        StartCoroutine(NPCDecision());
    }

    private bool ChooseToPlayCard()
    {

        // float difficultyModifier = GameManager.instance.iTurnCounter;

        //random range
        float generatedChance = UnityEngine.Random.Range(0.0f, 1.0f);
        // Debug.Log(generatedChance);

        generatedChance -= (((float)iCurrentSouls - 1.0f) * fSoulSavedScalar);
        //Debug.Log("Generated chance with  current soul subtracted" + generatedChance);

        //testing the generated float against the summon chance
        if (generatedChance <= fBaseSummonChance)
        {

            return true;
        }
        else
        {
            Debug.Log("Chose to save up...");
            return false;
        }

    }

    //Selects a card from the enemy's deck to play
    //This is the meaty selection AI part
    private void SelectCard()
    {

        if (enemyDeck.Count == 0)
        {
            Debug.Log("No cards in enemy deck");
            return;
        }
        //Thinking animation/timer script so it isnt all instant

        //int iPlayableCardQuantity = 0;


        int highestCost = 0;
        for (int i = 0; i < enemyDeck.Count - 1; i++)
        {
            if (enemyDeck[i].cost <= iCurrentSouls)
            {
                //adds the position of the playable card in the enemy deck to the array of ints
                playableCardPositions.Add(i);

                //finding the highest cost card in the deck
                if (highestCost < enemyDeck[i].cost)
                {
                    highestCost = enemyDeck[i].cost;
                    HighestCostCardPositions.Add(i);
                }
            }
        }

        //playing the highest cost card if current souls are a lot
        if (highestCost + 1 <= iCurrentSouls)
        {
            //play random card in range
            int randomHighCard = (int)UnityEngine.Random.Range(0, HighestCostCardPositions.Count);
            cardDataToPlay = enemyDeck[randomHighCard];
        }
        else
        {
            //play random card in range
            int randomCard = (int)UnityEngine.Random.Range(0, playableCardPositions.Count);

            cardDataToPlay = enemyDeck[playableCardPositions[randomCard]];
        }

        Debug.Log("Selected this card -> " + cardDataToPlay.name + ". Mana cost of card: " + cardDataToPlay.cost);
        //enemyDeck.;
        playableCardPositions.Clear();
        HighestCostCardPositions.Clear();
    }

    //Plays the chosen card
    private void PlayChosenCard()
    {
        //Instantiate card
        var newCard = Instantiate(prefab_EnemyCardVisuals, CardManager.instance.gameObject.transform);                                     ////PROBLEM IN HERE!!!
        newCard.GetComponent<CardBase>().card = cardDataToPlay;
        newCard.GetComponent<CardBase>().InitializeCardData();

        CardManager.instance.cardToSummon = newCard.GetComponent<CardBase>();
        CardManager.instance.SummonCard(assignedLane.iLaneIndex);

        Debug.Log("Played this card -> " + cardDataToPlay.name + ". Mana cost of card: " + cardDataToPlay.cost);

        assignedLane.SetSummonsLaneIndex();

        iCurrentSouls -= cardDataToPlay.cost;
    }

    //actual representation of me when I think
    IEnumerator NPCDecision()
    {

        Debug.Log(sEnemyName + ": Thinking...");

        yield return new WaitForSeconds(2);

        //the lane is already full
        if (assignedLane.summons[2])
        {
            //Lane is already full...

            //Play animation/Send message
            Debug.Log(sEnemyName + " cannot play cards, their lane is full.");
            bThinking = false;
            objTurnIndicator.gameObject.SetActive(false);
            //increment turns passed in a row
            // iTurnsPassedInARow++;
            yield break;
        }

        //If the enemy chooses to play a card...
        if (ChooseToPlayCard() == true)
        {
            //Select a card
            SelectCard();

            //Catches if there is no card to play
            if (cardDataToPlay)
            {
                // assignedLane.AddSummon(cardToPlay);         /////PLAY CARD IS HERE _ THIS SHOULD CHANGE MAYBE????
                //  iTurnsPassedInARow = 0;

                PlayChosenCard();
                cardDataToPlay = null;
            }
            else
            {
                Debug.Log("Did not choose a card");
            }
        }

        objTurnIndicator.gameObject.SetActive(false);
        bThinking = false;
    }

    IEnumerator DamageEffect()
    {
        SpriteRenderer SR = GetComponentInChildren<SpriteRenderer>();
        SR.color = Color.red;
        yield return new WaitForSeconds(0.65f);

        SR.color = Color.white;
    }

    IEnumerator InitialHealthCheck()
    {
        yield return new WaitForSeconds(0.0f);
        if (iCurrentHealth <= 0)
        {
            HideEnemy();
        }
        else
        {
            Debug.Log(sEnemyName + ": \"" + sSpawnQuote + "\"");
        }
    }
}

