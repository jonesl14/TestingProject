using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerExtraControls : MonoBehaviour
{
    private bool placingTorch = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            transform.GetChild(2).GetComponentInChildren<Light>().range = 7;
            placingTorch = !placingTorch;
            transform.GetChild(2).gameObject.SetActive(placingTorch);
        }
        if(Input.GetKey(KeyCode.T))
        {
            transform.GetChild(2).GetComponentInChildren<Light>().range += .1f;
        }
    }
}
