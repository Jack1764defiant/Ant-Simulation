using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ant : MonoBehaviour
{
    public float speed;
    bool hasFood = false;
    bool hasTrail = false;
    [HideInInspector]public Transform nest;
    public GameObject trailMaster;
    public GameObject trailComponent;
    public GameObject baseTrailMaster;
    public GameObject baseTrailComponent;
    private GameObject currentlyDeployingTrail;
    private GameObject currentlyDeployingBaseTrail;
    private PheromoneTrailMaster currentlyAcquiredTrail;
    private GameObject nextTrailPoint;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(randomRotation());
        StartCoroutine(deployTrail());
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
        //transform.Translate(transform.forward * speed * Time.deltaTime);
        if (hasFood)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
        
    }

    IEnumerator randomRotation()
    {
        while (true)
        {
            if (!hasFood && !hasTrail)
            {
                yield return new WaitForSeconds(Random.Range(0.2f, 0.75f));
                transform.Rotate(new Vector3(0, Random.Range(0, 360), 0));
            }
            else if (hasFood)
            {
                TurnToFace(nest.position);
                //transform.rotation = new Vector3(0, transform.rotation.y, 0);
                yield return null;
            }
            else if (hasTrail)
            {
                if (nextTrailPoint == null)
                {
                    hasTrail = false;
                    currentlyAcquiredTrail = null;
                    continue;
                }
                TurnToFace(nextTrailPoint.transform.position);
                yield return null;
            }
            else
            {
                yield return null;
            }
        }
    }

    IEnumerator deployTrail()
    {
        while (true)
        {
            if (hasFood)
            {
                currentlyDeployingBaseTrail = null;
                var t = Instantiate(trailComponent, transform.position, transform.rotation, currentlyDeployingTrail.transform);
                t.GetComponent<TrailComponent>().master = currentlyDeployingTrail.GetComponent<PheromoneTrailMaster>();
                t.GetComponent<TrailComponent>().index = currentlyDeployingTrail.GetComponent<PheromoneTrailMaster>().trailComponents.Count;
                currentlyDeployingTrail.GetComponent<PheromoneTrailMaster>().trailComponents.Add(t.GetComponent<TrailComponent>());
                yield return new WaitForSeconds(0.2f);
            }
            else
            {
                if (currentlyDeployingBaseTrail == null)
                {
                    currentlyDeployingBaseTrail = Instantiate(baseTrailMaster, transform.position, transform.rotation);
                }
                var t = Instantiate(baseTrailComponent, transform.position, transform.rotation, currentlyDeployingBaseTrail.transform);
                t.GetComponent<TrailComponent>().master = currentlyDeployingBaseTrail.GetComponent<PheromoneTrailMaster>();
                t.GetComponent<TrailComponent>().index = currentlyDeployingBaseTrail.GetComponent<PheromoneTrailMaster>().trailComponents.Count;
                currentlyDeployingBaseTrail.GetComponent<PheromoneTrailMaster>().trailComponents.Add(t.GetComponent<TrailComponent>());
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    void OnColliderEnter(Collider other)
    {
        if (other.gameObject.tag == "Ant" && !hasFood)
        {
            transform.Rotate(new Vector3(0, Random.Range(90, 270), 0));
        }
        else if (other.gameObject.tag == "Obstacle")
        {
            transform.Rotate(new Vector3(0, Random.Range(90, 270), 0));
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Food" && !hasFood)
        {
            hasFood = true;
            //hasTrail = false;
            other.GetComponent<food>().foodAmount--;
            currentlyDeployingTrail = Instantiate(trailMaster, transform.position, transform.rotation);
            var t = Instantiate(trailComponent, other.transform.position, transform.rotation, currentlyDeployingTrail.transform);
            t.GetComponent<TrailComponent>().master = currentlyDeployingTrail.GetComponent<PheromoneTrailMaster>();
            t.GetComponent<TrailComponent>().index = currentlyDeployingTrail.GetComponent<PheromoneTrailMaster>().trailComponents.Count;
            currentlyDeployingTrail.GetComponent<PheromoneTrailMaster>().trailComponents.Add(t.GetComponent<TrailComponent>());
        }
        else if (other.gameObject.tag == "Nest" && hasFood)
        {
            hasFood = false;
        }
        else if (other.gameObject.tag == "TrailComponent" && !hasFood)
        {
            if (!hasTrail)
            {
                Debug.Log("Found Trail");
                hasTrail = true;
                currentlyAcquiredTrail = other.gameObject.GetComponent<TrailComponent>().master;
                if (other.gameObject.GetComponent<TrailComponent>().index-1 >= 0)
                    nextTrailPoint = currentlyAcquiredTrail.trailComponents[other.gameObject.GetComponent<TrailComponent>().index-1].gameObject;
                else
                {
                    hasTrail = false;
                }
            }
            else if (other.gameObject.GetComponent<TrailComponent>().master == currentlyAcquiredTrail)
            {
                if (other.gameObject.GetComponent<TrailComponent>().index-1 >= 0)
                    nextTrailPoint = currentlyAcquiredTrail.trailComponents[other.gameObject.GetComponent<TrailComponent>().index-1].gameObject;
                else
                {
                    hasTrail = false;
                }
            }
        }
    }

    void TurnToFace(Vector3 lookTarget)
    {
        Vector3 dirToLookTarget = (lookTarget - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2(dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;

        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f)
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, 20);
            transform.eulerAngles = Vector3.up * angle;
        }
    }
}
