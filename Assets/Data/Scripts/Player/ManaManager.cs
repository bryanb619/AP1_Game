using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaManager : MonoBehaviour
{
    private int maxMana;
    [SerializeField] private int mana = 100;
    [SerializeField] private int fireAttackManaUsage;
    [SerializeField] private int iceAttackManaUsage;
    [SerializeField] private int thunderAttackManaUsage;
    private WeaponType _magicType;


    private void Start()
    {
        maxMana = mana;
    }

    private void Update()
    {
        if (mana > maxMana)
            mana = maxMana;

        if (mana < 0)
            mana = 0;
    }

    internal bool ManaCheck(WeaponType _magicType)
    {
        switch(_magicType)
        { 
            case WeaponType.Fire:
                if (mana > fireAttackManaUsage)
                { 
                    FireAttack();
                    return true;
                }
                else
                    return false;

            case WeaponType.Ice:
                if (mana > iceAttackManaUsage)
                {
                    IceAttack();
                    return true;
                }
                else
                    return false;

            case WeaponType.Thunder:
                if (mana > thunderAttackManaUsage)
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

    internal void FireAttack()
    {
        mana -= fireAttackManaUsage;
    }
    internal void IceAttack()
    {
        mana -= iceAttackManaUsage;
    }
    internal void ThunderAttack()
    {
        mana -= thunderAttackManaUsage;
    }

    internal void RecoverMana()
    {

    }

}
