//using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AbilityHolder : MonoBehaviour
{
    //[SerializeField] private Image icon = default;
    //[SerializeField] private Image coolDownImage = default;

  
    public Ability abilityNEW;

    // used variables from "Ability script"

    [SerializeField]private float cooldownTime;
    private float activeTime;
    
    [SerializeField]
    private Slider uISlider;

    //[SerializeField] private SpriteRenderer CoolDownSprite, ReadySprite;

    [SerializeField] 
    private GameObject ActiveSprite, CooldownSprite;

    private bool _isPaused;

    private Shooter shooter; 

    private WeaponType _weaponType; 

    // types of ability condition
    private enum Ability_State
    {
        ready,
        active,
        cooldown
    }

    // Ability state // INITIAL???

    private Ability_State state = Ability_State.ready;

    // Player INPUT

    // key input
    [SerializeField]
    private KeyCode key;

    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    

    private void Start()
    {
        shooter = FindObjectOfType<Shooter>();
        PowerReady();
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
            switch (state)
            {
                // ready state
                case Ability_State.ready:
                    if (Input.GetKeyDown(key))
                    {
                        // change name
                        abilityNEW.Activate(gameObject);
                        activeTime = abilityNEW.activeTime;
                        state = Ability_State.active;

                        PowerReady();
                    }
                    break;

                // activate state
                case Ability_State.active:

                    if (activeTime > 0)
                    {
                        activeTime -= Time.deltaTime;
                    }
                    else
                    {
                        // call ability cooldown
                        abilityNEW.BeginCooldown(gameObject);
                        state = Ability_State.cooldown;
                        cooldownTime = abilityNEW.cooldownTime;
                    }

                    break;

                case Ability_State.cooldown:

                    Debug.Log("Entered Ability cooldown");
                    
                    if (cooldownTime > 0)
                    {
                        cooldownTime -= Time.deltaTime;
                    }

                    else
                    {
                        state = Ability_State.ready;
                        //cooldownTime = abilityNEW.cooldownTime;

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
        if(state == Ability_State.cooldown)
        {   
            uISlider.value = Mathf.Lerp(0f, cooldownTime, cooldownTime);
        }
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }

}