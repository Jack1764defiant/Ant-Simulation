using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nest : MonoBehaviour
{
    public GameObject ant;
    public int antCount = 10;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < antCount; i++)
        {
            var a = Instantiate(ant, transform.position, transform.rotation);
            a.transform.Rotate(new Vector3(0, Random.Range(0, 360), 0));
            a.transform.Translate(a.transform.forward * 0.1f);
            a.GetComponent<ant>().nest = transform;
        }
    }
}
