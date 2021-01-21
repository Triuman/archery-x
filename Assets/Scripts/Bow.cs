using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{

    public Transform RightHandTransform;
    public Transform LeftHandTransform;
    public Transform UpperStringAttachment;
    public Transform LowerStringAttachment;
    public Transform CenterStringAttachment;

    public GameObject UpperStringHolderOutside;
    public GameObject UpperStringHolderInside;
    public GameObject LowerStringHolderOutside;
    public GameObject LowerStringHolderInside;

    public GameObject ArrowPrefab;
    public Transform ArrowAttachment;
    public GameObject ArrowHolderOutside;
    public GameObject ArrowHolderInside;
    public GameObject Arrow;

    public AudioSource FireSound;

    private bool rightHandTouchesString = false;

    private OVRHapticsClip oVRHapticsClip;
    private float lastVibrationMagnitude = 0;

    readonly List<GameObject> Arrowlist = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        int cnt = 20;
        oVRHapticsClip = new OVRHapticsClip(cnt);
    }

    private bool isHolding = false;

    // Update is called once per frame
    void Update()
    {
        transform.position = LeftHandTransform.position;

        Quaternion lerpHandRotation = Quaternion.Lerp(transform.rotation, LeftHandTransform.rotation, 0.2f);

        if (isHolding)
        {
            transform.rotation = Quaternion.LookRotation(ArrowAttachment.position - (RightHandTransform.position - new Vector3(0, 0.03f, 0)));
            transform.localRotation = new Quaternion(transform.localRotation.x, transform.localRotation.y, lerpHandRotation.z, lerpHandRotation.w);
        }
        else
        {
            transform.rotation = lerpHandRotation;
            transform.Rotate(0, 0.5f, 0);
        }

        float holdValue = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch);

        if (!isHolding && rightHandTouchesString && holdValue > 0.5f)
        {
            isHolding = true;
            ArrowHolderOutside.SetActive(true);
        }
        else if (isHolding && holdValue < 0.5f)
        {
            //Throw the arrow
            FireSound.Play();
            var newArrow = Instantiate(ArrowPrefab, Arrow.transform.position, Arrow.transform.rotation);
            Arrowlist.Add(newArrow);
            newArrow.GetComponent<Collider>().enabled = true;
            Rigidbody arrowRigidbody = newArrow.GetComponent<Rigidbody>();
            arrowRigidbody.useGravity = true;
            Vector3 possDiffArrow = (RightHandTransform.position - new Vector3(0, 0.03f, 0)) - ArrowAttachment.position;
            arrowRigidbody.AddRelativeForce(0, 0, -possDiffArrow.magnitude * 600);
            newArrow.GetComponent<Arrow>().isMoving = true;
            isHolding = false;
            ArrowHolderOutside.SetActive(false);
            rightHandTouchesString = false;
        }

        if (isHolding)
        {
            MoveStrings((RightHandTransform.position - new Vector3(0, 0.03f, 0)));
        }
        else
        {
            MoveStrings(CenterStringAttachment.position);
        }
    }

    public void DestroyArrows()
    {
        foreach (var arrow in Arrowlist)
        {
            Destroy(arrow);
        }
        Arrowlist.Clear();
    }

    public void MoveStrings(Vector3 handPosition)
    {
        UpperStringHolderOutside.transform.position = handPosition;
        Vector3 possDiffUpper = handPosition - UpperStringAttachment.position;
        float upperAngleZInside = Mathf.Atan2(Mathf.Sqrt(possDiffUpper.x * possDiffUpper.x + possDiffUpper.y * possDiffUpper.y), possDiffUpper.z);
        float upperAngleZOutside = Mathf.Atan2(-possDiffUpper.y, possDiffUpper.x);
        UpperStringHolderOutside.transform.eulerAngles = new Vector3(0, 0, 180 - upperAngleZOutside / Mathf.PI * 180);
        UpperStringHolderInside.transform.localEulerAngles = new Vector3(0, 180 - upperAngleZInside / Mathf.PI * 180, 0);
        UpperStringHolderInside.transform.localScale = new Vector3(UpperStringHolderInside.transform.localScale.x, UpperStringHolderInside.transform.localScale.y, possDiffUpper.magnitude / 0.8981151f * 0.15f);

        LowerStringHolderOutside.transform.position = handPosition;
        Vector3 possDiffLower = handPosition - LowerStringAttachment.position;
        float lowerAngleZInside = Mathf.Atan2(Mathf.Sqrt(possDiffLower.x * possDiffLower.x + possDiffLower.y * possDiffLower.y), possDiffLower.z);
        float lowerAngleZOutside = Mathf.Atan2(-possDiffLower.y, possDiffLower.x);
        LowerStringHolderOutside.transform.eulerAngles = new Vector3(0, 0, 180 - lowerAngleZOutside / Mathf.PI * 180);
        LowerStringHolderInside.transform.localEulerAngles = new Vector3(0, 180 - lowerAngleZInside / Mathf.PI * 180, 0);
        LowerStringHolderInside.transform.localScale = new Vector3(LowerStringHolderInside.transform.localScale.x, LowerStringHolderInside.transform.localScale.y, possDiffLower.magnitude / 0.8981151f * 0.15f);

        ArrowHolderOutside.transform.position = handPosition;
        Vector3 possDiffArrow = handPosition - ArrowAttachment.position;
        float arrowAngleZInside = Mathf.Atan2(Mathf.Sqrt(possDiffArrow.x * possDiffArrow.x + possDiffArrow.y * possDiffArrow.y), possDiffArrow.z);
        float arrowAngleZOutside = Mathf.Atan2(-possDiffArrow.y, possDiffArrow.x);
        ArrowHolderOutside.transform.eulerAngles = new Vector3(0, 0, 180 - arrowAngleZOutside / Mathf.PI * 180);
        ArrowHolderInside.transform.localEulerAngles = new Vector3(0, 180 - arrowAngleZInside / Mathf.PI * 180, 0);

        if (lastVibrationMagnitude + 0.04f < possDiffArrow.magnitude ||
            lastVibrationMagnitude - 0.04f > possDiffArrow.magnitude)
        {

            for (int i = 0; i < oVRHapticsClip.Samples.Length; i++)
            {
                oVRHapticsClip.Samples[i] = i % 2 == 0 ? (byte)0 : (byte)Mathf.Min(255, possDiffArrow.magnitude * 400);
            }
            oVRHapticsClip = new OVRHapticsClip(oVRHapticsClip.Samples, oVRHapticsClip.Samples.Length);

            lastVibrationMagnitude = possDiffArrow.magnitude;
            OVRHaptics.RightChannel.Preempt(oVRHapticsClip);
            OVRHaptics.LeftChannel.Preempt(oVRHapticsClip);
        }
    }


    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag != "RightHand")
            return;

        rightHandTouchesString = true;
    }
    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag != "RightHand")
            return;

        rightHandTouchesString = false;
    }
}
