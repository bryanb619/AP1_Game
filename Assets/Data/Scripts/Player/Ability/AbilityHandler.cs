using Unity.VisualScripting;
using UnityEngine;

public class AbilityHandler : MonoBehaviour
{

    [SerializeField] private GameObject Normal, Ice, Fire, Thunder;

    private void Start()
    {
        Normal.SetActive(true);
        Ice.SetActive(false);
        Fire.SetActive(false);
        Thunder.SetActive(false);
    }
    private void Update()
    {
        DeteckKey();
    }

    private void DeteckKey()
    {
       if(Input.GetKeyUp(KeyCode.Alpha1))
       {
            Normal.SetActive(true);
            Ice.SetActive(false);
            Fire.SetActive(false);
            Thunder.SetActive(false);
       }
       if (Input.GetKeyUp(KeyCode.Alpha2))
       {
            Normal.SetActive(false);
            Ice.SetActive(true);
            Fire.SetActive(false);
            Thunder.SetActive(false);
       }
       if (Input.GetKeyUp(KeyCode.Alpha3))
       {
            Normal.SetActive(false);
            Ice.SetActive(false);
            Fire.SetActive(true);
            Thunder.SetActive(false);
       }
       if (Input.GetKeyUp(KeyCode.Alpha4))
       {
            Normal.SetActive(false);
            Ice.SetActive(false);
            Fire.SetActive(false);
            Thunder.SetActive(true);
       }

    }
}
