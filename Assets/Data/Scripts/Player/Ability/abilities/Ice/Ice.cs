using UnityEngine;

[CreateAssetMenu(menuName = "Akarya/abilities/Ice")]
public class Ice : Ability
{
    private Shooter shoot;

    public override void Activate(GameObject parent)
    {
        // ACTIVATE ANIMATION

        // INSTANTIATE
        shoot = FindObjectOfType<Shooter>();



        shoot.Shoot();
        //_player.TakeDamage(7);

        // POWER CODE
    }

    public override void BeginCooldown(GameObject parent)
    {
        shoot = FindObjectOfType<Shooter>();
      
    }


}
