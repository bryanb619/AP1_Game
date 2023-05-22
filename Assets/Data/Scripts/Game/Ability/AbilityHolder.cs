using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class AbilityHolder : MonoBehaviour
{  
    [FormerlySerializedAs("abilityNEW")] [SerializeField] private Ability abilityNew;
    private Dashing _dashCooldownVariable;

    // used variables from "Ability script"

    private float _cooldownTime;
    private float _activeTime;
    [SerializeField] private float flashDelay;

    [SerializeField] private Slider uISlider;
    [SerializeField] private Transform handleRect;

    private bool _isPaused;

    private Shooter _shooter; 

    private WeaponType _weaponType; 

    // types of ability condition
    private enum AbilityState
    {
        Ready,
        Active,
        Cooldown
    }

    // Ability state

    private AbilityState _state = AbilityState.Ready;

    // key input
    [SerializeField]
    private KeyCode key;

    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    

    private void Start()
    {
        _shooter = FindObjectOfType<Shooter>();
        PowerReady();
        _dashCooldownVariable = GameObject.FindGameObjectWithTag("Player").GetComponent<Dashing>();
    }

    // Update is called once per frame
    private void Update()
    {
        FindAbilityStateInput();
        PowerDisabled();
    }

    private void GameManager_OnGameStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.Paused:
                {
                    _isPaused = true;
                    break;
                }
                
            case GameState.Gameplay:
                {
                    _isPaused = false;
                    break;
                }
            default: {break;}
        }
    }

    

    private void FindAbilityStateInput()
    {
        if(!_isPaused) 
        {
            switch (_state)
            {
                // ready state
                case AbilityState.Ready:
                    if (Input.GetKeyUp(key))
                    {
                        if(abilityNew)
                        {
                        // change name
                        abilityNew.Activate(gameObject);
                        _activeTime = abilityNew.activeTime;
                        }

                        _state = AbilityState.Active;
                        PowerReady();
                    }
                    break;

                // activate state
                case AbilityState.Active:

                    if (_activeTime > 0)
                    {
                        _activeTime -= Time.deltaTime;
                    }
                    else
                    {
                        if (abilityNew)
                        {
                            _cooldownTime = abilityNew.cooldownTime;
                            abilityNew.BeginCooldown(gameObject);
                        }
                        else
                        {
                            _cooldownTime = _dashCooldownVariable.dashCd;
                            Debug.Log("Cooldown Time = " + _cooldownTime);
                        }
                        
                        // call ability cooldown
                        _state = AbilityState.Cooldown;
                        uISlider.maxValue = _cooldownTime;
                    }

                    break;

                case AbilityState.Cooldown:

                    //Debug.Log("Entered Ability cooldown");
                    
                    if (_cooldownTime > 0)
                    {
                        handleRect.GetComponent<Image>().enabled = true;
                        _cooldownTime -= Time.deltaTime;
                        
                        if(_cooldownTime <= 0 + flashDelay)
                            ReadyFlash();
                    }

                    else
                    {
                        _state = AbilityState.Ready;
                        //cooldownTime = abilityNEW.cooldownTime;

                        handleRect.GetComponent<Image>().enabled = false;
                        PowerReady();
                    }
                    break;
            }
        }
        
    }

    private void PowerReady()
    {
        uISlider.value = 0f;
    }

    private void PowerDisabled()
    {
        if(_state == AbilityState.Cooldown)
        {   
            uISlider.value = Mathf.Lerp(0f, _cooldownTime, _cooldownTime);
        }
    }

    #region special condition
    internal void TargetedAttackCooldownUi()
    {
        if (abilityNew)
        {
            // change name
            abilityNew.Activate(gameObject);
            _activeTime = abilityNew.activeTime;
        }

        _state = AbilityState.Active;
        PowerReady();
        ReadyFlash();
    }
    
    internal void AreaAttackCooldownUi()
    {
        if (abilityNew)
        {
            // change name
            abilityNew.Activate(gameObject);
            _activeTime = abilityNew.activeTime;
        }

        _state = AbilityState.Active;
        PowerReady();
        ReadyFlash();
    }
    #endregion
    
    private void ReadyFlash()
    {
        uISlider.GetComponent<Animation>().Play();
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }

}