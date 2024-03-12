using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jackal_3 : Attack
{
    public override void ActivateAttack(int iLanePosIndex, int _ownLaneIndex, List<Lane> _ownLanes, List<Lane> _targetLanes)
    {
        if (_targetLanes[_ownLaneIndex].summons[0] != null)
        {
            // get attackers damage
            int iDamage = _ownLanes[_ownLaneIndex].summons[iLanePosIndex].GetComponentInParent<CardBase>().iDamage;

            // deal damage to target
            _targetLanes[_ownLaneIndex].summons[0].GetComponentInParent<CardBase>().TakeDamage(iDamage);

            // activate on hit effects
            ActivateEffect(iLanePosIndex, _ownLaneIndex, _ownLanes, _targetLanes, effectType.ON_HIT);
        }
    }

    public override void ActivateEffect(int iLanePosIndex, int _ownLaneIndex, List<Lane> _ownLanes, List<Lane> _targetLanes, effectType _effectType)
    {
        int iDamageAmount = 1; 

        switch (_effectType)
        {
            case effectType.ON_PLAY:
                break;
            case effectType.ON_HIT:
                break;
            case effectType.ON_KILL:
                break;
            case effectType.ON_LANE_COMBAT_END:

                int laneCount = 0;

                // check how many lanes are still active
                for (int i = 0; i < 3; i++)
                {
                    if (_ownLanes[i] != null)
                    {
                        laneCount++;
                    }
                }
                int randomLane = Random.Range(1, laneCount);   // max is exclusive

                // iterate through all lanes and all their summons and then add all valid
                // summons to our randomTarget list
                for (int i = 0; i < 3; i++)
                {
                    if (_targetLanes[randomLane].summons[i] != null)
                    {
                        // deal damage to the random target
                        _targetLanes[randomLane].summons[i].GetComponentInParent<CardBase>().TakeDamage(iDamageAmount);
                    }
                }

                break;
            default:
                break;
        }
    }
}
