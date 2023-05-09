using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ValuesTextsScript : MonoBehaviour
{
    internal int kills, crystals, upgrades;
    
    [SerializeField]
    private Text killsText, crystalsText, upgradesText;

    void Start()
    {
        kills = 0;
        crystals = 0;
        upgrades = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        killsText.text = kills.ToString();
        crystalsText.text = crystals.ToString();
        upgradesText.text = upgrades.ToString();
    }



    internal void GetKill()
    {
        kills += 1;
    }
    internal void GetCrystal()
    {
        crystals += 1;
    }
    internal void GetUpgrade()
    {
        upgrades += 1;
    }
}
