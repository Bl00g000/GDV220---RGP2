using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attack : MonoBehaviour
{
    public enum effectType
    {
        ON_PLAY,
        ON_HIT,
        ON_KILL,
        ON_LANE_COMBAT_END,
    }

    public abstract void ActivateAttack(int iLanePosIndex, int _ownLaneIndex, List<Lane> _ownLanes, List<Lane> _targetLanes);

    public abstract void ActivateEffect(int iLanePosIndex, int _ownLaneIndex, List<Lane> _ownLanes, List<Lane> _targetLanes, effectType _effectType);
}
