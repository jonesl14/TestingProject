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
        //switch (buildThingSlider.GetComponent<Slider>().value)
        switch(craftDropdown.value)
        {
            case 0:
                if (blockTrackerRef.GetComponent<blockManager>().getCountOfBlock((int)buildThingSlider.value) > 0)
                {
                    craftedGameObject = (GameObject)Instantiate(prefabManagerRef.GetComponent<prefabManager>().getObjectToCraft("buildBlock"), placingBlockTemplateRef.transform.position, transform.rotation);
                    craftedGameObject.AddComponent<craftedObjectPlacer>().placingBlockTemplateRef = placingBlockTemplateRef;
                    craftedGameObject.GetComponent<Renderer>().material = (Material)Resources.Load("BlockArt/4-All/" + buildThingSlider.value);
                    blockTrackerRef.GetComponent<blockManager>().removeBlock((int)buildThingSlider.value);
                }
                else
                {
                    StartCoroutine(showMessage("Not enough " + buildThingSlider.value + " blocks", 1));
                }
                break;
            case 1:
                if(blockTrackerRef.GetComponent<blockManager>().getCountOfBlock(12) > 0)
                {
                    craftedGameObject = (GameObject)Instantiate(prefabManagerRef.GetComponent<prefabManager>().getObjectToCraft("door"), placingBlockTemplateRef.transform.position,transform.rotation);
                    craftedGameObject.AddComponent<craftedObjectPlacer>().placingBlockTemplateRef = placingBlockTemplateRef;
                    blockTrackerRef.GetComponent<blockManager>().removeBlock(12);
                    craftedGameObject.GetComponent<Renderer>().material = (Material)Resources.Load("BlockArt/4-All/" + buildThingSlider.value);
                    craftedGameObject.GetComponent<craftedObjectPlacer>().setMaterial();
                }
                else
                {
                    StartCoroutine(showMessage("Not enough wood", 1));
                }
                break;
            case 2:
                if (blockTrackerRef.GetComponent<blockManager>().getCountOfBlock(12) > 0)
                {
                    craftedGameObject = (GameObject)Instantiate(prefabManagerRef.GetComponent<prefabManager>().getObjectToCraft("table"), placingBlockTemplateRef.transform.position, transform.rotation);
                    craftedGameObject.AddComponent<craftedObjectPlacer>().placingBlockTemplateRef = placingBlockTemplateRef;
                    blockTrackerRef.GetComponent<blockManager>().removeBlock(12);
                    craftedGameObject.GetComponent<Renderer>().material = (Material)Resources.Load("BlockArt/4-All/" + buildThingSlider.value);
                    craftedGameObject.GetComponent<craftedObjectPlacer>().setMaterial();
                }
                else
                {
                    StartCoroutine(showMessage("Not enough wood", 1));
                }
                break;
            case 3:
                if (blockTrackerRef.GetComponent<blockManager>().getCountOfBlock(12) > 0)
                {
                    craftedGameObject = (GameObject)Instantiate(prefabManagerRef.GetComponent<prefabManager>().getObjectToCraft("door2"), placingBlockTemplateRef.transform.position, transform.rotation);
                    craftedGameObject.AddComponent<craftedObjectPlacer>().placingBlockTemplateRef = placingBlockTemplateRef;
                    blockTrackerRef.GetComponent<blockManager>().removeBlock(12);
                    craftedGameObject.GetComponent<Renderer>().material = (Material)Resources.Load("BlockArt/4-All/" + buildThingSlider.value);
                    craftedGameObject.GetComponent<craftedObjectPlacer>().setMaterial();
                }
                else
                {
                    StartCoroutine(showMessage("Not enough wood", 1));
                }
                break;
        }
    }

    private IEnumerator showMessage(string messageToShow, int timeToShow)
    {
        //These messages will get stuck on the screen if the menu is left before the text dissapears
        transform.parent.GetChild(2).GetComponent<Text>().text = messageToShow;

        yield return new WaitForSeconds(timeToShow);

        transform.parent.GetChild(2).GetComponent<Text>().text = "";
    }
}
