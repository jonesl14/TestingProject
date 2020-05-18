using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayAndNightCycle : MonoBehaviour
{
    float newTimeX;
    // Start is called before the first frame update
    void Start()
    {
        newTimeX = transform.rotation.x + .05f;
    }

    private void Update()
    {
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(newTimeX, 0, 0),  Time.deltaTime);
        transform.rotation = Quaternion.RotateTowards(Quaternion.Euler(newTimeX, 0, 0),transform.rotation, Time.deltaTime);
        if (transform.rotation == Quaternion.Euler(newTimeX, 0, 0))
        {
            /*if (transform.localEulerAngles.x >180)
            {
                newTimeX += .5f;
            }
            else*/
            {
                newTimeX += .1f;
            }
        }
    }
}
