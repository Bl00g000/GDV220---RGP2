using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scarab_1 : Attack
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
        int iHealAmount = 1;

        switch (_effectType)
        {
            case effectType.ON_PLAY:

                // check that summon is not at the very front
                if (iLanePosIndex > 0)
                {
                    for (int i = 0; i < iLanePosIndex; i++)
                    {
                        if (_ownLanes[_ownLaneIndex].summons[i] != null)
                        {
                            _ownLanes[_ownLaneIndex].summons[i].GetComponentInParent<CardBase>().iHealth += iHealAmount;
                        }
                    }
                }
                
                break;
            case effectType.ON_HIT:
                break;
            case effectType.ON_KILL:
                break;
            case effectType.ON_LANE_COMBAT_END:
                break;
            default:
                break;
        }
    }
}
