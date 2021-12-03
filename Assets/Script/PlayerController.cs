using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent ( typeof(Rigidbody))]

public class PlayerController : MonoBehaviour {

    Rigidbody myRigibody;

    Vector3 Velocity;
	
	void Start () {
        myRigibody = GetComponent<Rigidbody>();
	}

    public void Move(Vector3 _Velocity)
    {
        Velocity = _Velocity;
    }

    public void LookAt(Vector3 point)
    {
        Vector3 lookpoint = new Vector3(point.x, transform.position.y, point.z);
        transform.LookAt(lookpoint);
    }
	
    //
	private void FixedUpdate () {

        myRigibody.MovePosition(myRigibody.position + Velocity * Time.fixedDeltaTime);

	}
}
