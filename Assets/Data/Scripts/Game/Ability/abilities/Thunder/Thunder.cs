using UnityEngine;

[CreateAssetMenu(menuName = "Akarya/abilities/Thunder")]
public class Thunder : Ability
{
    private Shooter _shoot;

    public override void Activate(GameObject parent)
    {
        // ACTIVATE ANIMATION

        // INSTANTIATE
        _shoot = FindObjectOfType<Shooter>();
        
        if (_shoot != null)
        {
            _shoot.Shoot();
        }

    }

    public override void BeginCooldown(GameObject parent)
    {
        _shoot = FindObjectOfType<Shooter>();
      
    }


}
