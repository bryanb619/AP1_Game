using UnityEngine;

public class SaveManager : MonoBehaviour
{
    #region Variables
    private float x, y, z;

    // FIND PLAYER IN SCENE
    private PlayerMovement PLAYER;

    // player transform
    private Transform _player;

    // save animation
    private Animator _saveAnimator;

    #endregion

    #region Start
    private void Start()
    {
        _saveAnimator = GetComponent<Animator>();

        PLAYER = FindObjectOfType<PlayerMovement>();
        _player = PLAYER.transform;
         
    }
    #endregion

    #region Save & Load

    public void Save()
    {
        GetSave(); 
    }

    /// <summary>
    /// Save function
    /// </summary>
    private void GetSave()
    {
        StartAnimation(); 

        x = _player.transform.position.x;
        y = _player.transform.position.y;
        z = _player.transform.position.z; 

        //return;
    }

    public void load()
    {
       GetLoad();
    }

    /// <summary>
    /// Load function
    /// </summary>
    private void GetLoad()
    {
        PlayerPrefs.GetFloat("x");
        PlayerPrefs.GetFloat("y");
        PlayerPrefs.GetFloat("z");

        Vector3 LoadPos = new Vector3(x, y, z);

        _player.transform.position = LoadPos;

        //return;
    }


    private void StartAnimation()
    {

    }

    #endregion
}
