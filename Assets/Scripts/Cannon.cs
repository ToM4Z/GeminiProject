using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    private Transform baseRotonda;
    private Transform canna;
    public float obstacleRange = 5.0f;

    [SerializeField] private GameObject cannonBallPrefab;
    private GameObject _cannonBall;
    
    void Start()
    {
        //Ruotarli sull'asse Z
        baseRotonda = this.gameObject.transform.GetChild(0).GetChild(1);
        canna = baseRotonda.GetChild(0).GetChild(2);
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)){
            Debug.DrawRay(transform.position, transform.forward * 10, Color.green);
            GameObject hitObject = hit.transform.gameObject;
            Debug.Log(hit.collider.gameObject.name);
                if (hitObject.GetComponent<PlayerController>()) {
                    if (_cannonBall == null){             
                        _cannonBall = Instantiate(cannonBallPrefab) as GameObject;
                        _cannonBall.transform.position =  transform.TransformPoint(Vector3.forward * 1.5f);
                        _cannonBall.transform.rotation = transform.rotation;
                    }
            }
        }

        baseRotonda.Rotate(0,0,1);
        //canna.Rotate(0,0,1);
    }
}
