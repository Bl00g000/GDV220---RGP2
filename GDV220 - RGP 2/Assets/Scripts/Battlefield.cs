using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.EventSystems;
using static Attack;
using static GameManager;

public class Battlefield : MonoBehaviour
{
    //Reference to self
    public static Battlefield instance;

    //Enemies
    //public Enemy enemies[0] = null;
    //public Enemy enemies[1] = null;
    //public Enemy enemies[2] = null;
    
    // these gotta be lists for now sorry (say hi bloo)
    public List<Enemy> enemies = new List<Enemy>();

    public Vector3 v3PlayerSpriteLocation = Vector3.zero;

    //public Vector3 v3Enemy1SpriteLocation = Vector3.zero;
    //public Vector3 v3Enemy2SpriteLocation = Vector3.zero;
    //public Vector3 v3Enemy3SpriteLocation = Vector3.zero;

    [HideInInspector] public List<Lane> allLanes = new List<Lane>();
    [HideInInspector] public List<Lane> allyLanes = new List<Lane>();
    [HideInInspector] public List<Lane> enemyLanes = new List<Lane>();

    public event Action<int> onSummonInLane;
    private bool bLaneCombatPlaying = false;
    private bool bEnemyTurnsPlaying = false;

    //STATIC/SINGLETON THING 
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
        //find and assign lanes 
        InitiateLanes();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            StartCoroutine(ResolveBattles(0));
        }
    }

    //finds the lanes from the prefab and sets them up
    private void InitiateLanes()
    {
        // Teddy was hereeeeee
        // Initialize empty list with count
        for (int i = 0; i < 6; i ++)
        {
            allLanes.Add(null);
        }

        //Assigning prefab lanes to lane variables
        var lanes = gameObject.GetComponentsInChildren<Lane>();
        foreach (Lane lane in lanes)
        {
            // Teddy changed this all to the list version :(
            // Lane indices are from 1-6
            allLanes[lane.iLaneIndex - 1] = lane;
        }

        // create the lane lists with each sides lanes (need this for CastAbility)
        for (int i = 0; i < 3; i++)
        {
            allyLanes.Add(allLanes[i]);
            enemyLanes.Add(allLanes[i + 3]);
        }

        //Assigning the enemies lanes to their respective enemies
        if (enemies[0] && allLanes[3]) { enemies[0].assignedLane = allLanes[3]; }
        if (enemies[1] && allLanes[4]) { enemies[1].assignedLane = allLanes[4]; }
        if (enemies[2] && allLanes[5]) { enemies[2].assignedLane = allLanes[5]; }
    }

    public void SummonInLane(int _iLaneIndex)
    {
        onSummonInLane?.Invoke(_iLaneIndex);
    }

    public IEnumerator CommenceBattle()
    {
        if (allLanes[0].bActiveLane)
        {
            bLaneCombatPlaying = true;
            GameManager.instance.combatPhaseSign.SetActive(true);
            StartCoroutine(instance.ResolveBattles(0));
            yield return new WaitUntil(() => !bLaneCombatPlaying);
        }

        if (allLanes[1].bActiveLane)
        {
            bLaneCombatPlaying = true;
            StartCoroutine(instance.ResolveBattles(1));
            yield return new WaitUntil(() => !bLaneCombatPlaying);
        }

        if (allLanes[2].bActiveLane)
        {
            bLaneCombatPlaying = true;
            StartCoroutine(instance.ResolveBattles(2));
            yield return new WaitUntil(() => !bLaneCombatPlaying);
        }

        GameManager.instance.combatPhaseSign.SetActive(false);
        GameManager.instance.ChangeGameState(GameState.Game_PrepPhase);
    }

    public IEnumerator ResolveBattles(int _iLaneIndex)
    {
        List<CardBase> allySummons = new List<CardBase>();
        List<CardBase> enemySummons = new List<CardBase>();

        // create the summon lists with 3 null slots
        for (int i = 0; i < 3; i++)
        {
            allySummons.Add(null);
            enemySummons.Add(null);
        }

        // populate the lists with all summons for both player and enemy lanes of current laneIndex
        for (int i = 0; i < 3; i++)
        {
            if (allyLanes[_iLaneIndex].summons[i] != null)
            {
                allySummons[i] = allyLanes[_iLaneIndex].summons[i].GetComponentInParent<CardBase>();
            }
            if (enemyLanes[_iLaneIndex].summons[i] != null) 
            {
                enemySummons[i] = enemyLanes[_iLaneIndex].summons[i].GetComponentInParent<CardBase>();
            }
        }

        // iterate through all 3 summons and ensure that if they exist, they use their ability
        int round = 0;
        while (round < 3)
        {
            // disable indicator
            if (allySummons[round] != null)
            {
                allySummons[round].SetTurnIndicator(true);
            }
            if (enemySummons[round] != null)
            {
                enemySummons[round].SetTurnIndicator(true);
            }

            if (round < 2)
            {
                if (allySummons[round] != null || enemySummons[round] != null)
                {
                    yield return new WaitForSeconds(1.5f);
                }
            }
            else
            {
                yield return new WaitForSeconds(1.5f);
            }

            if (allySummons[round] != null)
            {
                allySummons[round].card.attack.ActivateAttack(allySummons[round].iLanePosIndex, _iLaneIndex, allyLanes, enemyLanes);

                // attack the enemy directly if they have no summons in their lane
                if (enemyLanes[_iLaneIndex].summons[0] == null)
                {
                    enemies[_iLaneIndex].TakeDamage(allySummons[round].GetComponentInParent<CardBase>().iDamage);
                }
            }
            if (enemySummons[round] != null)
            {
                enemySummons[round].card.attack.ActivateAttack(enemySummons[round].iLanePosIndex, _iLaneIndex, enemyLanes, allyLanes);

                // attack the player directly if they have no summons in their lane
                if (allyLanes[_iLaneIndex].summons[0] == null)
                {
                    Player.instance.TakeDamage(enemySummons[round].GetComponentInParent<CardBase>().iDamage);

                    // Check if player has died
                    GameManager.instance.GameEnd(GameManager.instance.CheckWinLoss());
                }
            }

            // check if the summons have taken enough damage to die and then destroy the game objects
            // and set the summon slot to null
            CheckLanesForDeath();

            // advance rows after the round
            allLanes[_iLaneIndex].AdvanceRow();
            allLanes[_iLaneIndex + 3].AdvanceRow();


            // disable indicator
            if (allySummons[round] != null)
            {
                allySummons[round].SetTurnIndicator(false);
            }
            if (enemySummons[round] != null)
            {
                enemySummons[round].SetTurnIndicator(false);
            }

            round++;
        }

        // resolve any end of lane combat effects
        for (int i = 0; i < 3; i++)
        {
            // check that summons are not null then activate their effects
            if (allySummons[i] != null)
            {
                allySummons[i].card.attack.ActivateEffect(allySummons[i].iLanePosIndex, _iLaneIndex, allyLanes, enemyLanes, effectType.ON_LANE_COMBAT_END);
            }
            if (enemySummons[i] != null)
            {
                enemySummons[i].card.attack.ActivateEffect(enemySummons[i].iLanePosIndex, _iLaneIndex, enemyLanes, allyLanes, effectType.ON_LANE_COMBAT_END);
            }
        }

        bLaneCombatPlaying = false;

        // Check if all enemies have died
        GameManager.instance.GameEnd(GameManager.instance.CheckWinLoss());
    }

    // Check all lanes if any summon has died and remove it from battlefield
    public void CheckLanesForDeath()
    {
        // Iterate through all lanes
        foreach (Lane lane in allLanes)
        {
            // Iterate through all summons in lane
                // Need a proper for-loop instead of for-each
                // So that it doesn't break when we remove things -_-
            for (int i = 0; i < lane.summons.Count; i++)
            {
                if (lane.summons[i] == null) continue;

                GameObject summon = lane.summons[i].gameObject;
                CardBase currentSummonCard = summon.GetComponentInParent<CardBase>();

                // Check if death
                if (currentSummonCard.iHealth <= 0)
                {
                    // Move card to discard pile and destroy summon
                    CardManager.instance.DiscardCard(currentSummonCard);

                    // Free lane position
                    lane.summons[currentSummonCard.iLanePosIndex] = null;
                }
            }

            // Advance remaining units in lane
            lane.AdvanceRow();
        }
    }

    //Plays enemy turn sequence
    public IEnumerator PlayEnemyTurns()
    {
        

        if (bEnemyTurnsPlaying || bLaneCombatPlaying) { yield break; }
        bEnemyTurnsPlaying = true;
        GameManager.instance.enemyTurnSign.SetActive(true);

        if (enemies[0] && !enemies[0].IsDead())
        {
            enemies[0].bThinking = true;
            enemies[0].PlayTurn();
        }

        yield return new WaitUntil(() => enemies[0].bThinking == false);

        if (enemies[1] && !enemies[1].IsDead())
        {
            enemies[1].bThinking = true;
            enemies[1].PlayTurn();
        }

        yield return new WaitUntil(() => enemies[1].bThinking == false);

        if (enemies[2] && !enemies[2].IsDead())
        {
            enemies[2].bThinking = true;
            enemies[2].PlayTurn();
        }
        yield return new WaitUntil(() => enemies[2].bThinking == false);

        // Delay before combat starts
        yield return new WaitForSeconds(1.0f);

        bEnemyTurnsPlaying = false;
        GameManager.instance.enemyTurnSign.SetActive(false);
        GameManager.instance.ChangeGameState(GameState.Game_CombatPhase);
    }
}