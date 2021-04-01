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

    [HideInInspector]                               // lista dei nemici individuati, pubblica cosi che l'editor può disegnarli
    public List<Transform> visibleTargets = new List<Transform>();

    private IAEnemy enemy;

    void Start()
    {
        enemy = this.gameObject.GetComponentInParent<IAEnemy>();
        StartCoroutine("FindTargetsWithDelay", .1f);
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);

            if (isPlayerVisible())
            {
                enemy.Warned(visibleTargets[0]);
            }
        }
    }

    /*
     Ogni frame vedo se ci sono nemici nel mio campo visivo
     */
    /*void Update()
    {
        FindVisibleTargets();
        
        if(isPlayerVisible())
        {
            enemy.Warned(visibleTargets[0]);
        }
    }*/

    /**
        Ogni volta che devo cercare nuovi nemici
        pulisco la lista dei nemici rilevati
     */
    public void FindVisibleTargets()                
    {
        visibleTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
        
        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                if (toggleSeeThroughObstacles || !Physics.Raycast(transform.position, dirToTarget, Vector3.Distance(transform.position, target.position), obstacleMask))
                {
                    visibleTargets.Add(target);
                }
            }
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public bool isPlayerVisible()
    {
        FindVisibleTargets();
        return visibleTargets.Count == 1;
    }
}
