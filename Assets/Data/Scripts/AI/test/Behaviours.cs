using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Behaviours : ScriptableObject
{
   public abstract void Act(Brain controller);
}
