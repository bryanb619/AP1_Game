using System.Collections;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    private SaveManager         _saveManager;

    private BoxCollider         _boxCollider;


    private void Start()
    {
        _boxCollider            = GetComponent<BoxCollider>();

        _saveManager            = FindObjectOfType<SaveManager>();


        _boxCollider.enabled    = true;

    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement player       = GetComponent<PlayerMovement>();

        if (player != null)
        {
            _saveManager.Save();

            
            _boxCollider.enabled    = false;
        }
    }

    private IEnumerator StartDestroy()
    {
        bool isRunning = false;

        if(!isRunning) 
        {
            isRunning = true;

            yield return new WaitForSeconds(3.0f);

            DestroySavePoint();

            isRunning = false;
        }

        

    }

    private void DestroySavePoint()
    {
        Destroy(this.gameObject); 
    }
}
