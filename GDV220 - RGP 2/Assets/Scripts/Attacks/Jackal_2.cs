using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jackal_2 : Attack
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
        // gets a random value between 1-3
        int iDamageAmount = Random.Range(1, 4);   // max is exclusive

        // create new list for all targets
        List<GameObject> randomTarget = new List<GameObject>();

        switch (_effectType)
        {
            case effectType.ON_PLAY:
                break;
            case effectType.ON_HIT:
                break;
            case effectType.ON_KILL:
                break;
            case effectType.ON_LANE_COMBAT_END:

                // iterate through all lanes and all their summons and then add all valid
                // summons to our randomTarget list
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (_targetLanes[i] != null)
                        {
                            if (_targetLanes[i].summons[j] != null)
                            {
                                randomTarget.Add(_targetLanes[i].summons[j]);
                            }
                        }
                    }
                }

                if (randomTarget.Count > 0)
                {
                    // select a random target out of the list
                    int iRandomTargetIndex = Random.Range(0, randomTarget.Count);

                    // deal damage to the random target
                    randomTarget[iRandomTargetIndex].GetComponentInParent<CardBase>().TakeDamage(iDamageAmount);
                }

                break;
            default:
                break;
        }
    }
}
