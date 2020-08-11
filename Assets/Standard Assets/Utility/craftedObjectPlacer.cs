using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class craftedObjectPlacer : MonoBehaviour
{
    //Add a way to track what material(s) were used to create this object so when its destroyed they are refunded
    public GameObject placingBlockTemplateRef;
    void Update()
    {
        //transform.position = placingBlockTemplateRef.transform.position;
        transform.position = placingBlockTemplateRef.GetComponent<placedBlockSnapping>().pickNewOpenPos2();
        if (Input.GetKeyDown(KeyCode.B))
        {
            gameObject.layer = 16;
            GetComponent<BoxCollider>().enabled = true;
            Destroy(this);
        }
    }

    public void setMaterial()
    {
        if(transform.childCount > 0)
        {
            if(transform.GetChild(0).transform.childCount > 0)
            {
                foreach(Transform grandChildren in transform.GetChild(0).transform)
                {
                    grandChildren.gameObject.GetComponent<Renderer>().material = GetComponent<Renderer>().material;
                }
            }
            else
            {
                foreach (Transform children in transform)
                {
                    children.gameObject.GetComponent<Renderer>().material = GetComponent<Renderer>().material;
                }
            }
        }
    }
}
