using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ValuesTextsScript : MonoBehaviour
{
    internal int Kills, Crystals, Upgrades;
    
    [SerializeField]
    private Text killsText, crystalsText, upgradesText;

    void Start()
    {
        Kills = 0;
        Crystals = 0;
        Upgrades = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        killsText.text = Kills.ToString();
        crystalsText.text = Crystals.ToString();
        upgradesText.text = Upgrades.ToString();
    }



    internal void GetKill()
    {
        Kills += 1;
    }
    internal void GetCrystal()
    {
        Crystals += 1;
    }
    internal void GetUpgrade()
    {
        Upgrades += 1;
    }
}
