using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;

    [Header("Player Stats")]
    public int iHealth;
    public int iCurrentHealth = 0;
    public int iSouls;
    private TextMeshProUGUI healthUI;
    private TextMeshProUGUI soulsUI;

    private void Awake()
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

    // Start is called before the first frame update
    void Start()
    {

        iCurrentHealth = iHealth;
        // Set UI vars
        healthUI = GameManager.instance.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
        soulsUI = GameManager.instance.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
        UpdateStatsUI();

        Battlefield.instance.onSummonInLane += CardSummoned;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartTurn()
    {
        // Don't draw a new card on first turn
        if (GameManager.instance.iTurnCounter == 1) return;

        CardManager.instance.DrawCard();
    }

    // Pay the cost of card and update UI
    void CardSummoned(int _iLaneIndex)
    {
        // Stops player from paying cost of enemy cards
        // Player lanes are index 1-3
        //if (_iLaneIndex > 3) return;

        iSouls -= CardManager.instance.cardToSummon.GetComponent<CardBase>().card.cost;
        UpdateStatsUI();
    }

    public void TakeDamage(int _iDamage)
    {
        iCurrentHealth -= _iDamage;
        UpdateStatsUI();
    }

    void UpdateStatsUI()
    {
        healthUI.text = iCurrentHealth.ToString();
        soulsUI.text = iSouls.ToString();
    }
}
