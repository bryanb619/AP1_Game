using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    // --------------------------Health----------------------------------------
    [Header ("Health")]
    [SerializeField]        internal int            _currentHealth;
                            private HealthBar       _healthBar;
                            internal bool             HealthSetAtMax;
                            private int             _maxHealth = 100;
                            internal int            CurretHealth => _currentHealth;
                           
                            private PlayerMovement  coroutineCaller;
                            internal _PlayerHealth _playerHealthState;

    internal enum _PlayerHealth
    {
        _Max,
        NotMax
    }

    // Start is called before the first frame update
    void Start()
    {
        coroutineCaller = FindObjectOfType<PlayerMovement>();
        _healthBar = FindObjectOfType<HealthBar>();

        _currentHealth = _maxHealth;
        _healthBar.healthSlider.value = _currentHealth;
        _healthBar.healthSlider.maxValue = _maxHealth;
        HealthSetAtMax = true;
        _currentHealth = 100;
    }

    internal void EmpowerHealth(int powerUp)
    {

        _maxHealth = _maxHealth + powerUp;

        //_healthBar._slider.maxValue = _maxHealth; 
        
        _currentHealth += powerUp;
        _healthBar.HealthEmpower(powerUp);



        //PlayerPrefs()
        //print(_maxHealth);
    }
    
    internal void TakeDamage(int damage)
    {
        if (currentShield < damage)
        {
            damage -= currentShield;
            currentShield = 0;
            _currentHealth -= (damage - currentShield);
        }
        else if (currentShield >= damage)
        {
            currentShield -= damage;
        }

        if ((currentShield + _currentHealth) > _maxHealth)
            _healthBar.SetMaxHealth(_maxHealth, currentShield);
        else
            _healthBar.SetHealth(_currentHealth, currentShield);

        regenOn = false;
        HealthSetAtMax = false;

        StartCoroutine(coroutineCaller.VisualFeedbackDamage());

        _healthBar.SetHealth(_currentHealth, currentShield);

        if (_currentHealth <= 0)
        {
            //Debug.Log("DEAD");
            //restartMenu.LoadRestart();
            SceneManager.LoadScene("RestartScene");
            //RestarMenu.SetActive(true);

            //Time.timeScale = 0f;
        }
    }

    internal void RegenerateHealth(int health)
    {
        _currentHealth += health;

        StartCoroutine(coroutineCaller.VisualFeedbackHeal());

        _healthBar.SetMaxHealth(_maxHealth, currentShield);

        _healthBar.SetHealth(_currentHealth, currentShield);

        if (_currentHealth >= _maxHealth)
        {
            HealthSetAtMax = true;
            _currentHealth = _maxHealth;
            _healthBar.SetHealth(_currentHealth, currentShield);
        }

        coroutineCaller.VisualFeedbackHeal();
    }

    internal void GiveShield(int shieldAmount)
    {
        this.currentShield = shieldAmount;
        _healthBar.SetMaxHealth(_maxHealth, currentShield);
        _healthBar.SetHealth(_currentHealth, currentShield);

        StopCoroutine(DecayTimer());
        StartCoroutine(DecayTimer());
    }

    internal void Takehealth(int health)
    {
        _currentHealth += health;
        _healthBar.SetHealth(_currentHealth, currentShield);
    }

    private void Update()
    {

        switch (HealthSetAtMax)
        {
            case true:
                {
                    _playerHealthState = _PlayerHealth._Max;
                    break;
                }
            case false:
                {
                    _playerHealthState = _PlayerHealth.NotMax;
                    break;
                }
        }

        Cheats();
    }

    private void FixedUpdate()
    {

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
            _healthBar.SetMaxHealth(_maxHealth, currentShield);
            _healthBar.SetHealth(_currentHealth, currentShield);
        } 
        else
        {

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
