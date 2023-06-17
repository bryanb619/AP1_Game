using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ManaManager : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] public int maxMana = 100;
    [SerializeField] private float mana;

    [Header("Ability Mana Cost")]
    [SerializeField] public int fireAttackCost;
    [SerializeField] public int iceAttackCost;
    [SerializeField] public int thunderAttackCost;
    
    [Header ("Regeneration")]
    [SerializeField] private float manaRegenRate = 1f;
    [SerializeField] private float manaRegenAmount = 10f;
    [SerializeField] private float waitTime = 2f;
                     private WaitForSeconds _manaTimer;

                     private bool canUseFireAttack;
                     private bool canUseIceAttack;
                     private bool canUseThunderAttack;
                     
                     
    [Header("References")]
    private ManaBar manaUi;

    [Header("Cheats")]
    [SerializeField] private bool manaCheat;


    private void Start()
    {
        manaUi = FindObjectOfType<ManaBar>();
        _manaTimer = new WaitForSeconds(waitTime);
        manaUi.SetMaxMana(maxMana);
    }

    private void Update()
    {
        mana = Mathf.Clamp(mana, 0f, maxMana);
        
        if (mana == maxMana)
            CancelInvoke("ManaRegeneration");

        if (manaCheat == true)
        {
            mana = maxMana;
            manaUi.SetMana(mana);
        }
    }

    #region Mana Management

    internal bool ManaCheck(WeaponType magicType)
    {
        CancelInvoke("ManaRegeneration");

        switch (magicType)
        { 
            case WeaponType.Fire:
                if (mana >= fireAttackCost)
                { 
                    return true;
                }
                else
                    return false;

            case WeaponType.Ice:
                if (mana >= iceAttackCost)
                {
                    return true;
                }
                else
                    return false;

            case WeaponType.Thunder:
                if (mana >= thunderAttackCost)
                { 
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
        manaUi.SetMana(mana);
    }
    internal void RecoverMana(int recoverable)
    {
        mana += recoverable;
        manaUi.SetMana(mana);
    }

    public void ManaIncrease(int add)
    {
        maxMana += add; 
        manaUi.SetNewMax(add);
    }

    #endregion

    #region Abilities

    internal void FireAttack()
    {
        mana -= fireAttackCost;
        manaUi.SetMana(mana);

        StopCoroutine(RegenerationTimer());
        StartCoroutine(RegenerationTimer());
    }
    internal void IceAttack()
    {
        mana -= iceAttackCost;
        manaUi.SetMana(mana);

        StopCoroutine(RegenerationTimer());
        StartCoroutine(RegenerationTimer());
    }
    internal void ThunderAttack()
    {
        mana -= thunderAttackCost;
        manaUi.SetMana(mana);

        StopCoroutine(RegenerationTimer());
        StartCoroutine(RegenerationTimer());
    }

    #endregion

    
}
