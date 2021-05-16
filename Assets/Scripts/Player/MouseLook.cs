using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 *  Class: MouseLook
 *  
 *  Description:
 *  This script manages the mouse/analog input to handle camera orientation.
 *  
 *  Author: Gemini
*/
public class MouseLook : MonoBehaviour
{
    private bool enable = true;

    void Start()
    {
        Rigidbody body = GetComponent<Rigidbody>();
        if (body != null)
            body.freezeRotation = true;
    }


    public enum RotationAxes
    {
        MouseXAndY = 0,
        MouseX = 1,
        MouseY = 2
    }
    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float sensitivityHor = 9f;
    public float sensitivityVer = 9f;

    public float minimumVert = -45f;
    public float maximumVert = 45f;

    private float _rotationX = 0f;

    void Update()
    {
        if (!enable)
            return;

        if (axes == RotationAxes.MouseX)
        {
            transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityHor, 0);
        }
        else if (axes == RotationAxes.MouseY)
        {
            _rotationX -= Input.GetAxis("Mouse Y") * sensitivityVer;
            _rotationX = Mathf.Clamp(_rotationX, minimumVert, maximumVert);

            float rotationY = transform.localEulerAngles.y;

            transform.localEulerAngles = new Vector3(_rotationX, rotationY, 0);
        }
        else
        {
            _rotationX -= Input.GetAxis("Mouse Y") * sensitivityVer;
            _rotationX = Mathf.Clamp(_rotationX, minimumVert, maximumVert);
            float delta = Input.GetAxis("Mouse X") * sensitivityHor;
            float rotationY = transform.localEulerAngles.y + delta;

            transform.localEulerAngles = new Vector3(_rotationX, rotationY, 0);

        }
    }

    private void Awake()
    {
        Messenger<bool>.AddListener(GameEvent.ENABLE_INPUT, enableInput);
    }

    private void OnDestroy()
    {
        Messenger<bool>.RemoveListener(GameEvent.ENABLE_INPUT, enableInput);
    }

    private void enableInput(bool _b)
    {
        enable = _b;
    }
}
