using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField]private Transform muzzle;
    [SerializeField]private Bullet bullet;
    [SerializeField]private float msBetweenShots = 100;
    [SerializeField]private float muzzleVelocity = 35;

    private float _shotCoolDown;
    
    public void Shoot(float damage, LayerMask collisionMask)
    {
        if (_shotCoolDown > 0)
        {
            _shotCoolDown -= Time.deltaTime * 1000;
            return;
        }

        _shotCoolDown = msBetweenShots;
        var newBullet = Instantiate(bullet, muzzle.position, muzzle.rotation);
        newBullet.SetBullet(collisionMask, muzzleVelocity, damage);
    }
}