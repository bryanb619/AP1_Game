using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Akarya/Interaction/Drop")]
public class DropData : ScriptableObject
{

    [FormerlySerializedAs("_dropType")] [SerializeField] 
    private DropType                dropType;
    internal DropType               Drop => dropType;


    [FormerlySerializedAs("_health")]
    [Header("Health")]

        [Range(0f, 100f)]
        [SerializeField] 
        private int                 health;
        internal int                Health => health;

    // Mana //
    [FormerlySerializedAs("_mana")]
    [Header("Mana")]
    

        [Range(0f, 100f)]
        [SerializeField]
        private int                 mana;
        internal int                Mana => mana;


    [FormerlySerializedAs("_canIgnoreMaxHealth")]
    [Header("Float")]

        [SerializeField] 
        private bool                canIgnoreMaxHealth;
        internal bool               CanIgnoreHealth => canIgnoreMaxHealth;


    [FormerlySerializedAs("_float")]
    [Header("Float")]

        [SerializeField]
        private bool                @float;
        internal bool               Float => @float;

        [FormerlySerializedAs("_heightFloat")] [SerializeField]
        private float               heightFloat = 4f;
        internal float              HeightFloat => heightFloat;

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
