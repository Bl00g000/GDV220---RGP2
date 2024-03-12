using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCameraAssigner : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        GetComponent<Canvas>().worldCamera = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
    }
}
