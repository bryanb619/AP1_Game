using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField]    private int playerMaxHealth;
    [SerializeField]    private int playerMaxMana;
    [SerializeField]    private int playerMaxShield;

    [Header("Cooldown")]
    [SerializeField]    private float dashCd;
    [SerializeField]    private float fireCd;
    [SerializeField]    private float iceCd;
    [SerializeField]    private float thunderCd;
    [SerializeField]    private float normalCd;
    
    [Header("Mana Cost")]
    [SerializeField]    private int fireManaCost;
    [SerializeField]    private int iceManaCost;
    [SerializeField]    private int thunderManaCost;
    
    [Header("Fire Damage")]
    [SerializeField]    private int fireDamageRanged;
    [SerializeField]    private int fireDamageChase;
    [SerializeField]    private int fireDamageBoss;
    
    [Header("Ice Damage")]
    [SerializeField]    private int iceDamageRanged;
    [SerializeField]    private int iceDamageChase;
    [SerializeField]    private int iceDamageBoss;
    
    [Header("Thunder Damage")]
    [SerializeField]    private int thunderDamageChase;
    [SerializeField]    private int thunderDamageRanged;
    [SerializeField]    private int thunderDamageBoss;
    
    [Header("Normal Damage")]
    [SerializeField]    private int normalDamageRanged;
    [SerializeField]    private int normalDamageChase;
    [SerializeField]    private int normalDamageBoss;
    
    [Header("Others")]
    [SerializeField]    private float areaAttackRadius;
    
    [Header("References")]
    [SerializeField]    private ProjectileData normalData;
    [SerializeField]    private ProjectileData fireData;
    [SerializeField]    private ProjectileData iceData;
    [SerializeField]    private ProjectileData thunderData;
    [SerializeField]    private EnemyProjectileData rangedProjectile1;
    [SerializeField]    private EnemyProjectileData rangedProjectile2;
                        private Dashing        dashingScript;
                        private Shooter        shooterScript;
                        private ManaManager    manaManagerScript;
                        private PlayerHealth   playerHealthScript;
    




    // Start is called before the first frame update
    void Start()
    {
        dashingScript = FindObjectOfType<Dashing>();
        shooterScript = FindObjectOfType<Shooter>();
        manaManagerScript = FindObjectOfType<ManaManager>();
        playerHealthScript = FindObjectOfType<PlayerHealth>();
        UpdateValues();
    }

    private void UpdateValues()
    {
        playerHealthScript.maxHealth = playerMaxHealth;
        manaManagerScript.maxMana = playerMaxMana;
        dashingScript.shieldAmount = playerMaxShield;

        dashingScript.dashCd = dashCd;
        shooterScript.fireTimer.cooldownTime = fireCd;
        shooterScript.iceTimer.cooldownTime = iceCd;
        shooterScript.thunderTimer.cooldownTime = thunderCd;
        shooterScript.normalTimer.cooldownTime = normalCd;
        
        fireData.enemyRangedDamage = fireDamageRanged;
        fireData.enemyChaseDamage = fireDamageChase;
        fireData.enemyBossDamage = fireDamageBoss;
        
        iceData.enemyRangedDamage = iceDamageRanged;
        iceData.enemyChaseDamage = iceDamageChase;
        iceData.enemyBossDamage = iceDamageBoss;

        thunderData.enemyRangedDamage = thunderDamageRanged;
        thunderData.enemyChaseDamage = thunderDamageChase;
        thunderData.enemyBossDamage = thunderDamageBoss;

        normalData.enemyRangedDamage = normalDamageRanged;   
        normalData.enemyChaseDamage = normalDamageChase;
        normalData.enemyBossDamage = normalDamageBoss;
        
        manaManagerScript.fireAttackCost = fireManaCost;
        manaManagerScript.iceAttackCost = iceManaCost;
        manaManagerScript.thunderAttackCost = thunderManaCost;
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateValues();
    }
}
