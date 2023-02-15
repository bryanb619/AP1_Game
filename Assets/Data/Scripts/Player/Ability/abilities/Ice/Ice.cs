using UnityEngine;

[CreateAssetMenu(menuName = "Akarya/abilities/Ice")]
public class Ice : Ability
{
    private Shooter shoot;

    public override void Activate(GameObject parent)
    {
        shoot = FindObjectOfType<Shooter>();
        if (shoot != null)
        {
            shoot.Shoot();
        }
    }

    public override void BeginCooldown(GameObject parent)
    {
        shoot = FindObjectOfType<Shooter>();
      
    }


}
