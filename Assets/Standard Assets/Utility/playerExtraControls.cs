using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerExtraControls : MonoBehaviour
{
    private bool placingTorch = false;
    bool downSwing = false, upSwing = false;
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

        /*if(Input.GetKey(KeyCode.Y))
        {
            UnityEngine.Debug.Log(transform.GetChild(4).localEulerAngles.x);
            //transform.GetChild(4).transform.Rotate(1f, 0, 0);
            // - is the upswing + is the downswing
            if (transform.GetChild(4).localEulerAngles.x <= 80f && !downSwing)
            {
                upSwing = true;
                transform.GetChild(4).transform.Rotate(3f, 0, 0);
                if(transform.GetChild(4).localEulerAngles.x > 80f)
                {
                    upSwing = false;
                }
            }
            else if (transform.GetChild(4).localEulerAngles.x >= 60f && !upSwing)
            {
                downSwing = true;
                transform.GetChild(4).transform.Rotate(-3f, 0, 0);
                if(transform.GetChild(4).localEulerAngles.x < 60f)
                {
                    downSwing = false;
                }
            }
        }*/
    }
}
