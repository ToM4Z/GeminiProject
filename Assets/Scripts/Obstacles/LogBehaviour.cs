using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogBehaviour : MonoBehaviour
{
    private Vector3 startPos;
    private Vector3 endPos;
    private float distance;
    private float durationTime;
    private float startTime;
    public float speed;

    public void SetPos(Vector3 startPos, Vector3 endPos) {
        this.startPos = startPos;
        this.endPos = endPos;
    }

    // Start is called before the first frame update
    void Start()
    {
        distance = Vector3.Distance(this.startPos,this.endPos);
        this.durationTime = 1.0f * distance;
        this.startTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = Vector3.Lerp(startPos,endPos,startTime/durationTime);
        startTime += Time.deltaTime * speed;
    }
}
