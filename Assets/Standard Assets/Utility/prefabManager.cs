using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class prefabManager : MonoBehaviour
{
    public List<UnityEngine.Object> storedPrefabs;
    public List<string> storedPrefabNames;

    public Dictionary<string, UnityEngine.Object> storedKeyedPrefabs;

    int keyNum = 0;
    void Start()
    {
        UnityEngine.Object[] loadedPrefabs = Resources.LoadAll("Crafting/", typeof(GameObject));

        storedKeyedPrefabs = new Dictionary<string, UnityEngine.Object>();

        foreach(UnityEngine.Object prefabsLoaded in loadedPrefabs)
        {
            storedKeyedPrefabs.Add(prefabsLoaded.name, prefabsLoaded);
        }

        foreach(string prefabKeys in storedKeyedPrefabs.Keys)
        {
            storedPrefabNames.Add(prefabKeys);
        }
        foreach(UnityEngine.Object prefabObject in storedKeyedPrefabs.Values)
        {
            storedPrefabs.Add(prefabObject);
        }
    }

    public UnityEngine.Object getObjectToCraft(string objectCraftName)
    {
        //if(objectCraftID < storedPrefabs.Count)
        if(storedKeyedPrefabs.ContainsKey(objectCraftName))
        {
            return storedKeyedPrefabs[objectCraftName];
            //return storedPrefabs[objectCraftID];
        }
        else
        {
            return null;
        }
    }
}
