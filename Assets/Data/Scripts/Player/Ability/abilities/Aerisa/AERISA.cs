using UnityEngine;

[CreateAssetMenu]
public class AERISA : Ability
{
    [SerializeField]
    private Animator _animAstra;

    public AerisaShoot AerisaShoot;
    private PlayerMovement _player;

    public override void Activate(GameObject parent)
    {
        // ACTIVATE ANIMATION

        AerisaShoot = FindObjectOfType<AerisaShoot>();

        _player = FindObjectOfType<PlayerMovement>();
        // INSTANTIATE
        AerisaShoot.Shoot();

        //_player.TakeDamage(12);

        // POWER CODE
    }

    public override void BeginCooldown(GameObject parent)
    {
        AerisaShoot = FindObjectOfType<AerisaShoot>();
        _player = FindObjectOfType<PlayerMovement>();

        Debug.Log("AERISA cooldown");
    }
}