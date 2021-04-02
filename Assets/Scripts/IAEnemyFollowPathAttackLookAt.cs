using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAEnemyFollowPathAttackLookAt : IAEnemy
{
    public float distanceToWalk = 10f;  // distanza da percorrere 
    private Vector3 targetPathPoint;
    private float oldRotY;

    protected override void Start()
    {
        base.Start();
        ChangeStatus(EStatus.GO);
        targetPathPoint = transform.position;
        targetPathPoint.z += distanceToWalk;
        oldRotY = 0;
    }


    void FixedUpdate()
    {
        if (alive)  // se sono vivo
        {
            if(status != EStatus.WARNED)    // se non è allarmato cammino
            {
                if (status == EStatus.GO)
                {
                    Vector3 move = Vector3.MoveTowards(transform.position, targetPathPoint, speed * Time.fixedDeltaTime);
                    rig.MovePosition(move);
                    transform.LookAt(targetPathPoint);

                    if (Vector3.Distance(transform.position, targetPathPoint) == 0)
                        ChangeStatus(EStatus.TURNBACK);
                }
                else
                if (status == EStatus.TURNBACK)
                {
                    if( oldRotY == 0)
                    {
                        if (transform.rotation.eulerAngles.y < 180)
                        {
                            rig.MoveRotation(Quaternion.Euler(transform.localEulerAngles + new Vector3(0, speedRotate * Time.fixedDeltaTime)));
                        }
                        else
                        {
                            rig.MoveRotation(Quaternion.Euler(0, 180, 0));
                            ChangeStatus(EStatus.GO);
                            targetPathPoint.z -= distanceToWalk;
                            oldRotY = 180;
                        }
                    }
                    else
                    {
                        if (transform.rotation.eulerAngles.y >= 180)
                            rig.MoveRotation(Quaternion.Euler(transform.localEulerAngles + new Vector3(0, speedRotate * Time.fixedDeltaTime)));
                        else
                        {
                            rig.MoveRotation(Quaternion.Euler(0, 0, 0));
                            ChangeStatus(EStatus.GO);
                            targetPathPoint.z += distanceToWalk;
                            oldRotY = 0;
                        }
                    }
                }
            }
            else
            {
                transform.LookAt(player);
                transform.SetPositionAndRotation(transform.position, Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0));
                // sphere cast
                // se è vicino abbastanza gli sparo fuoco o uso l'animazione dell'attacco
                // se non è più warned ritorno al posto
            }
        }
    }

    protected override void Attack()
    {

    }

}
