using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogBehaviour : MonoBehaviour
{
    private Vector3 startPos;
    private Vector3 endPos;
    private float distance;

    public LogBehaviour(Vector3 startPos, Vector3 endPos) {
        this.startPos = startPos;
        this.endPos = endPos;
    }

    // Start is called before the first frame update
    void Start()
    {
        distance = Vector3.Distance(this.startPos,this.endPos);
    }

    // Update is called once per frame
    void Update()
    {
        //inserire lerp rotation e transform
    }
}
