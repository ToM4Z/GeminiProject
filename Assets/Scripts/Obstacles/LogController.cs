using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        waitTime = 2.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(startTime >= waitTime) {
            //append di un nuovo log istanziato
        }
        foreach (GameObject log in _logs) {
            //eliminare gli oggetti arrivati alla fine
        }
        startTime += Time.deltaTime;
    }
}
