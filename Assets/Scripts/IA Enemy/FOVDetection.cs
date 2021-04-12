using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVDetection : MonoBehaviour
{
    public bool toggleSeeThroughObstacles;
    [Range(0,360)]      public float viewAngle;     // angolo del fov
    public float viewRadius;                        // raggio del fov
    public LayerMask targetMask;                    // i layer che indica come nemici
    public LayerMask obstacleMask;                  // i layer che indica come ostacoli
                                                    
    private IAEnemy enemy;                          // script dell'IA del nemico
    private bool playerVisible = false;
    private GameObject player;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        enemy = GetComponent<IAEnemy>();
        StartCoroutine("FindTargetsWithDelay", .1f);
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);

            if (isPlayerVisible())
            {
                enemy.SetWarned();
            }
        }
    }

    /*
     Ogni frame vedo se ci sono nemici nel mio campo visivo
     */
    /*void Update()
    {
        
        if(isPlayerVisible())
        {
            enemy.Warned();
        }
    }*/

    /**
        Ogni volta che devo cercare nuovi nemici
        pulisco la lista dei nemici rilevati
     */
    public bool isPlayerVisible()                
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
        
        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                if (toggleSeeThroughObstacles || !Physics.Raycast(transform.position, dirToTarget, distanceTo(target), obstacleMask))
                {
                    return playerVisible = true;
                }
            }
        }
        return playerVisible = false;
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public float distanceTo(Transform target)
    {
        return Vector3.Distance(transform.position, target.position);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 fovLine1 = Quaternion.AngleAxis(viewAngle/2, transform.up) * transform.forward * viewRadius;
        Vector3 fovLine2 = Quaternion.AngleAxis(-viewAngle/2, transform.up) * transform.forward * viewRadius;

        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, fovLine1);
        Gizmos.DrawRay(transform.position, fovLine2);

        if (player && playerVisible)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, (player.transform.position - transform.position).normalized * viewRadius);
        }
    }
}
