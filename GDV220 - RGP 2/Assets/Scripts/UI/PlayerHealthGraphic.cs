using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerHealthGraphic : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        GetComponent<Image>().fillAmount = Mathf.Lerp(GetComponent<Image>().fillAmount, (float)Player.instance.iCurrentHealth / (float)Player.instance.iHealth, Time.deltaTime*4f);
    }
}
