using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Akarya/Interaction/SpecialDrop")]
public class SpecialDropData : ScriptableObject
{

    [FormerlySerializedAs("_dropType")] [SerializeField] 
    private DropType                dropType;
    internal DropType               Drop => dropType;


    [FormerlySerializedAs("_healthEmpower")]
    [Header("Health")]

        [Range(0f, 100f)]
        [SerializeField] 
        private int                 healthEmpower;
        internal int                Health => healthEmpower;

    // Mana //
    [FormerlySerializedAs("_manaEmpower")]
    [Header("Mana")]
    

        [Range(0f, 100f)]
        [SerializeField]
        private int                 manaEmpower;
        internal int                Mana => manaEmpower;


    [FormerlySerializedAs("_heightFloat")]
    [Header("Height")]
        [SerializeField]
            private float           heightFloat = 1f;
            internal float          HeightFloat => heightFloat;

    [FormerlySerializedAs("_float")]
    [Header("Float")]

        [SerializeField]
            private bool            @float;
            internal bool           Float => @float;


    [FormerlySerializedAs("_rotate")]
    [Header("Rotate")]
        [SerializeField]
            private bool            rotate;
            public bool             Rotate => rotate;

        [SerializeField]
            private float           rotateSpeed;
            internal float          RotateSpeed; 

        [FormerlySerializedAs("_canBeAttracted")] [SerializeField] 
        private bool                canBeAttracted;
        internal bool               CanBeattracted => canBeAttracted;
        [FormerlySerializedAs("_useStartForce")] [SerializeField]
        private bool                useStartForce;
        internal bool               UseStartForce => useStartForce;    

        [SerializeField] 
        private int                 atractionSpeed = 2;
        internal int                AtractionSpeed => atractionSpeed;    

        [SerializeField]
        private float               maxDistance = 3f;
        internal float              MaxDistance => maxDistance;


        [FormerlySerializedAs("_layerMask")] [SerializeField] 
        private LayerMask           layerMask;
        internal LayerMask          LayerMask => layerMask;

        [FormerlySerializedAs("_pushForce")] [SerializeField] 
        private float               pushForce;
        internal float              PushForce => pushForce;


        [FormerlySerializedAs("_useAudio")]
        [Header("Sound")]

        [SerializeField]
        private bool                useAudio;
        internal bool               UseAudio => useAudio;

        [FormerlySerializedAs("_usesAudioAmbient")] [SerializeField]
        private bool                usesAudioAmbient;
        internal bool               UseAudioAmbient => usesAudioAmbient;
}
