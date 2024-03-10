using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Lane : MonoBehaviour
{

    //Spot transform data and offsets
    public const float fSpotWidth = 4.0f;
    public const float fSpotHeight = 3.0f;
    public const float fSpotYOffset = 0.0f;
    public float fSpotCanvasOffset = 0.0f;

    //Lane data
    //public GameObject OwnerEntity = null;
    public int iLaneIndex = 0;
    public bool bFriendlyLane = false;

    [HideInInspector]public bool Advancing = false;

    //Creatures
    public List<GameObject> summons = new List<GameObject>();

    public List<Transform> lanePositions = new List<Transform>();
    //Lane spawn locations
    //protected Vector3 v3Spot1Location;
    //protected Vector3 v3Spot2Location;
    //protected Vector3 v3Spot3Location;

    // Just teddy things
    [HideInInspector] public bool bCombatPlaying = false;

    // Start is called before the first frame update
    void Start()
    {
        InitiateSummons();

        // Check if player lane based on lane index
        if (iLaneIndex < 4)
        {
            bFriendlyLane = true;
        }
        else 
        { 
            bFriendlyLane = false; 
        }
        
        //setting spot locations
        //v3Spot1Location = new Vector3(fSpotWidth / 2 - 6, fSpotYOffset, 0) + gameObject.transform.position; //IDEALLY USE THE BUTTON WIDTH
        //v3Spot2Location = new Vector3(fSpotWidth / 2 + fSpotWidth - 6, fSpotYOffset, 0) + gameObject.transform.position;
        //v3Spot3Location = new Vector3(fSpotWidth / 2 + fSpotWidth * 2 - 6, fSpotYOffset, 0) + gameObject.transform.position;
        //
        ////setting spot locations   //IDEALLY USE THE BUTTON WIDTH
        //v3Spot1Location = new Vector3(fSpotWidth / 2 - 6, fSpotYOffset, 0) + gameObject.transform.position;                     // Left   
        //v3Spot2Location = new Vector3(fSpotWidth / 2 + fSpotWidth - 6, fSpotYOffset, 0) + gameObject.transform.position;        // Middle
        //v3Spot3Location = new Vector3(fSpotWidth / 2 + fSpotWidth * 2 - 6, fSpotYOffset, 0) + gameObject.transform.position;    // Right

        // Mirror friendly lanes - swap spot 1 and 3
        //if (bFriendlyLane)
        //{
        //    Debug.Log("Mirroring lane " + iLaneIndex);
        //    Vector3 tempSpot = v3Spot1Location;
        //    v3Spot1Location = v3Spot3Location;
        //    v3Spot3Location = tempSpot;
        //}

        //moving to spot
        MoveSummonsToSpots();

        //Fills summons in the lane to the front
        AdvanceRow();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void InitiateSummons()
    {
        for (int i = 0; i < 3; i++)
        {
            summons.Add(null);
        }
    }

    //Checks if summons filling front to back and advances them if there are gaps.
    public void AdvanceRow()
    {
        //advancing spot 1
        if (summons[0] == null && summons[1] != null)
        {
            summons[0] = summons[1];
            summons[1] = null;
        }

        //advancing spot 2
        if (summons[1] == null && summons[2] != null)
        {
            summons[1] = summons[2];
            summons[2] = null;
        }

        //recurse once 
        if(summons[1] != null && summons[0] == null)
        {
            AdvanceRow();
        }

        //Moving them
        MoveSummonsToSpots();
        SetSummonsLaneIndex();

        //Debug.Log(transform.name + " fully advanced");
    }


    // Button on click function
    // Triggered when lane is chosen to summon card in
    public void LanePicked()
    {
        if (CardManager.instance.cardToSummon != null)
        {
            Battlefield.instance.SummonInLane(iLaneIndex);
            SetSummonsLaneIndex();
        }
        else
        {
            Debug.Log("no card to summon");
        }
    }

    // Checks if the lane is full and returns result
    public bool HasSpace()
    {
        if (summons[2] == null) return true;
        else return false;
    }

    //Adds a summon to a lane. 
    public void AddSummon(GameObject _newSummon)
    {
        //_newSummon.GetComponent<CardBase>().cardSummonObj.transform.position = Vector3.zero; 

        //assign summon to spot 3
        summons[2] = _newSummon;
        //Advance summons
        AdvanceRow();
    }

    //Translates the summons to their spots, checking for null errors, and flipping depending on loyalty
    public void MoveSummonsToSpots()
    {
        //spot 2 is always the same
        if (summons[0])
        {
            summons[0].transform.position = lanePositions[0].position;
        }
        if (summons[1])
        {
            summons[1].transform.position = lanePositions[1].position;
        }
        if (summons[2])
        {
            summons[2].transform.position = lanePositions[2].position;
        }
     
        // Adjust heights of summons
        //foreach (GameObject _summon in summons)
        //{
        //    if (_summon == null) continue;
        //
        //    _summon.transform.position += new Vector3(0, Mathf.Abs(_summon.transform.localPosition.y) / 2, 0);
        //}
    }

    //Quick way to check if a line is empty
    public bool LineHasSummons()
    {
        if (summons[0]) { return true; }
        if (summons[1]) { return true; }
        if (summons[2]) { return true; }

        return false;
    }

    // sets a variable inside the summons to which position they are on the board
    // this is important for their attacks
    public void SetSummonsLaneIndex()
    {
        for (int i = 0; i < 3;  i++)
        {
            if (summons[i] != null)
            {
                summons[i].gameObject.GetComponentInParent<CardBase>().iLanePosIndex = i;
            }
        }
    }

    //TEST FUNCTION
    public void InvokeLaneSummons(Action _funcToInvoke)
    {
        if (summons[0] != null)
        {
          //  summons[0]
          
        }
    }

    // TEDDY WAS HERE
    // Moves the front line summon to simulate attack
    public IEnumerator PlayCombat()
    {
        if (summons[0] == null)
        {
            bCombatPlaying = false;
            yield break;
        }

        float fAttackTime = 0.2f;
        float fReturnTime = 0.5f;
        float fElapsedTime = 0.0f;

        Vector3 v3OriginPos = summons[0].transform.position;
        Vector3 v3AttackPoint;
        
        // Set the attack point (towards the middle)
        if (bFriendlyLane)
        {
            v3AttackPoint = summons[0].transform.position + new Vector3(Mathf.Abs(summons[0].transform.position.x) - 1, 0, 0);
        }
        else
        {
            v3AttackPoint = summons[0].transform.position - new Vector3(summons[0].transform.position.x - 1, 0, 0);
        }

        // Moves the object towards the middle to simulate attack
        while (fElapsedTime <= fAttackTime)
        {
            fElapsedTime += Time.deltaTime;
            summons[0].transform.position = Vector3.Lerp(v3OriginPos, v3AttackPoint, fElapsedTime/fAttackTime);

            yield return null;
        }

        // Moves object back to its original spot
        fElapsedTime = 0.0f;
        while (fElapsedTime <= fReturnTime)
        {
            fElapsedTime += Time.deltaTime;
            summons[0].transform.position = Vector3.Lerp(v3AttackPoint, v3OriginPos, fElapsedTime / fAttackTime);
            yield return null;
        }

        bCombatPlaying = false;
    }
}
