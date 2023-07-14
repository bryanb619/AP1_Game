using System.Collections;
using System.Data;
using Data.Scripts.Game.Effects.Visuals;
using TMPro;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

//using UnityEngine.UI;
//using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]        private int             regenAmount;
    [SerializeField]        private float           regenTimer;
    [SerializeField]        private bool            regenOn;

    // --------------------------Shield----------------------------------------
    [Header ("Shield")]
    [SerializeField]        private float           shieldDecayWaitTime;
    [SerializeField]        private int             shieldDecayAmount;
    [SerializeField]        private float           shieldDecayRate;
    [SerializeField]        internal int            currentShield = 0;
    [SerializeField]        private GameObject      healthShield;


    // --------------------------Health----------------------------------------
    [Header ("Health")]
    [SerializeField]        internal int            currentHealth;
                            private HealthBar       _healthBar;
                            internal bool             HealthSetAtMax;
                            public int             maxHealth = 100;
                            internal int            CurretHealth => currentHealth;
                           
                            private PlayerMovement  _coroutineCaller;
                            internal PlayerHealthMaxed PlayerHealthState;

                            private GameManager _gameManager;
                            private PlayerAnimationHandler _playerAnim; 
                            
    // --------------------------Vignette----------------------------------------
    [SerializeField]        private Material _vignetteController;
                            private float intensity;
                            private WaitForSeconds fadeWait;

    internal enum PlayerHealthMaxed
    {
        Max,
        NotMax
    }

    // Start is called before the first frame update
    void Start()
    {
        fadeWait = new WaitForSeconds(0.1f);
        _coroutineCaller                     = FindObjectOfType<PlayerMovement>();
        _healthBar                          = FindObjectOfType<HealthBar>();
        _gameManager                        = FindObjectOfType<GameManager>();
        _playerAnim                         = GetComponentInChildren<PlayerAnimationHandler>();

        currentHealth                      = maxHealth;
        _healthBar.healthSlider.value       = currentHealth;
        _healthBar.healthSlider.maxValue    = maxHealth;
        HealthSetAtMax                      = true;
        currentHealth                      = 100;
    }

    internal void EmpowerHealth(int powerUp)
    {
        currentHealth += powerUp;
        
        print(currentHealth);
        //_healthBar.SetMaxHealth(powerUp, currentShield);
        _healthBar.HealthEmpower(currentHealth , powerUp);
        //PlayerPrefs()
        //print(_maxHealth);
    }
    
    public void TakeDamage(int damage)
    {
        print("Player took damage: " + damage);
        if (currentShield < damage)
        {
            damage -= currentShield;
            currentShield = 0;
            currentHealth -= (damage - currentShield);
        }
        else if (currentShield >= damage)
        {
            currentShield -= damage;
        }

        if ((currentShield + currentHealth) > maxHealth)
            _healthBar.SetMaxHealth(maxHealth, currentShield);
        else
            _healthBar.SetHealth(currentHealth, currentShield);

        regenOn = false;
        HealthSetAtMax = false;

        StartCoroutine(_coroutineCaller.VisualFeedbackDamage());

        _healthBar.SetHealth(currentHealth, currentShield);

        if (currentHealth <= 0)
        {
            _coroutineCaller.StopMovement();
            _playerAnim.DeathAnim();

            _gameManager.UpdateGameState(GameState.Death);
        }
        else
        {
            _playerAnim.HitAnim();
        }
    }

    internal void RegenerateHealth(int health)
    {
        currentHealth += health;

        StartCoroutine(_coroutineCaller.VisualFeedbackHeal());

        _healthBar.SetMaxHealth(maxHealth, currentShield);

        _healthBar.SetHealth(currentHealth, currentShield);

        if (currentHealth >= maxHealth)
        {
            HealthSetAtMax = true;
            currentHealth = maxHealth;
            _healthBar.SetHealth(currentHealth, currentShield);
        }

        _coroutineCaller.VisualFeedbackHeal();
    }

    internal void GiveShield(int shieldAmount)
    {
        this.currentShield = shieldAmount;
        _healthBar.SetMaxHealth(maxHealth, currentShield);
        _healthBar.SetHealth(currentHealth, currentShield);

        StopCoroutine(DecayTimer());
        StartCoroutine(DecayTimer());
    }

    internal void Takehealth(int health)
    {
        currentHealth += health;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
            _healthBar.SetHealth(maxHealth, currentShield);
    }

    private void Update()
    {
        

        switch (HealthSetAtMax)
        {
            case true:
                {
                    PlayerHealthState = PlayerHealthMaxed.Max;
                    break;
                }
            case false:
                {
                    PlayerHealthState = PlayerHealthMaxed.NotMax;
                    break;
                }
        }

        Cheats();
    }

    private void VignetteChange(float fullscreenValue)
    {
        StartCoroutine(FadeVignette(fullscreenValue));



        /*
        _vignetteIntensity = Mathf.Round(_vignetteIntensity * 1000.0f) * 0.001f;
        if(_vignetteIntensity == 0f || _vignetteIntensity == 0.1f || _vignetteIntensity == 0.2f || _vignetteIntensity == 0.3f)
        {
            float interpolatedValue = Mathf.Lerp(_vignetteIntensity, fullscreenValue,1f);
        }
        */
    }

    IEnumerator FadeVignette(float fullscreenValue)
    {
        intensity = Mathf.Round(_vignetteController.GetFloat("_FullscreenIntensity") * 1000.0f) * 0.001f;
        if (intensity == fullscreenValue)
        {
            yield break;
        }
        else if(intensity > fullscreenValue)
        {
            while (intensity > fullscreenValue)
            {
                intensity -= 0.001f;
                _vignetteController.SetFloat("_FullscreenIntensity", intensity);
                yield return fadeWait;
            }
        }
        else if(intensity < fullscreenValue)
        {
            while (intensity < fullscreenValue)
            {
                intensity += 0.001f;
                _vignetteController.SetFloat("_FullscreenIntensity", intensity);
                yield return fadeWait;
            }
        }
    }

    private void FixedUpdate()
    {
        if (currentHealth <= (maxHealth * 0.3) && currentHealth > (maxHealth * 0.2))
        {
            VignetteChange(0.1f);
        }
    else if (currentHealth <= (maxHealth * 0.2) && currentHealth > (maxHealth * 0.1))
        {
            VignetteChange(0.2f);
        }
        else if (currentHealth <= (maxHealth * 0.1) && currentHealth > 0)
        {
            VignetteChange(0.3f);
        }
        else
        {
            VignetteChange(0f);
        }


        if (regenOn == true)
        {
            Invoke("Regen", regenTimer);
        }

    }

    IEnumerator DecayTimer()
    {
        yield return new WaitForSecondsRealtime(shieldDecayWaitTime);
        InvokeRepeating("ShieldDecay", 0f, shieldDecayRate);
    }

    private void ShieldDecay()
    {
        if (currentShield > 0)
        { 
            currentShield -= shieldDecayAmount;
            //_healthBar.SetMaxHealth(_maxHealth, currentShield);
           // _healthBar.SetHealth(_currentHealth, currentShield);
            healthShield.SetActive(true);
        }
        else
        {
            healthShield.SetActive(false);
        }
    }

    private void Cheats()
    {
        //Increase max health by 10
        if (Input.GetKeyDown(KeyCode.Z))
        {
            EmpowerHealth(10);
        }

        //Take 10 damage
        if (Input.GetKeyDown(KeyCode.B))
        {
            TakeDamage(10);
        }

        //Recover 10 health
        if (Input.GetKeyDown(KeyCode.P))
        {
            RegenerateHealth(10);
        }
    }
}
