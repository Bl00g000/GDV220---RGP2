using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthUI : MonoBehaviour
{
    public Enemy enemy;
    void Update()
    {
        transform.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = enemy.iCurrentHealth.ToString();
    }
}
