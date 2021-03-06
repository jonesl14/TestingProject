﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class placedBlockColliders : MonoBehaviour
{
    public bool currentlyColliding = false;
    private int collisionCount = 0;
    private GameObject stuckCollider;
    public List<GameObject> childCollidersNotClearing;

    //private void OnTriggerEnter(Collider other)
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == 10 || collision.gameObject.layer == 16)
        {

            childCollidersNotClearing.Add(collision.gameObject);
            collisionCount++;
            if (collisionCount != 0)
            {
                
                //stuckCollider = collision.gameObject;
                //Debug.Log("colliding" + name);
                currentlyColliding = true;
            }
        }
    }

    //private void OnTriggerExit(Collider other)
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == 10 || collision.gameObject.layer == 16)
        {

            childCollidersNotClearing.Remove(collision.gameObject);
            collisionCount--;
            if (collisionCount == 0)
            {
                
                //stuckCollider = null;
                currentlyColliding = false;
            }
        }
    }

    private IEnumerator clearEmptyCollisions()
    {
        //yield return new WaitForSeconds(.1f);
        yield return new WaitForSeconds(0);
        int collisionCount = childCollidersNotClearing.Count;

        for (int collisionIndex = 0; collisionIndex < collisionCount; collisionIndex++)
        {

            if (collisionIndex < childCollidersNotClearing.Count)
            {
                if ((childCollidersNotClearing[collisionIndex] == null))
                {
                    childCollidersNotClearing.Remove(childCollidersNotClearing[collisionIndex]);
                    collisionCount--;
                    if(collisionCount == 0)
                    {
                        currentlyColliding = false;
                    }
                }
                else if (collisionIndex < childCollidersNotClearing.Count && !childCollidersNotClearing[collisionIndex].activeSelf)
                {
                    childCollidersNotClearing.Remove(childCollidersNotClearing[collisionIndex]);
                    collisionCount--;
                    if (collisionCount == 0)
                    {
                        currentlyColliding = false;
                    }
                }
            }
        }
        if(childCollidersNotClearing.Count == 0)
        {
            currentlyColliding = false;
        }
    }

    private void Update()
    {
        if(collisionCount > 1)
        {
            StartCoroutine(clearEmptyCollisions());
        }
    }
}
