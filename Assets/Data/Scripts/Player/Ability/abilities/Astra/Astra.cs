using UnityEngine;

[CreateAssetMenu]
public class Astra : Ability
{
    [SerializeField]
    private Animator _animAstra;

    //public AIBehaviour _enemyScript;

    public AstraShoot AstraShoot;

    public Player _player;

    public override void Activate(GameObject parent)
    {
        // ACTIVATE ANIMATION

        // INSTANTIATE
        AstraShoot = FindObjectOfType<AstraShoot>();

        _player = FindObjectOfType<Player>();

        AstraShoot.Shoot();
        //_player.TakeDamage(7);

        // POWER CODE
    }

    public override void BeginCooldown(GameObject parent)
    {
        AstraShoot = FindObjectOfType<AstraShoot>();
        _player = FindObjectOfType<Player>();
    }
}