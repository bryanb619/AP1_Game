using UnityEngine;

public class Ability : ScriptableObject
{
    // ability name
    public new string name;

    // cooldown time
    public float cooldownTime;

    // Activate time
    public float activeTime;

    // ability void
    public virtual void Activate(GameObject parent)
    { }

    public virtual void BeginCooldown(GameObject parent)
    { }

  
}