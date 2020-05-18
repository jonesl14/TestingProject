using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;

public class PassiveCreatureAI2 : MonoBehaviour
{
    public bool doneRotating = true;
    public bool doneMoving = true;
    public bool runningCreatureRotation = false;
    public bool stuckOnTerrarin = false;
    private float groundedCreatureHeight = -999f;

    private IEnumerator /*rotateRoutine,*/ walkRoutine;
    Vector3 creatureVelocity;

    IEnumerator coCreatureWalkTimer(float creatureTimer)
    {//4 called by calcNewCreatureVelocity using calcNewCreatureWalkTime
        yield return new WaitForSecondsRealtime(creatureTimer);
        doneMoving = true;
        doneRotating = false;
        StopCoroutine(walkRoutine);

    }

    private Vector3 calcNewCreatureVelocity()
    {//2 called by update
        doneMoving = false;
        walkRoutine = coCreatureWalkTimer(calcNewCreatureWalkTime());
        StartCoroutine(walkRoutine);

        Vector3 tempVelocity = transform.forward;
        return tempVelocity;
    }

    private float calcNewCreatureWalkTime()
    {//3 called by calcNewCreatureVelocity
        return Random.Range(4, 9);
    }

    bool moving = false;
    bool needToFall = false;
    private Quaternion newRandomCreatureRotation;
    private void Update()
    {
        if(doneMoving && doneRotating)
        {//1
            //GetComponent<Rigidbody>().AddRelativeForce(calcNewCreatureVelocity());
            GetComponent<CharacterController>().SimpleMove(calcNewCreatureVelocity());
        }

        if(!doneMoving && doneRotating && !runningCreatureRotation)
        {
            if(!moving)
            {
                moving = true;
                GetComponent<Animator>().Play("PassiveAIAnim");
                creatureVelocity = calcNewCreatureVelocity();
            }
            GetComponent<CharacterController>().SimpleMove(creatureVelocity);
        }

        if(doneMoving && !doneRotating && !runningCreatureRotation)
        {
            if(moving)
            {
                GetComponent<Animator>().Play("PassiveAIStand");
                newRandomCreatureRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            }
            moving = false;

            if(transform.rotation != newRandomCreatureRotation)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, newRandomCreatureRotation, 200 * Time.deltaTime);
            }
            else
            {
                doneRotating = true;
                doneMoving = false;
                runningCreatureRotation = false;
            }
        }
    }
}
