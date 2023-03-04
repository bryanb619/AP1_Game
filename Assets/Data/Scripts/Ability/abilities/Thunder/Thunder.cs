using UnityEngine;

[CreateAssetMenu(menuName = "Akarya/abilities/Thunder")]
public class Thunder : Ability
{
    private Shooter shoot;

    public override void Activate(GameObject parent)
    {
        // ACTIVATE ANIMATION

        // INSTANTIATE
        if (shoot != null)
        {
            shoot = FindObjectOfType<Shooter>();
            shoot.Shoot();
        }

    }

    public override void BeginCooldown(GameObject parent)
    {
        //shoot = FindObjectOfType<ShooterThunder>();
      
    }


}
