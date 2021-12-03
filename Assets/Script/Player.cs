using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(PlayerController))]
[RequireComponent (typeof(GunController))]

public class Player : LivingEntity {

    public float speed = 5f;

    PlayerController Controller;

    Camera viewCamera;
    GunController gunController;
    
	protected override void Start () {

        base.Start();

        Controller = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
        viewCamera = Camera.main;
	}
	
	
	void Update () {

        //move
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, (Input.GetAxisRaw("Vertical")));

        Vector3 moveVelocity = moveInput * speed;

        Controller.Move(moveVelocity);


        //look at
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);

        Plane GroundPlane = new Plane(Vector3.up, Vector3.zero);

        float rayDistance;
        if (GroundPlane.Raycast(ray, out rayDistance))
            {
                Vector3 point = ray.GetPoint(rayDistance);
                Debug.DrawLine(ray.origin, point, Color.red);

                Controller.LookAt(point);
            }

        //weapon 

        if(Input.GetMouseButton(0))
        {
            gunController.Shoot();
        }
        
	}
}
