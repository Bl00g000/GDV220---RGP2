using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardRotate : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler
{
    public bool bMouseOnCard;
    public float fTiltAmount;
    public float fTiltSpeed;
    public float fTiltResetSpeed;
    private Quaternion qInitialRotation;
    private Quaternion qBoxInitialRotation;
    public GameObject cardSprite;
    public GameObject cardParralax1;
    private Vector3 localOffset;
    private void Start()
    {
        qInitialRotation = transform.parent.localRotation;
        qBoxInitialRotation = transform.localRotation;

        cardSprite.transform.SetParent(cardParralax1.transform);
    }

    public void Update()
    {
        transform.localRotation = qBoxInitialRotation;
        
        CardTiltEffect();
    }

    public void OnPointerEnter(PointerEventData eventData )
    {
        bMouseOnCard = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        bMouseOnCard = false;
    }

    private void OnMouseExit()
    {
        bMouseOnCard = false;
    }

    void CardTiltEffect()
    {
        if (bMouseOnCard)
        {
            Vector3 v3MousePosition = Input.mousePosition;
            Vector3 v3DirectionToMouse = (v3MousePosition - Camera.main.WorldToScreenPoint(transform.parent.position)).normalized;

            float fTiltX = v3DirectionToMouse.y * fTiltAmount;
            float fTiltY = -v3DirectionToMouse.x * fTiltAmount;

            Quaternion qTargetRotation = Quaternion.Euler(fTiltX, fTiltY, 0f);
            transform.parent.localRotation = Quaternion.Slerp(transform.parent.localRotation, qTargetRotation, fTiltSpeed * Time.deltaTime);
        }
        else
        {
            transform.parent.localRotation = Quaternion.Slerp(transform.parent.localRotation, qInitialRotation, fTiltResetSpeed * Time.deltaTime);
        }
    }
}
