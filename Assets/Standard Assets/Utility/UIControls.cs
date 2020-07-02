using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControls : MonoBehaviour
{
    public Button createButton;
    public Slider buildThingSlider;
    public Dropdown craftDropdown;
    public GameObject blockTrackerRef;
    public GameObject prefabManagerRef;
    public GameObject placingBlockTemplateRef;

    private GameObject craftedGameObject;
    private bool craftedObjectPlaced = true;

    void Start()
    {
        createButton.onClick.AddListener(createButtonClick);
        craftDropdown.GetComponent<Dropdown>().AddOptions(prefabManagerRef.GetComponent<prefabManager>().storedPrefabNames);
    }
    /// <summary>
    /// Modify the entire building aspect to allow storing full objects instead of just materials. Inventory system
    /// </summary>
    void createButtonClick()
    {
        switch (buildThingSlider.GetComponent<Slider>().value)
        {
            case 0:
                if(blockTrackerRef.GetComponent<blockManager>().getCountOfBlock(12) > 0)
                {
                    craftedGameObject = (GameObject)Instantiate(prefabManagerRef.GetComponent<prefabManager>().getObjectToCraft("door"), placingBlockTemplateRef.transform.position,transform.rotation);
                    craftedGameObject.AddComponent<craftedObjectPlacer>().placingBlockTemplateRef = placingBlockTemplateRef;
                }
                break;
            case 1:
                if (blockTrackerRef.GetComponent<blockManager>().getCountOfBlock(12) > 0)
                {
                    craftedGameObject = (GameObject)Instantiate(prefabManagerRef.GetComponent<prefabManager>().getObjectToCraft("table"), placingBlockTemplateRef.transform.position, transform.rotation);
                    craftedGameObject.AddComponent<craftedObjectPlacer>().placingBlockTemplateRef = placingBlockTemplateRef;
                }
                break;
        }
    }
}
