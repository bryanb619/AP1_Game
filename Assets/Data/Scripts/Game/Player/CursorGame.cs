using System;
using UnityEngine;
using UnityEngine.Serialization;


public class CursorGame : MonoBehaviour
{
                        public enum CursorState                         { Normal, Attack }

    [FormerlySerializedAs("_state")] [SerializeField]    private CursorState                             state;

    [FormerlySerializedAs("_imageNormal")] [SerializeField]    private Texture2D                               imageNormal;
    [FormerlySerializedAs("_imageAttack")] [SerializeField]    private Texture2D                               imageAttack;

    public static CursorGame                        Instance;

                        public static event Action<CursorState>         OnCursorStateChanged;



    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        Instance                            = this;

        CursorGame[] otherManagers         = FindObjectsOfType<CursorGame>();



        for (int i = 1; i < otherManagers.Length; i++)
        {
            Destroy(otherManagers[i].gameObject);
        }

    }

    private void Start()
    {
        
        UpdateCursor(CursorState.Normal);
    }

    
    public void UpdateCursor(CursorState newState)
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
        
        //print("cursor normal");
    }

    private void AttackCursor() 
    {

        //print("cursor Attack");

        Cursor.SetCursor(imageAttack, Vector2.zero, CursorMode.ForceSoftware);
        return;
    }

}
