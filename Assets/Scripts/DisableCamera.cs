using Cinemachine;
using UnityEngine;

public class DisableCamera : MonoBehaviour
{
    private bool enableInput = true;

    void Start()
    {
        CinemachineCore.GetInputAxis = GetAxisCustom;
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
        Messenger<bool>.AddListener(GlobalVariables.ENABLE_INPUT, EnableInput);
    }

    void OnDestroy()
    {
        Messenger<bool>.RemoveListener(GlobalVariables.ENABLE_INPUT, EnableInput);
    }

    private void EnableInput(bool b)
    {
        enableInput = b;
    }
}