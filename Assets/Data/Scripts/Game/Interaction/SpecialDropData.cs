using UnityEngine;

[CreateAssetMenu(menuName = "Akarya/Interaction/SpecialDrop")]
public class SpecialDropData : ScriptableObject
{

    [SerializeField] 
    private DropType                _dropType;
    internal DropType               Drop => _dropType;


    [Header("Health")]

        [Range(0f, 100f)]
        [SerializeField] 
        private int                 _healthEmpower;
        internal int                Health => _healthEmpower;

    // Mana //
    [Header("Mana")]
    

        [Range(0f, 100f)]
        [SerializeField]
        private int                 _manaEmpower;
        internal int                Mana => _manaEmpower;


    [Header("Float")]

    [Header("Float")]

        [SerializeField]
        private bool                _float;
        internal bool               Float => _float;

        [SerializeField]
        private float               _heightFloat = 1f;
        internal float              HeightFloat => _heightFloat;

        [SerializeField] 
        private bool                _canBeAttracted;
        internal bool               CanBeattracted => _canBeAttracted;
        [SerializeField]
        private bool                _useStartForce;
        internal bool               UseStartForce => _useStartForce;    

        [SerializeField] 
        private int                 atractionSpeed = 2;
        internal int                AtractionSpeed => atractionSpeed;    

        [SerializeField]
        private float               maxDistance = 3f;
        internal float              MaxDistance => maxDistance;


        [SerializeField] 
        private LayerMask           _layerMask;
        internal LayerMask          LayerMask => _layerMask;

        [SerializeField] 
        private float               _pushForce;
        internal float              PushForce => _pushForce;


        [Header("Sound")]

        [SerializeField]
        private bool                _useAudio;
        internal bool               UseAudio => _useAudio;

        [SerializeField]
        private bool                _usesAudioAmbient;
        internal bool               UseAudioAmbient => _usesAudioAmbient;
}
