using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableWallController : MonoBehaviour
{
    private Rigidbody[] rbChild;

    private BoxCollider[] bc;
    private BoxCollider[] bcChild;

    void Start()
    {
        //rbChild and bcChild are all rigidbody and box colliders of each brick that make the wall
        bcChild = GetComponentsInChildren<BoxCollider>();
        rbChild = GetComponentsInChildren<Rigidbody>();

        //bc are two boxcolliders that compose the principal gameobject: one is used for trigger and the other
        //it's used to make the wall touchable
        bc = GetComponents<BoxCollider>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DestroyWall(float power, Vector3 center, float radius, float upforce, ForceMode fm){
        
        //Loop on rbChild in order to make the explosion
        for (int i = 0; i < rbChild.Length; i++) {
            rbChild[i].AddExplosionForce(power, center, radius, upforce, fm);
            rbChild[i].useGravity = true;
            bcChild[i].enabled = true;
        }

        //I disable the boxcollider otherwise i touch the box even if the wall is exploded
        for (int i = 0; i < bc.Length; i++) {
            bc[i].enabled = false;
        }

        //I destroy the object to clean the scene and to not make collision errors
        StartCoroutine(Disappear());
    }

    private IEnumerator Disappear(){
        yield return new WaitForSeconds(5.0f);
        Destroy(this.gameObject);
    }
}
