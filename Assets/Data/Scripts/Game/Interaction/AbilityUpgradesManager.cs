using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityUpgradesManager : MonoBehaviour
{
    [SerializeField]
    private upgradeType upgrade;
    [SerializeField]
    private GameObject upgradeUI;

    private Shooter shooterScript;
    private Dashing dashingScript;


    // Start is called at the beginning
    void Awake()
    {
        UpgradeText();
        shooterScript = GameObject.FindWithTag("Player").GetComponent<Shooter>();
        dashingScript = GameObject.FindWithTag("Player").GetComponent<Dashing>();


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpgradeText()
    {
        switch(upgrade)
        { 
            case upgradeType.qUpgrade:
                GetComponentInChildren<TextMeshProUGUI>().text = "Q Upgrade";
                break;
            case upgradeType.wUpgrade:
                GetComponentInChildren<TextMeshProUGUI>().text = "W Upgrade";
                break;
            case upgradeType.eUpgrade:
                GetComponentInChildren<TextMeshProUGUI>().text = "E Upgrade";
                break;
            case upgradeType.rUpgrade:
                GetComponentInChildren<TextMeshProUGUI>().text = "R Upgrade";
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            switch (upgrade)
            {
                case upgradeType.qUpgrade:
                    upgradeUI.transform.GetChild(0).gameObject.SetActive(true);
                    shooterScript.UpgradeChecker(1);
                    break;

                case upgradeType.wUpgrade:
                    upgradeUI.transform.GetChild(1).gameObject.SetActive(true);
                    shooterScript.UpgradeChecker(2);                                 
                    break;                                                           
                                                                                     
                case upgradeType.eUpgrade:
                    upgradeUI.transform.GetChild(2).gameObject.SetActive(true);
                    dashingScript.EnableUpgrade();                                   
                    break;                                                           
                                                                                     
                case upgradeType.rUpgrade:
                    upgradeUI.transform.GetChild(3).gameObject.SetActive(true);
                    shooterScript.UpgradeChecker(3);
                    break;

                default:
                    break;
            }

            Destroy(gameObject);
        }
    }

    public enum upgradeType
    {
        qUpgrade,
        wUpgrade,
        eUpgrade,
        rUpgrade
    }
}
