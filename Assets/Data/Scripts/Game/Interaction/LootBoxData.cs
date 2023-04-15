using FMODUnity;
using UnityEngine;

[CreateAssetMenu (menuName = "Akarya/Interaction/Loot Box")]
public class LootBoxData : ScriptableObject
{
    [Header("Loot")]
        [SerializeField] 
            private GameObject                  _loot;
            public GameObject                   Loot => _loot;

        [SerializeField] 
            private GameObject                  _openEffect; 
            public GameObject                   OpenEffect => _openEffect;

        [SerializeField] 
            private int                         _maxItems;
            public int                          MaxItems => _maxItems;

        [SerializeField]
            private float                       _dropRadius; 
            public float                        DropRadius => _dropRadius;

        [Tooltip("check box if loot instead of spawned should be given directly to player")]
        [SerializeField]
            private bool                        _giveLoot;
            public bool                         GiveLoot => _giveLoot;

    [Header("Sound")]
        [SerializeField] 
            private EventReference              _soundOpen;
            public EventReference               SoundOpen => _soundOpen;  



    //[SerializeField]
}
