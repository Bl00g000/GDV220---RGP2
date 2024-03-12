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
    public TMP_Text txtDescription;
    public TMP_Text txtHealth;
    public TMP_Text txtDamage;
    public TMP_Text txtCost;
    public TMP_Text txtEffect;
    public Image sprCardArt;

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
            // Disable activating cards on click 
            // if not in prep phase or not enough souls
            if (GameManager.instance.bTurnFinished || Player.instance.iSouls < card.cost)
            {
                return;
            }

            // Set this card to be the next summoned card (for when lane chosen)
            CardManager.instance.cardToSummon = this;
        }
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            CardDetails.instance.SetDetailsValues(card);
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

    public void SetTurnIndicator(bool _isOn)
    {
        foreach (Transform child in cardSummonObj.gameObject.transform)
        {
            if (child.CompareTag("TurnIndicator"))
            {
                child.gameObject.SetActive(_isOn);
            }
        }
    }

    public void InitializeCardData()
    {
        iHealth = card.health;
        iStartHealth = card.health;
        iDamage = card.damage;
        bMarkedForDeath = false;

        txtHealth.text = card.health.ToString();
        txtName.text = card.cardName.ToString();
        txtDescription.text = card.cardDescription.ToString();
        txtDamage.text = card.damage.ToString();
        txtCost.text = card.cost.ToString();
        txtEffect.text = card.cardEffect.ToString();
        sprCardArt.sprite = card.sprite;

        cardSummonObj.GetComponent<Animator>().runtimeAnimatorController = card.animController;
        if(card.cardEffect == CardData.Effects.None) txtEffect.text = string.Empty;
    }

    public void TakeDamage(int _damageAmount)
    {
        OnTakeDamage?.Invoke(_damageAmount);
        iHealth -= _damageAmount;
    }
}
