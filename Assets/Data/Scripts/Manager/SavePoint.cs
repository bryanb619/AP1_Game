using UnityEngine;

public class SavePoint : MonoBehaviour
{
    private SaveManager _saveManager;


    private void Start()
    {
        _saveManager = FindObjectOfType<SaveManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = GetComponent<PlayerMovement>();

        if (player != null)
        {
            _saveManager.Save();

            DestroySavePoint(); 

        }
    }

    private void DestroySavePoint()
    {
        Destroy(this.gameObject); 
    }
}
