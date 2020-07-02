using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class craftedObjectPlacer : MonoBehaviour
{
    public GameObject placingBlockTemplateRef;
    void Update()
    {
        transform.position = placingBlockTemplateRef.transform.position;
        if(Input.GetKeyDown(KeyCode.B))
        {
            Destroy(this);
        }
    }
}
