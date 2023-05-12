using UnityEngine;

[CreateAssetMenu(menuName = "Akarya/abilities/Normal")]
public class Normal : Ability
{
    private Shooter _shoot;

    public override void Activate(GameObject parent)
    {
        // ACTIVATE ANIMATION
        _shoot = FindObjectOfType<Shooter>();
        // INSTANTIATE
        if (_shoot != null)
        {
            
            _shoot.Shoot();
        }



        _shoot.Shoot();
        //_player.TakeDamage(7);

        // POWER CODE
    }

    public override void BeginCooldown(GameObject parent)
    {
        _shoot = FindObjectOfType<Shooter>();
      
    }


}
