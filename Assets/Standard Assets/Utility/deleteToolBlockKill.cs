using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class deleteToolBlockKill : MonoBehaviour
{
    public GameObject originPointObj;
    bool downSwing = false, upSwing = false;
    private void OnTriggerStay(Collider other)
    {
        if ((other.gameObject.layer == 10 || other.gameObject.layer == 16) && Input.GetMouseButton(1))// Input.GetKey(KeyCode.Y))// && CrossPlatformInputManager.GetButton("RemoveBuilt"))
        {
            other.GetComponent<BlockBreaking>().killBlock();
        }
    }

    void Update()
    {
        //if (Input.GetKey(KeyCode.Y))
        if(Input.GetMouseButtonDown(1))
        {
            //UnityEngine.Debug.Log(transform.parent.GetChild(4).transform.rotation);
            //transform.parent.GetChild(4).transform.rotation = new Quaternion(.6f, 0, 0, 0);
            transform.parent.transform.GetChild(4).transform.Rotate(85f, 0, 0);
        }
        if(Input.GetMouseButton(1))
        {
            //UnityEngine.Debug.Log(transform.GetChild(4).localEulerAngles.x);
            //transform.GetChild(4).transform.Rotate(1f, 0, 0);
            // - is the upswing + is the downswing
            if (transform.parent.transform.GetChild(4).localEulerAngles.x <= 80f && !downSwing)
            {
                upSwing = true;
                transform.parent.transform.GetChild(4).transform.Rotate(1f, 0, 0);
                if (transform.parent.transform.GetChild(4).localEulerAngles.x > 80f)
                {
                    upSwing = false;
                }
            }
            else if (transform.parent.transform.GetChild(4).localEulerAngles.x >= 60f && !upSwing)
            {
                downSwing = true;
                transform.parent.transform.GetChild(4).transform.Rotate(-1f, 0, 0);
                if (transform.parent.transform.GetChild(4).localEulerAngles.x < 60f)
                {
                    downSwing = false;
                }
            }
        }
        if(Input.GetMouseButtonUp(1))
        {
            upSwing = false;
            downSwing = false;
            //transform.parent.transform.GetChild(4).transform.Rotate(-85f, 0, 0);
            transform.parent.GetChild(4).transform.localRotation = new Quaternion(0, 0, 0, 0);
            //transform.parent.GetChild(4).transform.rotation = new Quaternion(0,0,0,0);
        }
    }
}
