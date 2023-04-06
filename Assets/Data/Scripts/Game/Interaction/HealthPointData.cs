using UnityEngine;

[CreateAssetMenu(menuName = "Akarya/Interaction/Health Point")]
public class HealthPointData : ScriptableObject
{
    [Header("Health")]
    [Range(0f, 100f)]
    [SerializeField] 
    private int _health;
    public int Health => _health;


    [Header("Little health config")]
    [SerializeField] private bool _canIgnoreMaxHealth;
    public bool CanIgnoreHealth => _canIgnoreMaxHealth;

    [SerializeField] 
    private bool _usesAudioAmbient;
    public bool UseAudioAmbient => _usesAudioAmbient;   

    [SerializeField] 
    private bool _canBeAttracted;
    public bool CanBeattracted => _canBeAttracted;
    [SerializeField]
    private bool _useStartForce;
    public bool UseStartForce => _useStartForce;    

    [SerializeField] 
    private int atractionSpeed = 2;
    public int AtractionSpeed => atractionSpeed;    

    [SerializeField]
    private float maxDistance = 1.2f;
    public float MaxDistance => maxDistance;    
}
