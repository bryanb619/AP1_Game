using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityUpgradesManager : MonoBehaviour
{
    [SerializeField]
    private UpgradeType upgrade;
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
            case UpgradeType.qUpgrade:
                GetComponentInChildren<TextMeshProUGUI>().text = "Q Upgrade";
                break;
            case UpgradeType.wUpgrade:
                GetComponentInChildren<TextMeshProUGUI>().text = "W Upgrade";
                break;
            case UpgradeType.eUpgrade:
                GetComponentInChildren<TextMeshProUGUI>().text = "E Upgrade";
                break;
            case UpgradeType.rUpgrade:
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
                case UpgradeType.qUpgrade:
                    upgradeUI.transform.GetChild(0).gameObject.SetActive(true);
                    shooterScript.UpgradeChecker(1);
                    break;

                case UpgradeType.wUpgrade:
                    upgradeUI.transform.GetChild(1).gameObject.SetActive(true);
                    shooterScript.UpgradeChecker(2);                                 
                    break;                                                           
                                                                                     
                case UpgradeType.eUpgrade:
                    upgradeUI.transform.GetChild(2).gameObject.SetActive(true);
                    dashingScript.EnableUpgrade();                                   
                    break;                                                           
                                                                                     
                case UpgradeType.rUpgrade:
                    upgradeUI.transform.GetChild(3).gameObject.SetActive(true);
                    shooterScript.UpgradeChecker(3);
                    break;

                default:
                    break;
            }

            Destroy(gameObject);
        }
    }

    public enum UpgradeType
    {
        qUpgrade,
        wUpgrade,
        eUpgrade,
        rUpgrade
    }
}
