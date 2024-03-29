using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenGuy_1 : Attack
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
        int iHealthToGain = 3;

        switch (_effectType)
        {
            case effectType.ON_PLAY:

                foreach (var lane in _ownLanes)
                {
                    if (lane.summons[0] != null)
                    {
                        lane.summons[0].GetComponentInParent<CardBase>().iHealth += iHealthToGain;
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
