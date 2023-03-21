using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaManager : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] private float maxMana = 100;
    [SerializeField] private float mana;

    [Header("Ability Mana Cost")]
    [SerializeField] private int fireAttackCost;
    [SerializeField] private int iceAttackCost;
    [SerializeField] private int thunderAttackCost;
    
    [Header ("Regeneration")]
    [SerializeField] private float manaRegenRate = 1f;
    [SerializeField] private float manaRegenAmount = 10f;
    [SerializeField] private float waitTime = 2f;
                     private WaitForSeconds manaTimer;
    
    [Header("References")]
    [SerializeField] private ManaBar manaUI;
    [SerializeField] private Shooter shooter;

    [Header("Cheats")]
    [SerializeField] private bool manaCheat;


    private void Start()
    {
        manaTimer = new WaitForSeconds(waitTime);
    }

    private void Update()
    {
        mana = Mathf.Clamp(mana, 0f, maxMana);
        
        if (mana == maxMana)
            CancelInvoke("ManaRegeneration");

        if (manaCheat == true)
        {
            mana = maxMana;
            manaUI.SetMana(mana);
        }
    }

    #region Mana Management

    internal bool ManaCheck(WeaponType _magicType)
    {
        CancelInvoke("ManaRegeneration");

        switch (_magicType)
        { 
            case WeaponType.Fire:
                if (mana >= fireAttackCost)
                { 
                    FireAttack();
                    return true;
                }
                else
                    return false;

            case WeaponType.Ice:
                if (mana >= iceAttackCost)
                {
                    IceAttack();
                    return true;
                }
                else
                    return false;

            case WeaponType.Thunder:
                if (mana >= thunderAttackCost)
                { 
                    ThunderAttack();
                    return true;
                }
                else
                    return false;

            default:
                return false;
        }
    }

    IEnumerator RegenerationTimer()
    {
        yield return new WaitForSecondsRealtime(waitTime);
        InvokeRepeating("ManaRegeneration", 0f, manaRegenRate);
    }

    private void ManaRegeneration()
    {
        mana += manaRegenAmount;
        manaUI.SetMana(mana);
    }
    internal void RecoverMana(int recoverable)
    {
        mana += recoverable;
        manaUI.SetMana(mana);
    }

    #endregion

    #region Abilities

    internal void FireAttack()
    {
        mana -= fireAttackCost;
        manaUI.SetMana(mana);

        StopCoroutine(RegenerationTimer());
        StartCoroutine(RegenerationTimer());
    }
    internal void IceAttack()
    {
        mana -= iceAttackCost;
        manaUI.SetMana(mana);

        StopCoroutine(RegenerationTimer());
        StartCoroutine(RegenerationTimer());
    }
    internal void ThunderAttack()
    {
        mana -= thunderAttackCost;
        manaUI.SetMana(mana);

        StopCoroutine(RegenerationTimer());
        StartCoroutine(RegenerationTimer());
    }

    #endregion

    
}
