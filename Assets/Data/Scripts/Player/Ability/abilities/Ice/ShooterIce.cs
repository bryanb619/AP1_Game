using UnityEngine;

public class ShooterIce : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;

    public void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
}
