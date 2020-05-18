using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class deleteToolBlockKill : MonoBehaviour
{
    public GameObject originPointObj;
    private void OnTriggerStay(Collider other)
    {
        //if(other.name.Contains("worldBlock") && CrossPlatformInputManager.GetButton("RemoveBuilt"))
        if ((other.gameObject.layer == 10 || other.gameObject.layer == 16) && CrossPlatformInputManager.GetButton("RemoveBuilt"))
        {
            //originPointObj.GetComponent<fixedWorldGen>().killBlock(other.gameObject);
            other.GetComponent<BlockBreaking>().killBlock();
        }
        //Debug.Log(other.name);
    }
}
