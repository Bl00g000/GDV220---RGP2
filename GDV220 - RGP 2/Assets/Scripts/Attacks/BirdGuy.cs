using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdGuy : Attack
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
        int iHealthToGain = 1;

        // create new lists
        List<GameObject> allEnemies = new List<GameObject>();
        List<GameObject> allAllies = new List<GameObject>();

        switch (_effectType)
        {
            case effectType.ON_PLAY:

                // iterate through all lanes and all their summons and then add all valid
                // summons to our allEnemies list
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (_ownLanes[i].summons[j] != null)
                        {
                            allAllies.Add(_ownLanes[i].summons[j]);
                        }
                        if (_targetLanes[i].summons[j] != null)
                        {
                            allEnemies.Add(_targetLanes[i].summons[j]);
                        }
                    }
                }

                foreach (GameObject enemy in allEnemies)
                {
                    // take damage equal to ally count
                    enemy.GetComponentInParent<CardBase>().TakeDamage(allAllies.Count);
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

