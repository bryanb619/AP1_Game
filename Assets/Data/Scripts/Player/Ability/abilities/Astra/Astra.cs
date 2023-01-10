using UnityEngine;

[CreateAssetMenu]
public class Astra : Ability
{

    //public AIBehaviour _enemyScript;

    public AstraShoot AstraShoot;

    public PlayerMovement _player;

    public override void Activate(GameObject parent)
    {
        // ACTIVATE ANIMATION

        // INSTANTIATE
        AstraShoot = FindObjectOfType<AstraShoot>();

        _player = FindObjectOfType<PlayerMovement>();

        AstraShoot.Shoot();
        //_player.TakeDamage(7);

        // POWER CODE
    }

    public override void BeginCooldown(GameObject parent)
    {
        AstraShoot = FindObjectOfType<AstraShoot>();
        _player = FindObjectOfType<PlayerMovement>();
    }
}