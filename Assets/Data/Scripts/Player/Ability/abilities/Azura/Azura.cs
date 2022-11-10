using UnityEngine;

[CreateAssetMenu]
public class Azura : Ability
{
    [SerializeField]
    private Animator _animAzura;

    public Player _playerScript;

    private AzuraEffect _effect;
    public int _health = 20;



    public override void Activate(GameObject parent)
    {
        // ACTIVATE ANIMATION

        // INSTANTIATE

        //Instantiate(_health, _healtPOS.position, _healtPOS.rotation);

        // POWER CODE

        _playerScript = FindObjectOfType<Player>();
        _effect = FindObjectOfType<AzuraEffect>();


        //_current
        if(_playerScript.HealthSetAtMax == false)
        {
            _playerScript.GiveHealth(_health);
            _effect.EffectSpawn();
            Debug.Log("Azura used");
        }
        
        else if (_playerScript.HealthSetAtMax == true)
        {
            Debug.Log("Player health is set to max" + _playerScript.CurretHealth);
        }


        
        
    }

    public override void BeginCooldown(GameObject parent)
    {
        _playerScript = FindObjectOfType<Player>();

        Debug.Log("Azura cooldown");
    }
}