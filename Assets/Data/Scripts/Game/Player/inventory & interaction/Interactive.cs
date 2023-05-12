using UnityEngine;
using UnityEngine.Serialization;

public class Interactive : MonoBehaviour
{
    public enum InteractiveType { Pickable, InteractOnce, InteractMulti, Indirect, TextInteract, GemClean, SceneChange };

    [FormerlySerializedAs("DEACTIVATE")] [SerializeField] private GameObject         deactivate;
    [FormerlySerializedAs("ACTIVATE")] [SerializeField] private GameObject         activate;
    [SerializeField] internal int                levelChosen;
    [FormerlySerializedAs("_isActive")] [SerializeField] private bool               isActive;
    [FormerlySerializedAs("_type")] [SerializeField] private InteractiveType    type;
    [FormerlySerializedAs("_icon")] [SerializeField] private Sprite             icon;
    [FormerlySerializedAs("_requirementText")] [SerializeField] private string             requirementText;
    [FormerlySerializedAs("_requirements")] [SerializeField] private Interactive[]      requirements;
    [FormerlySerializedAs("_activationChain")] [SerializeField] private Interactive[]      activationChain;
    [FormerlySerializedAs("_interactionTexts")] [SerializeField] private string[]           interactionTexts;
    [FormerlySerializedAs("_interactionChain")] [SerializeField] private Interactive[]      interactionChain;

    [Range(10,30)][SerializeField] private float radius = 20f;
    [FormerlySerializedAs("_canShowGizmo")] [SerializeField] private bool canShowGizmo = true;
    [FormerlySerializedAs("AILayer")] [SerializeField] private LayerMask aiLayer; 
    private bool _canSearch;
    private bool _isGemClean; 


    


    private Animator    _animator;
    private int         _curInteractionTextId;

    void Start()
    {
        _animator               = GetComponent<Animator>();
        _curInteractionTextId   = 0;
    }
    private void Update()
    {
        if (_canSearch) { GetAi(); }
        
    }

    public bool IsActive()
    {
        return isActive;
    }

    public InteractiveType GetInteractiveType()
    {
        return type;
    }

    public Sprite GetIcon()
    {
        return icon;
    }

    public string GetRequirementText()
    {
        return requirementText;
    }

    public string GetCurrentInteractionText()
    {
        return interactionTexts[_curInteractionTextId];
    }

    public Interactive[] GetRequirements()
    {
        return requirements;
    }

    private void Activate()
    {
        isActive = true;

        if (_animator != null)
            _animator.SetTrigger("Activate");
    }

    private void GetAi()
    {
        // Check if the player is within the warning radius (Target layer optional)
        Collider[] aiHits = Physics.OverlapSphere(transform.position, radius, aiLayer);
    
        if (aiHits.Length <= 0)
        {
            
            //print("interaction works");
            ActivateFunctions();

        }
    }

    public void Interact()
    {
        if (isActive)
        {
            if (_animator != null)
                _animator.SetTrigger("Interact");

            if (type == InteractiveType.Pickable)
            {
                GetComponent<Collider>().enabled = false;
                gameObject.SetActive(false); // 

            }
            else if (type == InteractiveType.InteractOnce)
                GetComponent<Collider>().enabled = false;
            else if (type == InteractiveType.InteractMulti)
                _curInteractionTextId = (_curInteractionTextId + 1) % interactionTexts.Length;

            else if(type == InteractiveType.TextInteract)
            {

            }
            else if(type == InteractiveType.GemClean) 
            {
                
                // Single interaction
                //GetComponent<Collider>().enabled = false;
                _canSearch = true; 

                
            }
            else if (type == InteractiveType.SceneChange)
            {
                gameObject.GetComponentInParent<SceneTransition>().FadeToLevel(levelChosen);
            }
            ProcessActivationChain();
            ProcessInteractionChain();
        }
    }

    private void ProcessActivationChain()
    {
        if (activationChain != null)
        {
            for (int i = 0; i < activationChain.Length; ++i)
                activationChain[i].Activate();
        }
    }

    private void ProcessInteractionChain()
    {
        if (interactionChain != null)
        {
            for (int i = 0; i < interactionChain.Length; ++i)
                interactionChain[i].Interact();
        }
    }

    private void ActivateFunctions()
    {
        // game object deactivate
        deactivate.SetActive(false);

        // game object activate
        activate.SetActive(true);

        Destroy(this.gameObject); 


    }

    private void OnDrawGizmos()
    {
        if(canShowGizmo)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position,
            radius);
        }
      
    }
    
}
