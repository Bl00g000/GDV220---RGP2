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
    private int iMaxSouls = 10;
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

    public void StartTurn()
    {
        // Don't draw a new card on first turn
        if (GameManager.instance.iTurnCounter == 1) return;

        if (iSouls < iMaxSouls)
        {
            iSouls += 2;
            UpdateStatsUI();
        }
        
        CardManager.instance.DrawCard();
        CardManager.instance.DrawCard();
    }

    // Pay the cost of card and update UI
    void CardSummoned(int _iLaneIndex)
    {
        iSouls -= CardManager.instance.cardToSummon.GetComponent<CardBase>().card.cost;
        UpdateStatsUI();
    }

    public void TakeDamage(int _iDamage)
    {
        iCurrentHealth -= _iDamage;
        StartCoroutine(DamageEffect());
        if (iCurrentHealth <= 0) iCurrentHealth = 0;
        UpdateStatsUI();
    }

    void UpdateStatsUI()
    {
        healthUI.text = iCurrentHealth.ToString();
        soulsUI.text = iSouls.ToString();
    }

    IEnumerator DamageEffect()
    {
        SpriteRenderer SR = GetComponentInChildren<SpriteRenderer>();
        SR.color = Color.red;
        yield return new WaitForSeconds(0.65f);

        SR.color = Color.white;
    }
}
