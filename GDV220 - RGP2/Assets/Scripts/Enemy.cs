using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{

    //Oh this whole hting should probably be inheritable...

    //public Sprite _enemySprite;
    public GameObject objEnemyImage;

    public string sEnemyName = "Pizza Guy";   //Default name

    public string sSpawnQuote = "My Grandpa's restuarant HAD no pathetic Pizzas!";
    public int iMaxHealth = 10;
    public int iCurrentHealth = 0;

    public int iCurrentSouls = 0;

    public float fBaseSummonChance = 0.5f;
    public float fSoulSavedScalar = 0.1f;

  //  public int iTurnsPassedInARow = 0;

    public List<CardData> enemyDeck = new List<CardData>();
    public List<int> playableCardPositions = new List<int>();
    public List<int> HighestCostCardPositions = new List<int>();

    public Lane assignedLane = null;

    
    protected CardData cardDataToPlay = null;

    public GameObject objTurnIndicator;
    public bool bThinking = false;

    // Start is called before the first frame update
    void Start()
    {
        iCurrentHealth = iMaxHealth;

        


        Debug.Log(sEnemyName + ": \"" + sSpawnQuote + "\"");


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //checks if player is dead
    public bool IsDead()
    {
        if(iCurrentHealth <= 0)
        {
            return true;
        }
        return false;
    }

    //takes damage. Changes UI? Hides sprite
    public void TakeDamage(int _iDamage)
    {
        iCurrentHealth -= _iDamage;
       // healthUI.text = iCurrentHealth.ToString();
       if(iCurrentHealth <= 0)
       {
            objEnemyImage.SetActive(false);
       }
    }

    //Plays the enemy's turn.
    public void PlayTurn()
    {
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
            return;
        }

        StartCoroutine(NPCDecision());



    }


    private bool ChooseToPlayCard()
    {

       // float difficultyModifier = GameManager.instance.iTurnCounter;

        //random range
        float generatedChance = Random.Range(0.0f, 1.0f);
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

        if(enemyDeck.Count == 0)
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
                if(highestCost < enemyDeck[i].cost)
                {
                    highestCost = enemyDeck[i].cost;
                    HighestCostCardPositions.Add(i);
                }
            }
        }

        //playing the highest cost card if current souls are a lot
        if(highestCost + 1 <= iCurrentSouls)
        {
            //play random card in range
            int randomHighCard = (int)Random.Range(0, HighestCostCardPositions.Count);
            cardDataToPlay = enemyDeck[randomHighCard];
        }
        else
        {
            //play random card in range
            int randomCard = (int)Random.Range(0, playableCardPositions.Count);

            cardDataToPlay = enemyDeck[playableCardPositions[randomCard]];
        }
        
        Debug.Log("Played this card -> " + cardDataToPlay.name + ". Mana cost of card: " + cardDataToPlay.cost);
        //enemyDeck.;
        playableCardPositions.Clear();
        HighestCostCardPositions.Clear();
    }

    //Plays the chosen card
    private void PlayChosenCard()
    {
        //Instantiate card
        var newCard = Instantiate(CardManager.instance.pf_baseCard, CardManager.instance.gameObject.transform);                                     ////PROBLEM IN HERE!!!
        newCard.GetComponent<CardBase>().card = cardDataToPlay;
        newCard.GetComponent<CardBase>().InitializeCardData();
       

        CardManager.instance.cardToSummon = newCard.GetComponent<CardBase>();
        CardManager.instance.SummonCard(assignedLane.iLaneIndex);
        

        assignedLane.SetSummonsLaneIndex();

        iCurrentSouls -= cardDataToPlay.cost;
    }

    //actual representation of me when I think
    IEnumerator NPCDecision()
    {
        Debug.Log(sEnemyName + ": Thinking...");
        
        yield return new WaitForSeconds(2);

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
}
