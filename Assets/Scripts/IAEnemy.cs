using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IAEnemy : MonoBehaviour
{
    protected enum EStatus
    {
        GO,
        TURNBACK,
        IDLE,
        WARNED
    }

    public float speed = 2f;            // velocità di movimento
    public float speedRotate = 60f;        // velocità di movimento durante la rotazione
    public float obstacleRange = 5f;    // distanza con cui rileva gli oggetti

    protected bool alive = true;

    protected EStatus status, oldStatus;
    protected Rigidbody rig;
    protected Transform player;

    protected FOVDetection fov;

    protected virtual void Start()
    {
        rig = GetComponent<Rigidbody>();
        fov = this.GetComponentInChildren<FOVDetection>();
    }

    protected void ChangeStatus(EStatus s)
    {
        Debug.Log(s);
        oldStatus = status;
        status = s;
    }

    public void Warned(Transform _target)
    {
        if (status == EStatus.WARNED)
            return;

        ChangeStatus(EStatus.WARNED);
        player = _target;
        StartCoroutine("isPlayerVisibleYet", 5f);
    }

    private IEnumerator isPlayerVisibleYet(float delay)
    {
        bool exit = false;
        while (!exit)
        {
            Debug.Log("wait");
            yield return new WaitForSeconds(delay);

            if (!fov.isPlayerVisible())
            {
                Debug.Log("return");
                ChangeStatus(oldStatus);
                player = null;
                exit = true;
            }
        }
    }

    public void hitted()
    {
        alive = false;
    }

    protected abstract void Attack();
}
