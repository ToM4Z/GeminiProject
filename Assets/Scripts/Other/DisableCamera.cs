using Cinemachine;
using UnityEngine;

/*
 *  Class: DisableCamera
 *  
 *  Description:
 *  This script is used to manage manually the Input Axis for the FreeLook camera
 *  So, when receive ENABLE_INPUT message with value false, I disable the input for the camera
 *  
 *  Author: Thomas Voce
*/

public class DisableCamera : MonoBehaviour, IResettable
{
    private bool enableInput = true;

    void Start()
    {
        CinemachineCore.GetInputAxis = GetAxisCustom;
        Reset();
    }

    public float GetAxisCustom(string axisName)
    {
        if (axisName == "Mouse X" || axisName == "Mouse Y")
        {
            return enableInput ? Input.GetAxis(axisName) : 0;
        }
        return Input.GetAxis(axisName);
    }

    void Awake()
    {
        Messenger.AddListener(GlobalVariables.RESET, Reset);
        Messenger<bool>.AddListener(GlobalVariables.ENABLE_INPUT, EnableInput);
    }

    void OnDestroy()
    {
        Messenger.RemoveListener(GlobalVariables.RESET, Reset);
        Messenger<bool>.RemoveListener(GlobalVariables.ENABLE_INPUT, EnableInput);
        Reset();
    }

    private void EnableInput(bool b)
    {
        enableInput = b;
    }

    public void Reset()
    {
        GetComponent<CinemachineFreeLook>().m_XAxis.Value = 0f;
        GetComponent<CinemachineFreeLook>().m_YAxis.Value = .5f;
    }
}