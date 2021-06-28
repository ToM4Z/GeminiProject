using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  Class: LogController
 *  
 *  Description:
 *  Script to handle the log controller trap which spawn some log prefab until it reaches the finalpoint
 *  
 *  Author: Gianfranco Sapia
*/
public class LogController : MonoBehaviour
{
    [SerializeField] private GameObject LogPrefab;
    private List<GameObject> _logs;
    [SerializeField] private GameObject startPos;
    [SerializeField] private GameObject endPos; 
    private float startTime;
    private float waitTime;

    // Start is called before the first frame update
    void Start()
    {
        _logs = new List<GameObject>();
        startTime = 0f;
        waitTime = 1.0f;
    }

    /*
    * Every waitTime seconds passed a new LogPrefab is istantiate with transform.position equal to startPos and a transform.rotation equal
    * to 0 if the object with LogController script has rotation equal to 0 or 180, or equal to the object.y if the the object
    * has rotation 90 or 270. Every time that an instantiated log transform is equal to endposition.transform.position is then deleted 
    */
    void Update()
    {
        if(startTime >= waitTime) {
            GameObject log = Instantiate(LogPrefab) as GameObject;
            log.GetComponent<LogBehaviour>().SetPos(startPos.transform.position,endPos.transform.position);
            log.transform.position = startPos.transform.position;
            if(this.transform.rotation.eulerAngles.y == 90 || this.transform.rotation.eulerAngles.y == 270) {
                log.transform.Rotate(0,this.transform.rotation.eulerAngles.y,0);
            }
            _logs.Add(log);
            startTime = 0;
        }
        
        // toBeRemoved list is necessary otherwise there will be an error
        List<GameObject> toBeRemoved = new List<GameObject>();
        foreach (GameObject log in _logs) {
            if(log.transform.position == endPos.transform.position) {
                toBeRemoved.Add(log);
                Destroy(log);
            }            
        }
        foreach (GameObject l in toBeRemoved) {
            _logs.Remove(l);
        }
        startTime += Time.deltaTime;
    }
}
