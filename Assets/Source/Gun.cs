using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform muzzle;
    public Bullet bullet;
    public float msBetweenShots = 100;
    public float muzzleVelocity = 35;

    private float shotCoolDown;
    
    public void Shoot()
    {
        if (shotCoolDown > 0)
        {
            shotCoolDown -= Time.deltaTime * 1000;
            return;
        }

        shotCoolDown = msBetweenShots;
        var newBullet = Instantiate(bullet, muzzle.position, muzzle.rotation);
        newBullet.SetSpeed(muzzleVelocity);
    }
}