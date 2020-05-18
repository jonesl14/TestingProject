using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKill : MonoBehaviour
{
    public GameObject partToSpawn;
    GameObject testSpawn = null;

    private int collisionCount = 0;
    private bool outOfBoundsRoutineRunning = false;

    private void OnTriggerEnter(Collider other)
    {
        //if(other.name == "weaponBlade" && !testSpawn)
        if (other.name == "DeleteTool" && !testSpawn)
        {
            string partName = "DroppedAssets/Part" + Random.Range(0, 4);
            //Debug.Log("part" + partName);
            testSpawn = Instantiate(Resources.Load(partName, typeof(GameObject)) as GameObject,this.gameObject.transform);
            testSpawn.transform.parent = null;

            //Instantiate(partToSpawn, this.gameObject.transform).gameObject.transform.parent = null;
            Destroy(this.gameObject);    
        }
        collisionCount++;
        if(collisionCount != 0)
        {
            if(outOfBoundsRoutineRunning)
            {
                StopCoroutine(killOutOfBoundsMob());
                outOfBoundsRoutineRunning = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        collisionCount--;
        if(collisionCount == 0)
        {
            //Debug.Log("starting");
            StartCoroutine(killOutOfBoundsMob());
        }
    }

    private IEnumerator killOutOfBoundsMob()
    {
        outOfBoundsRoutineRunning = true;
        yield return new WaitForSeconds(3);
        if (collisionCount == 0)
        {
            Destroy(gameObject);
        }
    }
}
