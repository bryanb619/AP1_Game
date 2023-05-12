using UnityEngine;

public class SaveManager : MonoBehaviour
{
    #region Variables
    private float _x, _y, _z;

    // FIND PLAYER IN SCENE
    private PlayerMovement _playerMovement;

    // player transform
    private Transform _playerTransform;

    // save animation
    private Animator _saveAnimator;

    #endregion

    #region Start
    private void Start()
    {
        _saveAnimator = GetComponent<Animator>();

        PlayerMovement player = FindObjectOfType<PlayerMovement>();
        _playerTransform = player.transform;
         
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

        _x = _playerMovement.transform.position.x;
        _y = _playerMovement.transform.position.y;
        _z = _playerMovement.transform.position.z; 

        //return;
    }

    public void Load()
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

        Vector3 loadPos = new Vector3(_x, _y, _z);

        _playerMovement.transform.position = loadPos;

        //return;
    }


    private void StartAnimation()
    {

    }

    #endregion
}
