using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardParralax : MonoBehaviour
{
    private Camera mainCam;
    public List<GameObject> ParralaxImages = new List<GameObject>();
    public List<Vector2> ParralaxMultiplier = new List<Vector2>();

    public float maxVertical;
    public float maxHorizontal;
    void Start()
    {
        mainCam = Camera.main;
    }

    #region Math Functions
    private float ClampEuler(float euler)
    {
        float newEuler = euler;
        if (newEuler >= 360) newEuler -= 360;
        return newEuler;
    }
    #endregion 

    // Update is called once per frame
    void Update()
    {
        if (ParralaxMultiplier.Count != ParralaxImages.Count)
        {
            Debug.LogError("ERROR: Parralax Image does not have an attached distance! IT WONT WORK FIX IT PLEASE");
            return;
        }

        for (int i = 0; i < ParralaxImages.Count; i++) 
        {
            GameObject parralax = ParralaxImages[i];

            Vector3 cameraAngle = mainCam.transform.forward;
            Vector3 parralaxAngle = parralax.transform.forward;

            // Find Angles between camera and card
            float angleDifferenceHorizontal = cameraAngle.y - parralaxAngle.y;
            float angleDifferenceVertical = cameraAngle.x - parralaxAngle.x;


            if(angleDifferenceHorizontal > maxHorizontal)
            {
                angleDifferenceHorizontal = maxHorizontal;
            }
            else if (angleDifferenceHorizontal < -maxHorizontal)
            {
                angleDifferenceHorizontal = -maxHorizontal;
            }

            if (angleDifferenceVertical > maxVertical)
            {
                angleDifferenceVertical = maxVertical;
            }
            else if(angleDifferenceVertical < -maxVertical)
            {
                angleDifferenceVertical = -maxVertical;
            }
            parralax.transform.localPosition = new Vector3(angleDifferenceVertical * ParralaxMultiplier[i].x, angleDifferenceHorizontal * ParralaxMultiplier[i].y);
        }
    }
}
