using UnityEngine;

/*
 *  Class: FOVDetection
 *  
 *  Description:
 *  This script check if the player (or the gameobject with targetMask) is in the enemy's field of view.
 *  
 *  Author: Thomas Voce
*/
public class FOVDetection : MonoBehaviour
{
    [Tooltip("Toggle See Through Obstacles")]
    public bool toggleSeeThroughObstacles;

    [Tooltip("Set Angle of View")]
    [Range(0,360)]      
    public float viewAngle = 120f;

    [Tooltip("Set Radius of View")]
    [Range(1, 30)] 
    public float viewRadius = 5.5f;

    // Denote the gameobject's layer that it have to detect (Player)
    private LayerMask targetMask;

    // Denote the gameobject layer that should be detected between it and the gameobject target (Obstacles: Default)
    private LayerMask obstacleMask;

    // Indicates that the player is actually visible
    public bool isPlayerVisible { get; private set; } = false;

    private PlayerStatistics player;
    public Vector3 lastPlayerPositionKnown { get;  private set; }

    private float originVA, originVR;

    void Start()
    {
        player = PlayerStatistics.instance;
        targetMask = LayerMask.GetMask("Player");
        obstacleMask = LayerMask.GetMask("Static"); //Default
        originVA = viewAngle;
        originVR = viewRadius;
    }
    
    // Check if the player is in the field of view and if there aren't obstacles in the middle
    public bool checkIsPlayerVisible()                
    {
        if (player.isDeath())
        {
            return isPlayerVisible = false;
        }

        Vector3 myPosition = transform.position;                    // to avoid collisions with terrain, I move up the position check
        myPosition.y += 0.5f;
        Collider[] targetsInViewRadius = Physics.OverlapSphere(myPosition, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - myPosition).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                RaycastHit hit;
                if (toggleSeeThroughObstacles || !Physics.Raycast(myPosition, dirToTarget, out hit, distanceTo(target.position), obstacleMask))
                {
                    lastPlayerPositionKnown = player.transform.position;
                    return isPlayerVisible = true;
                }
                //print(hit.collider.gameObject.name);
            }
        }
        return isPlayerVisible = false;
    }

    // calculate distance from this gameobject to target position
    public float distanceTo(Vector3 target)
    {
        return Vector3.Distance(transform.position, target);
    }

    // draw the fov of this enemy
    private void OnDrawGizmosSelected()
    {
        Vector3 pos = transform.position;
        pos.y += 0.5f;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pos, viewRadius);

        Vector3 fovLine1 = Quaternion.AngleAxis(viewAngle/2, transform.up) * transform.forward * viewRadius;
        Vector3 fovLine2 = Quaternion.AngleAxis(-viewAngle/2, transform.up) * transform.forward * viewRadius;

        Gizmos.color = Color.black;
        Gizmos.DrawRay(pos, fovLine1);
        Gizmos.DrawRay(pos, fovLine2);

        if (isPlayerVisible)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(pos, lastPlayerPositionKnown);
        }
    }

    public void Reset()
    {
        viewAngle = originVA;
        viewRadius = originVR;
    }


    //public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    //{
    //    if (!angleIsGlobal)
    //    {
    //        angleInDegrees += transform.eulerAngles.y;
    //    }
    //    return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    //}

}
