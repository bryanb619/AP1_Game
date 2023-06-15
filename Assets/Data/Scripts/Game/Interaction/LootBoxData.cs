using UnityEngine;
using FMODUnity;

[CreateAssetMenu (menuName = "Akarya/Interaction/Loot Box")]
public class LootBoxData : ScriptableObject
{

    [Header("Loot")]
        [SerializeField] 
            private GameObject                  loot;
            public GameObject                   Loot => loot; 
            
        [SerializeField] private DropType       dropType;
            public DropType                     DropType => dropType;
            

 [SerializeField] 
            private GameObject                  openEffect; 
            public GameObject                   OpenEffect => openEffect;

[SerializeField] 
            private int                         maxItems;
            public int                          MaxItems => maxItems;

    [SerializeField]
            private float                       dropRadius; 
            public float                        DropRadius => dropRadius;
            
        [Tooltip("check box if loot instead of spawned should be given directly to player")]
        [SerializeField]
            private bool                        giveLoot;
            public bool                         GiveLoot => giveLoot;

            [SerializeField] private int empower; 
            public int Empower => empower;
            
    [Header("Sound")]
        [SerializeField] 
            private EventReference              soundOpen;
            public EventReference               SoundOpen => soundOpen;  
            
}
