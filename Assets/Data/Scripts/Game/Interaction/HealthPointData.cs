using UnityEngine;

[CreateAssetMenu(menuName = "Akarya/Interaction/Health Point")]
public class HealthPointData : ScriptableObject
{
    [Header("Health")]

        [Range(0f, 100f)]
        [SerializeField] 
        private int                 _health;
        public int                  Health => _health;

    [Header("Float")]

        [SerializeField] 
        private bool                _canIgnoreMaxHealth;
        public bool                 CanIgnoreHealth => _canIgnoreMaxHealth;

    [Header("Float")]

        [SerializeField]
        private bool                _float;
        public bool                 Float => _float;

        [SerializeField]
        private float               _heightFloat = 4f; 
        public float                HeightFloat => _heightFloat;

        [SerializeField] 
        private bool                _canBeAttracted;
        public bool                 CanBeattracted => _canBeAttracted;
        [SerializeField]
        private bool                _useStartForce;
        public bool                 UseStartForce => _useStartForce;    

        [SerializeField] 
        private int                 atractionSpeed = 2;
        public int                  AtractionSpeed => atractionSpeed;    

        [SerializeField]
        private float               maxDistance = 3f;
        public float                MaxDistance => maxDistance;


        [SerializeField] 
        private LayerMask           _layerMask;
        public LayerMask            LayerMask => _layerMask;

        [SerializeField] 
        private float               _pushForce;
        public float                PushForce => _pushForce;


        [Header("Sound")]

        [SerializeField]
        private bool                _useAudio; 
        public bool                 UseAudio => _useAudio;

        [SerializeField]
        private bool                _usesAudioAmbient;
        public bool                 UseAudioAmbient => _usesAudioAmbient;
}
