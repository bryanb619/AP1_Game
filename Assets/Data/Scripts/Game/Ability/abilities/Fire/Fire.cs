using UnityEngine;

[CreateAssetMenu(menuName = "Akarya/abilities/Fire")]
public class Fire : Ability
{
    private Shooter _shoot;
    
    public override void Activate(GameObject parent)
    {
        // ACTIVATE ANIMATION

        // INSTANTIATE

        if(_shoot != null) 
        {

            _shoot = FindObjectOfType<Shooter>();

            _shoot.Shoot();
        }
        
        //_player.TakeDamage(7);

        // POWER CODE
    }

    public override void BeginCooldown(GameObject parent)
    {
        _shoot = FindObjectOfType<Shooter>();
      
    }


}
