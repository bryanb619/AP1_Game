using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlick : MonoBehaviour
{
    [Tooltip("External light to flicker; you can leave this null if you attach script to a light")]
    public new Light light;
    [Tooltip("Minimum random light intensity")]
    public float minIntensity;
    [Tooltip("Maximum random light intensity")]
    public float maxIntensity;
    [Tooltip("How much to smooth out the randomness; lower values = sparks, higher = lantern")]
    [Range(1, 50)]
    public int smoothing;

    // Continuous average calculation via FIFO queue
    // Saves us iterating every time we update, we just change by the delta
    Queue<float> _smoothQueue;
    float _lastSum = 0;


    /// <summary>
    /// Reset the randomness and start again. You usually don't need to call
    /// this, deactivating/reactivating is usually fine but if you want a strict
    /// restart you can do.
    /// </summary>
    public void Reset()
    {
        _smoothQueue.Clear();
        _lastSum = 0;
    }

    void Start()
    {
        _smoothQueue = new Queue<float>(smoothing);
        // External or internal light?
        if (light == null)
        {
            light = GetComponent<Light>();
        }
    }

    void Update()
    {
        if (light == null)
            return;


        if(light != null)
        {
            // pop off an item if too big
            while (_smoothQueue.Count >= smoothing)
            {
                _lastSum -= _smoothQueue.Dequeue();
            }

            // Generate random new item, calculate new average
            float newVal = Random.Range(minIntensity, maxIntensity);
            _smoothQueue.Enqueue(newVal);
            _lastSum += newVal;

            // Calculate new smoothed average
            light.intensity = _lastSum / (float)_smoothQueue.Count;

            //Debug.Log("Light active");
        }
        
    }
}
