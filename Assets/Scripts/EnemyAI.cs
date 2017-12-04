using System.Collections;
using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Seeker))]
public class EnemyAI : MonoBehaviour {

    public Transform target;

    public float updateRate = 2f;

    private Seeker seeker;
    private Rigidbody2D rb;

    //calculated path
    public Path path;

    //AIs speed
    public float speed = 300f;
    public ForceMode2D fMode;

    [HideInInspector]
    public bool pathIsEnded = false;

    //max distance from AI to point to continue
    public float nextWaypointDistance = 3;

    private int currentWaypoint = 0;

    private bool searchingPlayer = false;

    private void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        if(target == null)
        {
            if (!searchingPlayer)
            {
                searchingPlayer = true;
                StartCoroutine(SearchForPlayer());
            }
            return;
        }

        seeker.StartPath(transform.position, target.position, OnPathComplete);

        StartCoroutine(UpdatePath());
    }

    IEnumerator SearchForPlayer()
    {
       GameObject sResult = GameObject.FindGameObjectWithTag("Player");
       if(sResult == null)
        {
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(SearchForPlayer());
        }
        else
        {
            searchingPlayer = false;
            target = sResult.transform;
            StartCoroutine(UpdatePath());
            yield return false;
        }
    }

    IEnumerator UpdatePath()
    {
        if (target == null)
        {
            if (!searchingPlayer)
            {
                searchingPlayer = true;
                StartCoroutine(SearchForPlayer());
            }
            yield return false;
        }

        seeker.StartPath(transform.position, target.position, OnPathComplete);

        yield return new WaitForSeconds(1f / updateRate);
        StartCoroutine(UpdatePath());
    }

    public void OnPathComplete(Path p)
    {
        Debug.Log("Path found");
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    private void FixedUpdate()
    {
        if (target == null)
        {
            if (!searchingPlayer)
            {
                searchingPlayer = true;
                StartCoroutine(SearchForPlayer());
            }
            return;
        }

        if (path == null)
            return;

        if(currentWaypoint >= path.vectorPath.Count)
        {
            if (pathIsEnded)
                return;

            Debug.Log("Path has ended");
            pathIsEnded = true;
            return;
        }

        pathIsEnded = false;

        Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        dir *= speed * Time.fixedDeltaTime;

        rb.AddForce(dir, fMode);

        float dist = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);

        if (dist < nextWaypointDistance)
        {
            currentWaypoint++;
            return;
        }
    }
}
