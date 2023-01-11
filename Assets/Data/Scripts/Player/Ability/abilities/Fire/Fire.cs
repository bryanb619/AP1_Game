using UnityEngine;

[CreateAssetMenu(menuName = "Akarya/abilities/Fire")]
public class Fire : Ability
{
    private ShooterFire shoot;

    public override void Activate(GameObject parent)
    {
        // ACTIVATE ANIMATION

        // INSTANTIATE
        shoot = FindObjectOfType<ShooterFire>();



        shoot.Shoot();
        //_player.TakeDamage(7);

        // POWER CODE
    }

    public override void BeginCooldown(GameObject parent)
    {
        shoot = FindObjectOfType<ShooterFire>();
      
    }


}
