using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  Class: DestroyableWallController
 *  
 *  Description:
 *  This script make a wall destroyable by a bomb
 *  
 *  
 *  Author: Andrea De Seta, 
 *          Thomas Voce (Make this script resettable)
*/

public class DestroyableWallController : MonoBehaviour, IResettable
{
    private Rigidbody[] rbChild;

    private BoxCollider[] bc;
    private BoxCollider[] bcChild;

    private Vector3[] childOriginPos;

    void Start()
    {
        //bc are two boxcolliders that compose the principal gameobject: one is used for trigger and the other (Andrea)
        //it's used to make the wall touchable
        bc = GetComponents<BoxCollider>();

        //rbChild and bcChild are all rigidbody and box colliders of each brick that make the wall (Andrea)
        bcChild = GetComponentsInChildren<BoxCollider>();
        rbChild = GetComponentsInChildren<Rigidbody>();

        List<Vector3> _childOriginPos = new List<Vector3>();
        foreach ( Rigidbody r in rbChild)
        {
            _childOriginPos.Add(r.transform.position);
        }
        childOriginPos = _childOriginPos.ToArray();
    }

    public void DestroyWall(float power, Vector3 center, float radius, float upforce, ForceMode fm){

        //Loop on rbChild in order to make the explosion (Andrea)
        for (int i = 0; i < rbChild.Length; i++) {
            rbChild[i].AddExplosionForce(power, center, radius, upforce, fm);
            rbChild[i].isKinematic = false;
            rbChild[i].useGravity = true;
            bcChild[i].enabled = true;
        }

        //I disable the boxcollider otherwise i touch the box even if the wall is exploded (Andrea)
        for (int i = 0; i < bc.Length; i++) {
            bc[i].enabled = false;
        }

        //I disable the object to clean the scene and to not make collision errors (Andrea, Thomas)
        StartCoroutine(Disappear());
    }

    private IEnumerator Disappear(){
        yield return new WaitForSeconds(3.0f);
        gameObject.SetActive(false);        // I disactive bricks after few seconds (Thomas)
    }

    // when AmbientalObjectManager reset this wall, I reset all bricks position to their initial position, disable gravity on it and reset wall collider (Thomas) 
    public void Reset()
    {
        for (int i = 0; i < rbChild.Length; i++)
        {
            rbChild[i].useGravity = false;
            rbChild[i].isKinematic = true;
            bcChild[i].enabled = false;
            rbChild[i].transform.position = childOriginPos[i];
            rbChild[i].transform.rotation = Quaternion.identity;
        }

        for (int i = 0; i < bc.Length; i++)
        {
            bc[i].enabled = true;
        }

        gameObject.SetActive(true);
    }

}
