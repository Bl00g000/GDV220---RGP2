using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardDetails : MonoBehaviour
{
    public static CardDetails instance;

    [Header("Card Details")]
    public TMP_Text txtName;
    public TMP_Text txtDescription;
    public TMP_Text txtHealth;
    public TMP_Text txtDamage;
    public TMP_Text txtCost;
    public TMP_Text txtEffect;
    public Image sprCardArt;

    [Header("UI")]
    public float UIScale;
    public GameObject cardUI;

    public List<GameObject> effectToolTips;

    public bool itIsActive = false;



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

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            CloseScreen();
        }
        transform.localScale = Vector3.one * UIScale;
    }

    public void CloseScreen()
    {
        cardUI.SetActive(false);
        itIsActive = false;
    }

    public void SetDetailsValues(CardData cardData)
    {
        cardUI.SetActive(true);

        txtName.text = cardData.cardName;
        txtDescription.text = cardData.cardDescription;
        txtHealth.text = cardData.health.ToString();
        txtDamage.text = cardData.damage.ToString();
        txtCost.text = cardData.cost.ToString();
        txtEffect.text = cardData.cardEffect.ToString();
        sprCardArt.sprite = cardData.sprite;

        cardUI.GetComponentInChildren<CardBase>().card = cardData;

        foreach(GameObject effect in effectToolTips)
        {
            effect.SetActive(false);
        }
        effectToolTips[(int)cardData.cardEffect].SetActive(true);
        if (cardData.cardEffect == CardData.Effects.None) txtEffect.text = string.Empty;

        itIsActive = true;
    }
}
