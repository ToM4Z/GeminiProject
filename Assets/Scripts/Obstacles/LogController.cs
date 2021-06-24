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
    AudioSource _audio;

    // Start is called before the first frame update
    void Start()
    {
        _logs = new List<GameObject>();
        startTime = 0f;
        waitTime = 1.0f;
        _audio = this.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!_audio.isPlaying) {
            _audio.Play();
        }
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
        List<GameObject> toBeRemoved = new List<GameObject>();
        foreach (GameObject log in _logs) {
            if(log.transform.position == endPos.transform.position) {
                toBeRemoved.Add(log);
                Destroy(log);
                //_logs.Remove(log);
            }            
        }
        foreach (GameObject l in toBeRemoved) {
            _logs.Remove(l);
        }
        startTime += Time.deltaTime;
    }
}
