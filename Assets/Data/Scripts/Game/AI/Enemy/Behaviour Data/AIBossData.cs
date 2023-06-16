using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName ="Akarya/AI/Boss Data")]
public class AiBossData : ScriptableObject
{
      
    [SerializeField] private int healhtPoint[], 
    public int HealthPoint(int index) => healhtPoint[index];

    
    [SerializeField] private int               
        chaseCount[], rangedCount[];  
     
    
    public int ChaseCount => chaseCount;
    public int RangedCount => rangedCount1;


}
