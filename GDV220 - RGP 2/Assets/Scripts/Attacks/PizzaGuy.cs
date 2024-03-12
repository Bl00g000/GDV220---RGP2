using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PizzaGuy : Attack
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
        switch (_effectType)
        {
            case effectType.ON_PLAY:

                

                break;
            case effectType.ON_HIT:
                
                break;
            case effectType.ON_KILL:
                
                break;
            case effectType.ON_LANE_COMBAT_END:
                // on play does 50 damage to all enemies, pizza guy is just cool like that
                for (int i = 0; i < 3; i++)
                {
                    // checks which lane it is in then attacks the summon at the front of the opposite lane 
                    if (_ownLaneIndex == i)
                    {
                        if (_targetLanes[i].summons[0] != null)
                        {
                            _targetLanes[i].summons[0].GetComponentInParent<CardBase>().TakeDamage(50);
                        }
                    }
                }

                // probably figure out how to check for deaths here or maybe on the cardbase itself?? who knows figure it out later
                break;
            default:
                break;
        }
    }
}
