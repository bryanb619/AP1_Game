//using System;
using UnityEngine; 

namespace Data.Scripts.Game.AI.Enemy
{
    public class AiShoot : MonoBehaviour
    {
        [SerializeField] private AiRangedData data;
        
        [SerializeField] private Transform shootPosition;
        private float _accuracy;
        private float _shootSpread;
        
        private void Awake()
        {
            _accuracy       = data.Accuracy;
            _shootSpread    = data.ShootSpread;
            
        }
        
        public void Shoot(Transform target, GameObject spell)
        {
            
            var aiPosition              = transform.position;
            var aiDirection             = (target.position - aiPosition).normalized;
            var aiRotation            = Quaternion.LookRotation(aiDirection);
            
            var randomSpread        = Quaternion.Euler
                (Random.Range(-_shootSpread, _shootSpread), Random.Range(-_shootSpread,_shootSpread), 0);
            
            var finalRotation       = Quaternion.RotateTowards
                (aiRotation, randomSpread, Random.Range(0.0f, 1.0f) * (1 - _accuracy) * 180);
            
            ShootObject(spell, finalRotation);
        }

        private void ShootObject(GameObject spell, Quaternion finalRotation)
        {
            Instantiate(spell , shootPosition.position, finalRotation);
        }
        
        
    }
}