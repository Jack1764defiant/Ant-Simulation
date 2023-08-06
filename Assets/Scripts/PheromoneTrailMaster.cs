using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PheromoneTrailMaster : MonoBehaviour
{
    [HideInInspector]public List<TrailComponent> trailComponents = new List<TrailComponent>();
    public float lifeTime = 5f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}
