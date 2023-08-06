using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class food : MonoBehaviour
{
    public int foodAmount = 10;

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(0.2f + foodAmount*0.02f, 1, 0.2f + foodAmount * 0.02f);
        if (foodAmount <= 0)
        {
            Destroy(gameObject);
        }
    }
}
