using System;
using UnityEngine;


public class CursorGame : MonoBehaviour
{
                        public enum CursorState                         { _NORMAL, _ATTACK }

    [SerializeField]    private CursorState                             _state;

    [SerializeField]    private Texture2D                               _imageNormal, _imageAttack;

                        public static CursorGame                        Instance;

                        public static event Action<CursorState>         OnCursorStateChanged;



    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        Instance                            = this;

        CursorGame[] OTHER_MANAGERS         = FindObjectsOfType<CursorGame>();



        for (int i = 1; i < OTHER_MANAGERS.Length; i++)
        {
            Destroy(OTHER_MANAGERS[i].gameObject);
        }

    }

    private void Start()
    {
        
        UpdateCursor(CursorState._NORMAL);
    }

    
    public void UpdateCursor(CursorState newState)
    {
        switch(newState) 
        {
            case CursorState._NORMAL:
                {
                    NormalCursor();
                    break;
                }

            case CursorState._ATTACK:
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

        Cursor.SetCursor(_imageNormal, Vector2.zero,CursorMode.ForceSoftware);
        return;
        print("cursor normal");
    }

    private void AttackCursor() 
    {

        //print("cursor Attack");

        Cursor.SetCursor(_imageAttack, Vector2.zero, CursorMode.ForceSoftware);
        return;

        print("cursor Attack");
    }

}
