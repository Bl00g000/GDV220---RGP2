using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIDamageNumbers : MonoBehaviour
{
    public CardBase attachedCardBase;

    public GameObject prefab_DamageNumber;
    public GameObject prefab_NumberParticleSystem;
    public float MinScale;
    public float HealthScaling;
    public float numberLifetime;

    private void Awake()
    {

        if(attachedCardBase == null)
        {
            Debug.LogError("ERROR: root object does not have damageable component");
        }
        else
        {
            attachedCardBase.OnTakeDamage += ShowDamageNumber;
        }
    }

    void ShowDamageNumber(float damageAmount)
    {
        if (attachedCardBase == null) { return; }

        GameObject createdParticleSystem = Instantiate(prefab_NumberParticleSystem, transform.position, Quaternion.identity);
        float scale = MinScale + (damageAmount / attachedCardBase.iStartHealth * HealthScaling);

        UiDamageParticleSystem partSystem = createdParticleSystem.GetComponent<UiDamageParticleSystem>();

        partSystem.MinScale = MinScale;
        partSystem.HealthScaling = HealthScaling;
        partSystem.numberLifetime = numberLifetime;
        partSystem.MinScale = MinScale;
        partSystem.prefab_DamageNumber = prefab_DamageNumber;

        partSystem.damageAmount = damageAmount;
        partSystem.scale = scale;
    }
}
