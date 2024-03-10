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
    public Image sprCardArt;

    [Header("UI")]
    public float UIScale;
    public GameObject cardUI;

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
    }
    public void SetDetailsValues(string _name, string _description, string _health, string _damage, string _cost, Sprite _cardArt)
    {
        cardUI.SetActive(true);

        txtName.text = _name;
        txtDescription.text = _description;
        txtHealth.text = _health;
        txtDamage.text = _damage;
        txtCost.text = _cost;

        sprCardArt.sprite = _cardArt;
    }
}
