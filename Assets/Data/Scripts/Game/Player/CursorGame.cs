using System;
using UnityEngine;

public class CursorGame : MonoBehaviour
{
    
    [Header("Cursor State")]
    [SerializeField]    private CursorState                             state;
                        private enum CursorState                        { Normal, Attack }
    
    [SerializeField]    private Texture2D                               imageNormal;
    [SerializeField]    private Texture2D                               imageAttack;

                        public static CursorGame                        Instance;
                        private static event Action<CursorState>         OnCursorStateChanged;
                        
                        
                        private Camera                                  _mainCamera;
    [Header("Layer")]
    [SerializeField]    private LayerMask                               attackMask, tooltipMask;
                        private bool                                    _isAttackCursor;
                        
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        Instance                                = this;
        var otherManagers            = FindObjectsOfType<CursorGame>();
        _mainCamera                             = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>(); 
        
        for (var i = 1; i < otherManagers.Length; i++)
        {
            Destroy(otherManagers[i].gameObject);
        }
        
        UpdateCursor(CursorState.Normal);
    }
    
    private void Update()
    {
        CastHandler(); 
    }
    
    private void CastHandler()
    {
        if (_mainCamera == null) return;
        
        var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        
        _isAttackCursor = Physics.Raycast(ray, out hit, 100f, attackMask);

        switch (_isAttackCursor)
        {
            case true:
            {
                UpdateCursor(CursorState.Attack);
                break;
            }
            case false:
            {
                UpdateCursor(CursorState.Normal);
                break;
            }
        }

        if (Physics.Raycast(ray, out hit, 100f, tooltipMask))
        {
            
        }

    }

    private void UpdateCursor(CursorState newState)
    {
        switch(newState) 
        {
            case CursorState.Normal:
                {
                    NormalCursor();
                    break;
                }

            case CursorState.Attack:
                {
                    AttackCursor();
                    break;
                }

            default: { throw new ArgumentOutOfRangeException(nameof(newState), newState, null); }
        }
        OnCursorStateChanged?.Invoke(newState);
    }

    private void NormalCursor()
    {
        //print("cursor normal");
        Cursor.SetCursor(imageNormal, Vector2.zero,CursorMode.ForceSoftware);
    }
    private void AttackCursor() 
    {
        //print("cursor Attack");
        Cursor.SetCursor(imageAttack, Vector2.zero, CursorMode.ForceSoftware);
    }
}
