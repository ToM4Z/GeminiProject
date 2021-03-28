using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    private Transform baseRotonda;
    private Transform canna;
    
    void Start()
    {
        //Ruotarli sull'asse Z
        baseRotonda = this.gameObject.transform.GetChild(0).GetChild(1);
        canna = baseRotonda.GetChild(0).GetChild(2);
    }

    // Update is called once per frame
    void Update()
    {
        baseRotonda.Rotate(0,0,1);
        //canna.Rotate(0,0,1);
    }
}
