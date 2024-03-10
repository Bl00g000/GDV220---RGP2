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


    //public List<Enemy> enemyList = new List<Enemy>();

    //Enemies
    public Enemy Enemy1 = null;
    public Enemy Enemy2 = null;
    public Enemy Enemy3 = null;
   
    public Vector3 v3PlayerSpriteLocation = Vector3.zero;

    //public Vector3 v3Enemy1SpriteLocation = Vector3.zero;
    //public Vector3 v3Enemy2SpriteLocation = Vector3.zero;
    //public Vector3 v3Enemy3SpriteLocation = Vector3.zero;

    [HideInInspector] public List<Lane> allLanes = new List<Lane>();
    [HideInInspector] public List<Lane> allyLanes = new List<Lane>();
    [HideInInspector] public List<Lane> enemyLanes = new List<Lane>();

    public event Action<int> onSummonInLane;

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

        LoadNewLevel();

        //Assigning the enemies lanes to their respective enemies
        if (Enemy1 && allLanes[3]) { Enemy1.assignedLane = allLanes[3]; }
        if (Enemy2 && allLanes[4]) { Enemy2.assignedLane = allLanes[4]; }
        if (Enemy3 && allLanes[5]) { Enemy3.assignedLane = allLanes[5]; }
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

        if(Enemy1)
        {
            Enemy1.gameObject.transform.position = GameObject.Find("Enemy1Location").transform.position;
        }
        if (Enemy2)
        {
            Enemy2.gameObject.transform.position = GameObject.Find("Enemy2Location").transform.position;
        }
        if (Enemy3)
        {
            Enemy3.gameObject.transform.position = GameObject.Find("Enemy3Location").transform.position;
        }
    }


    //Load the new level
    public void LoadNewLevel()
    {

        //Empty level?

       


    }   //

    public void SummonInLane(int _iLaneIndex)
    {
        Debug.Log("Battlefield summoning on lane " + _iLaneIndex);
        onSummonInLane?.Invoke(_iLaneIndex);
    }

    public void CommenceBattle()
    {
        StartCoroutine(PlayBattle());
    }

    public IEnumerator PlayBattle()
    {
        for (int i = 0; i < 3; i++)
        {
            // Play combat in player lane
            allLanes[i].bCombatPlaying = true;
            StartCoroutine(allLanes[i].PlayCombat());

            // Play combat in opposing lane
            allLanes[i + 3].bCombatPlaying = true;
            StartCoroutine(allLanes[i + 3].PlayCombat());
            yield return new WaitUntil(() => allLanes[i + 3].bCombatPlaying == false);
        }

        // Wait for a sec then continue onto next turn
        yield return new WaitForSeconds(1.0f);
        GameManager.instance.ChangeGameState(GameManager.GameState.Game_PrepPhase);
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
            if (allySummons[round] != null)
            {
                allySummons[round].card.attack.ActivateAttack(allySummons[round].iLanePosIndex, _iLaneIndex, allyLanes, enemyLanes);
            }
            if (enemySummons[round] != null)
            {
                enemySummons[round].card.attack.ActivateAttack(enemySummons[round].iLanePosIndex, _iLaneIndex, enemyLanes, allyLanes);
            }

            // check if the summons have taken enough damage to die and then destroy the game objects
            // and set the summon slot to null
            if (allySummons[round] != null)
            {
                if (allySummons[round].iHealth <= 0)
                {
                    Destroy(allySummons[round].gameObject);

                    // this is a little weird because I can't use the round to set the summon to null since
                    // if were on round 2 which means the second wave of summons take their turn, the first wave dying
                    // might have caused the second wave to advance in the row which means setting the round 2 of
                    // the lane to null would actually set summon 3 to null and break the lane logic... (... say hi bloo)
                    allyLanes[_iLaneIndex].summons[allySummons[round].iLanePosIndex] = null;
                }
            }
            if (enemySummons[round] != null)
            {
                if (enemySummons[round].iHealth <= 0)
                {
                    Destroy(enemySummons[round].gameObject);
                    
                    // this is a little weird because I can't use the round to set the summon to null since
                    // if were on round 2 which means the second wave of summons take their turn, the first wave
                    // might have caused the second wave to advance in the row which means setting the round 2 of
                    // the lane to null would actually set summon 3 to null and break the lane logic... (... say hi bloo AGAIN!)
                    enemyLanes[_iLaneIndex].summons[enemySummons[round].iLanePosIndex] = null;
                }
            }

            // advance rows after the round
            allLanes[_iLaneIndex].AdvanceRow();
            allLanes[_iLaneIndex + 3].AdvanceRow();

            yield return new WaitForSeconds(1.0f);
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

        if (Enemy1 && !Enemy1.IsDead()) { Enemy1.PlayTurn(); }
        Enemy1.bThinking = true;
        yield return new WaitUntil(() => Enemy1.bThinking == false); ;

        if (Enemy2 && !Enemy2.IsDead()) { Enemy2.PlayTurn(); }
        Enemy2.bThinking = true;
        yield return new WaitUntil(() => Enemy2.bThinking == false); ;

        if (Enemy3 && !Enemy3.IsDead()) { Enemy3.PlayTurn(); }
        Enemy3.bThinking = true;
        yield return new WaitUntil(() => Enemy3.bThinking == false); ;

       // yield return new WaitForSeconds(1.0f);

        GameManager.instance.ChangeGameState(GameState.Game_CombatPhase);
    }
}
