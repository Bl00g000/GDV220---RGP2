using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardBase : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public CardData card;

    public GameObject cardVisualsObject;
    public GameObject cardSummonObj;
    public TMP_Text txtName;
    public TMP_Text txtHealth;
    public TMP_Text txtDamage;
    public TMP_Text txtCost;
    public Image sprCardArt;

    bool bCardActivated = false;
    public bool bMarkedForDeath = false;

    // HIDE THESE IN INSPECTOR AFTER DEBUGGING/TESTING
    public int iStartHealth;
    public int iHealth;
    public int iDamage;

    public int iLanePosIndex;

    // Start is called before the first frame update

    // HI ITS NATHAN HERE (say "hi nathan!!") ADDING IN AN EVENT THANKS
    public event Action<float> OnTakeDamage;

    void Start()
    {
        InitializeCardData();
    }

    private void Update()
    {
        // for now update this every frame > later move to takeDamage function
        txtHealth.text = iHealth.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // Teddy was here, everyone say hi teddy!!
            //---
            // Disable activating cards on click if not in prep phase
            if (GameManager.instance.currentState != GameManager.GameState.Game_PrepPhase)
            {
                Debug.Log("Not in prep phase");
                return;
            }

            // Disables activating cards if not enough souls
            if (Player.instance.iSouls < card.cost)
            {
                Debug.Log("Not enough souls.");
                return;
            }

            // Set this card to be the next summoned card (for when lane chosen)
            CardManager.instance.cardToSummon = this;
            //---


            CardManager.instance.UpdateCardPosInHand();
        }
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            CardDetails.instance.SetDetailsValues(card.name, card.cardDescription, card.health.ToString(), card.damage.ToString(), card.cost.ToString(), card.sprite);
        }
    }


        public void OnPointerEnter(PointerEventData eventData)
    {
        // when hovering over card
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // when no longer hovering over card
    }

    public void InitializeCardData()
    {
        iHealth = card.health;
        iStartHealth = card.health;
        iDamage = card.damage;
        bMarkedForDeath = false;

        txtHealth.text = card.health.ToString();
        txtName.text = card.cardName.ToString();
        txtDamage.text = card.damage.ToString();
        txtCost.text = card.cost.ToString();
        sprCardArt.sprite = card.sprite;

        cardSummonObj.GetComponent<SpriteRenderer>().sprite = sprCardArt.sprite;
        cardSummonObj.SetActive(false);
    }

    public void TakeDamage(int _damageAmount)
    {
        OnTakeDamage?.Invoke(_damageAmount);
        iHealth -= _damageAmount;
    }
}
