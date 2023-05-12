using UnityEngine;

[CreateAssetMenu(menuName = "Akarya/abilities/Ice")]
public class Ice : Ability
{
    private Shooter _shoot;

    public override void Activate(GameObject parent)
    {
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
