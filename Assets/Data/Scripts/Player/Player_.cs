using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_ : StateBehaviour<PlayerState>
{
    // Start is called before the first frame update
    void Start()
    {
        ChangeState(PlayerState.Idle);
    }
    

   
}
