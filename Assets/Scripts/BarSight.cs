using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class BarSight : MonoBehaviour
{
    public Transform RightHandTransform;
    
    private float sightLocalZPosition;
    private Vector3 lastHandPosition;

    // Start is called before the first frame update
    void Start()
    {
        sightLocalZPosition = transform.localPosition.z;
    }

    // Update is called once per frame
    void Update()
    {
        if (handIsHolding)
        {
            if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch) < 0.5f)
            {
                handIsHolding = false;
            }
            else
            {
                //Move with hand
                Vector3 posDiff = RightHandTransform.position - lastHandPosition;
                transform.position += new Vector3(posDiff.x / 8, posDiff.y / 8, posDiff.z / 8);
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, sightLocalZPosition);
                lastHandPosition = RightHandTransform.position;

                PlayerPrefs.SetFloat(currentDistanceString + "SightX", transform.localPosition.x);
                PlayerPrefs.SetFloat(currentDistanceString + "SightY", transform.localPosition.y);
                PlayerPrefs.SetFloat(currentDistanceString + "SightZ", transform.localPosition.z);
            }
        }
        else if (handIsTouching && OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch) > 0.5f)
        {
            handIsHolding = true;
            lastHandPosition = RightHandTransform.position;
        }
    }

    private string currentDistanceString = "18";

    public void SetDistance(EnumDistance distance)
    {
        switch (distance)
        {
            case EnumDistance.M18:
                currentDistanceString = "18";
                break;
            case EnumDistance.M30:
                currentDistanceString = "30";
                break;
            case EnumDistance.M50:
                currentDistanceString = "50";
                break;
            case EnumDistance.M70:
                currentDistanceString = "70";
                break;
        }

        if (PlayerPrefs.HasKey(currentDistanceString + "SightX"))
        {
            float sightX = PlayerPrefs.GetFloat(currentDistanceString + "SightX");
            float sightY = PlayerPrefs.GetFloat(currentDistanceString + "SightY");
            float sightZ = PlayerPrefs.GetFloat(currentDistanceString + "SightZ");
            transform.localPosition = new Vector3(sightX, sightY, sightZ);
        }
    }

    private bool handIsTouching = false;
    private bool handIsHolding = false;


    void OnTriggerEnter(Collider coll)
    {
        if (coll.tag == "RightHand")
            handIsTouching = true;
    }
    void OnTriggerExit(Collider coll)
    {
        if (coll.tag == "RightHand")
            handIsTouching = false;
    }
}
