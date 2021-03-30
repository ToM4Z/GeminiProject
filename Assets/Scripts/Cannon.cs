using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    private Transform baseRotonda;
    private Transform canna;
    private Transform puntoSparo;
    private Transform puntoRay;
    public float obstacleRange = 5.0f;

    private bool canShoot = true;
    [SerializeField] private GameObject cannonBallPrefab;
    [SerializeField] private GameObject smokeShootPrefab;
    private GameObject _cannonBall;
    private GameObject _smoke;
    
    void Start()
    {
        //Ruotarli sull'asse Z
        baseRotonda = this.gameObject.transform.GetChild(0).GetChild(1);
        canna = baseRotonda.GetChild(0).GetChild(2);
        puntoSparo = baseRotonda.GetChild(3);

    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(transform.position, transform.forward * 50, Color.red);
        Ray ray = new Ray(baseRotonda.position, baseRotonda.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)){
            Debug.Log(hit.transform.position);
            GameObject hitObject = hit.transform.gameObject;
            Debug.Log(hit.collider.gameObject.name);
                if (hitObject.GetComponent<PlayerController>()) {
                    if (canShoot){             
                        _cannonBall = Instantiate(cannonBallPrefab) as GameObject;
                        _cannonBall.transform.position = puntoSparo.position;
                        _cannonBall.transform.rotation = puntoSparo.rotation;
                        _smoke = Instantiate(smokeShootPrefab);
                        _smoke.transform.position = puntoSparo.position;
                        //_smoke.transform.rotation = puntoSparo.rotation;
                        StartCoroutine(Shoot());
                    }
            }
        }

        //baseRotonda.Rotate(0,0,1);
        //canna.Rotate(0,0,1);
    }

    private IEnumerator Shoot(){
        canShoot = false;
        yield return new WaitForSeconds(2f);
        canShoot = true;
        Destroy(_smoke);
    }
}
