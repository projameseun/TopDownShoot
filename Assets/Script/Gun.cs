using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

    public Transform muzzle;
    public Projectile projectile;
    public float msBetweenShots = 100f;
    public float muzzleVelocity = 35f;

    public Transform shell; //탄피
    public Transform shellEjection; //위치

    Muzzleflash muzzleflash;

    void Start()
    {
        muzzleflash = GetComponent<Muzzleflash>();
    }

    float nextShortTime;

    public void Shoot()
    {
        if(Time.time > nextShortTime)
        {
            nextShortTime = Time.time + msBetweenShots / 1000;
            Projectile newprojectile = Instantiate(projectile, muzzle.position, muzzle.rotation) as Projectile;
            newprojectile.SetSpeed(muzzleVelocity);

            Instantiate(shell, shellEjection.position, shellEjection.rotation);
            muzzleflash.Activate();
        }
    }
   
}
