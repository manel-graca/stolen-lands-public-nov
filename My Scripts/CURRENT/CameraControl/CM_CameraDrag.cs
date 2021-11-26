using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;
using StolenLands.Player;

public class CM_CameraDrag : MonoBehaviour
{
    [SerializeField] float sensitivity = 5f;

    PlayerInput pInput;
    void Start()
    {
        gameObject.SetActive(false);
        gameObject.SetActive(true);
        pInput = FindObjectOfType<PlayerInput>();
        CinemachineCore.GetInputAxis = GetAxisCustom;
    }
	public float GetAxisCustom(string axisName)
    {
        if (axisName == "Mouse X")
        {
            if (Input.GetMouseButton(0) && pInput.pressingALT)
            {
                return UnityEngine.Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
            }
            else
            {
                return 0;
            }
        }
        else if (axisName == "Mouse Y")
        {
            if (Input.GetMouseButton(0) && pInput.pressingALT)
            {
                return UnityEngine.Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
            }
            else
            {
                return 0;
            }
        }
        return UnityEngine.Input.GetAxis(axisName);
    } 
}
