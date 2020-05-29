using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayAndNightCycle : MonoBehaviour
{
    float newTimeX;
    void Start()
    {
        newTimeX = transform.rotation.x + .05f;
    }

    private void Update()
    {
        transform.rotation = Quaternion.RotateTowards(Quaternion.Euler(newTimeX, 0, 0),transform.rotation, Time.deltaTime);
        if (transform.rotation == Quaternion.Euler(newTimeX, 0, 0))
        {
            newTimeX += .1f;
        }
    }
}
