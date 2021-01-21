using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public bool isMoving = false;

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            rigidbody.AddForce(0,1f,0);
            RotateArrowByVelocity(rigidbody);
        }
    }

    void RotateArrowByVelocity(Rigidbody rigidbody)
    {
        if (rigidbody.velocity.magnitude < 0.05f)
            return;
        transform.rotation = Quaternion.LookRotation(new Vector3(rigidbody.velocity.x, rigidbody.velocity.y, rigidbody.velocity.z));
        transform.eulerAngles = new Vector3(transform.eulerAngles.x + 180, transform.eulerAngles.y, transform.eulerAngles.z);
    }
    
    void OnTriggerEnter(Collider coll)
    {
        print(coll.tag);

        if (coll.tag == "Bow" || coll.tag == "Arrow" || coll.tag == "RightHand" || coll.tag == "LeftHand" || coll.tag == "BarSight")
            return;

        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.useGravity = false;
        rigidbody.isKinematic = true;
    }

}
