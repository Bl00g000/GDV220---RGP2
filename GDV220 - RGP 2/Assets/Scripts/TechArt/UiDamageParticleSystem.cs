using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UiDamageParticleSystem : MonoBehaviour
{
    [HideInInspector] public GameObject prefab_DamageNumber;
    [HideInInspector] public GameObject prefab_NumberParticleSystem;

    [HideInInspector] public float MinScale;
    [HideInInspector] public float HealthScaling;
    [HideInInspector] public float numberLifetime;

    [HideInInspector] public float scale;
    [HideInInspector] public float damageAmount;

    private Canvas canvas;

    void Start()
    {
        canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;

        StartCoroutine(HandleNumbers());
    }

    IEnumerator HandleNumbers()
    {
        float currentLifetime = 0;
        GameObject createdNumberSystem = gameObject.GetComponentInChildren<ParticleSystem>().gameObject;

        ParticleSystem particleSys = createdNumberSystem.GetComponent<ParticleSystem>();
        AttachGameObjectsToParticles particleAttach = createdNumberSystem.GetComponent<AttachGameObjectsToParticles>();

        particleAttach.m_Prefab = prefab_DamageNumber;
        particleSys.startLifetime = numberLifetime;

        while (currentLifetime < numberLifetime)
        {
            foreach (GameObject _inst in particleAttach.m_Instances)
            {
                TMPro.TextMeshProUGUI damageText = _inst.GetComponent<TMPro.TextMeshProUGUI>();
                if ((int)(Math.Round(damageAmount, 1) * 10.0) % 10 == 0)
                {
                    damageText.text = Math.Round(damageAmount, 0).ToString("f0");
                }
                else
                {
                    damageText.text = Math.Round(damageAmount, 1).ToString("f1");
                }

                damageText.color = Color.white;
                damageText.transform.localScale = Vector3.one * scale;

            }
            currentLifetime += Time.deltaTime;
            yield return null;
        }

        Destroy(createdNumberSystem.transform.root.gameObject);
    }
}
