using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Lane : MonoBehaviour
{
    //Lane data
    //public GameObject OwnerEntity = null;
    public int iLaneIndex = 0;
    public bool bFriendlyLane = false;
    public bool bActiveLane = true;

    [HideInInspector]public bool Advancing = false;

    //Creatures
    public List<GameObject> summons = new List<GameObject>();

    //Lane spawn locations
    public List<Transform> laneTransforms = new List<Transform>();

    // Just teddy things
    [HideInInspector] public bool bCombatPlaying = false;

   // public event Action<int> onLaneInactive;

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

        //moving to spot
        MoveSummonsToSpots();

        //Fills summons in the lane to the front
        AdvanceRow();
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
        if(bActiveLane && !CardDetails.instance.itIsActive)
        {
            if (CardManager.instance.cardToSummon != null)
            {
                Battlefield.instance.SummonInLane(iLaneIndex);
                SetSummonsLaneIndex();
            }
            else
            {
                Debug.Log("No card to summon");
            }
        }
        else
        {
            Debug.Log("Lane is Inactive");
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
            summons[0].transform.position = laneTransforms[0].position;
            //summons[0].transform.localRotation = laneTransforms[0].localRotation;
        }
        if (summons[1])
        {
            summons[1].transform.position = laneTransforms[1].position;
            //summons[1].transform.localRotation = laneTransforms[1].localRotation;
        }
        if (summons[2])
        {
            summons[2].transform.position = laneTransforms[2].position;
            //summons[2].transform.localRotation = laneTransforms[2].localRotation;
        }
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

    public void HideLanePlane()
    {
        gameObject.GetComponentInChildren<MeshRenderer>().enabled = false;
        //gameObject.SetActive(false);
    }
}
