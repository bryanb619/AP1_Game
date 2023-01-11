using UnityEngine;

[CreateAssetMenu(menuName = "Akarya/abilities/Thunder")]
public class Thunder : Ability
{
    private ShooterThunder shoot;

    public override void Activate(GameObject parent)
    {
        // ACTIVATE ANIMATION

        // INSTANTIATE
        shoot = FindObjectOfType<ShooterThunder>();
        shoot.Shoot();
  
    }

    public override void BeginCooldown(GameObject parent)
    {
        shoot = FindObjectOfType<ShooterThunder>();
      
    }


}
