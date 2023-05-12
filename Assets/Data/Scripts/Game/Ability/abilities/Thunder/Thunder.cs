using UnityEngine;

[CreateAssetMenu(menuName = "Akarya/abilities/Thunder")]
public class Thunder : Ability
{
    private Shooter _shoot;

    public override void Activate(GameObject parent)
    {
        // ACTIVATE ANIMATION

        // INSTANTIATE
        if (_shoot != null)
        {
            _shoot = FindObjectOfType<Shooter>();
            _shoot.Shoot();
        }

    }

    public override void BeginCooldown(GameObject parent)
    {
        //shoot = FindObjectOfType<ShooterThunder>();
      
    }


}
