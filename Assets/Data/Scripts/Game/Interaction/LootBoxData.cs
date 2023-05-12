using FMODUnity;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu (menuName = "Akarya/Interaction/Loot Box")]
public class LootBoxData : ScriptableObject
{
    [FormerlySerializedAs("_loot")]
    [Header("Loot")]
        [SerializeField] 
            private GameObject                  loot;
            public GameObject                   Loot => loot;

        [FormerlySerializedAs("_openEffect")] [SerializeField] 
            private GameObject                  openEffect; 
            public GameObject                   OpenEffect => openEffect;

        [FormerlySerializedAs("_maxItems")] [SerializeField] 
            private int                         maxItems;
            public int                          MaxItems => maxItems;

        [FormerlySerializedAs("_dropRadius")] [SerializeField]
            private float                       dropRadius; 
            public float                        DropRadius => dropRadius;

        [FormerlySerializedAs("_giveLoot")]
        [Tooltip("check box if loot instead of spawned should be given directly to player")]
        [SerializeField]
            private bool                        giveLoot;
            public bool                         GiveLoot => giveLoot;

    [FormerlySerializedAs("_soundOpen")]
    [Header("Sound")]
        [SerializeField] 
            private EventReference              soundOpen;
            public EventReference               SoundOpen => soundOpen;  



    //[SerializeField]
}
