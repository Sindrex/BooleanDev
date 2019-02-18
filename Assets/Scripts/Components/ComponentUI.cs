using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComponentUI : MonoBehaviour {

    public GameController GC;
    public CamController CC;
    public UndoController UC;

    public GameObject confirmComponent;
    public InputField nameInput;
    public GameObject scrollContent;
    public GameObject scrollParent;
    public GameObject openButton;
    public GameObject overlayFather;

    //component tip:
    public GameObject tip;
    public Text tipStats;
    public Text tipName;

    public GameObject buttonPrefab;
    public GameObject componentPlacerPrefab;
    public GameObject compOverlayPrefab;

    private GameObject placer;

    //Overlay
    public bool showOverlay;
    public List<GameObject> overlays = new List<GameObject>();

    public void openConfirmComp()
    {
        if (GC.selectedTiles.Count <= 0)
        {
            print("No selection");
            return;
        }
        confirmComponent.SetActive(true);
        GC.AC.toggleSelectionBar(false);
        UtilBools.playerInteractLock(true);
    }

    public void closeConfirmComp()
    {
        nameInput.text = "";
        confirmComponent.SetActive(false);
        UtilBools.playerInteractLock(false);
    }

    public void saveComp()
    {
        if(nameInput.text == null || nameInput.text.Trim() == "")
        {
            print("Error");
            return;
        }
        else if (GC.selectedHeigth <= 0 || GC.selectedLength <= 0)
        {
            print("No selection: h/l: " + GC.selectedHeigth + "/" + GC.selectedLength);
            return;
        }

        ComponentSave comp = new ComponentSave();
        comp.name = nameInput.text;
        comp.height = GC.selectedHeigth;
        comp.length = GC.selectedLength + 1;

        print("H: " + comp.height + ", L: " + comp.length);

        int arraySize = comp.height*comp.length;
        int[] tileIDs = new int[arraySize];
        int[] tileDIRs = new int[arraySize];
        bool[] tilePower = new bool[arraySize];
        int[] tileSetting = new int[arraySize];
        string[] signTexts = new string[arraySize];

        TileController.label[] tileLabels = new TileController.label[arraySize];

        int index = 0;
        for(int i = 0; i < GC.selectedFloor.Count; i++)
        {
            bool ok = false;
            if(index < GC.selectedTileIndex.Count)
            {
                if (i == GC.selectedTileIndex[index])
                {
                    ok = true;
                }
            }

            if (ok)
            {
                TileController TC = GC.selectedTiles[index].GetComponent<TileController>();
                tileIDs[i] = TC.ID;
                tileDIRs[i] = TC.getDir();
                tilePower[i] = TC.beingPowered;
                tileLabels[i] = TC.myLabel;
                if (GC.selectedTiles[index].GetComponent<DelayerController>() != null)
                {
                    tileSetting[i] = GC.selectedTiles[index].GetComponent<DelayerController>().getSetting();
                }
                else if (GC.selectedTiles[index].GetComponent<SignController>() != null)
                {
                    signTexts[i] = GC.selectedTiles[index].GetComponent<SignController>().text;
                }
                else
                {
                    tileSetting[i] = 0;
                    signTexts[i] = "";
                }
                index++;
            }
            else
            {
                tileIDs[i] = 0;
                tileDIRs[i] = 0;
                tilePower[i] = false;
                tileSetting[i] = 0;
                signTexts[i] = "";
                tileLabels[i] = TileController.label.NULL;
            }
        }

        comp.tileIDs = tileIDs;
        comp.tileDIRs = tileDIRs;
        comp.tilePower = tilePower;
        comp.tileSetting = tileSetting;
        comp.signTexts = signTexts;
        comp.tileLabels = tileLabels;
        comp.date = "" + System.DateTime.Today.Day + "." + System.DateTime.Today.Month + "." + System.DateTime.Today.Year;

        SaveLoadComp.SaveComp(comp);
        closeConfirmComp();
        updateUI();
    }

    public void openCompUI()
    {
        openButton.SetActive(false);
        scrollParent.SetActive(true);
        SaveLoadComp.LoadComp();
        createButtons();
    }

    public void toggleCompUI()
    {
        if (!scrollParent.activeSelf)
        {
            scrollParent.SetActive(true);
            SaveLoadComp.LoadComp();
            createButtons();
        }
        else
        {
            destroyButtons();
            scrollParent.SetActive(false);
        }
    }

    public void updateUI()
    {
        destroyButtons();
        SaveLoadComp.LoadComp();
        createButtons();
    }

    public void closeCompUI()
    {
        destroyButtons();
        scrollParent.SetActive(false);
        //openButton.SetActive(true);
    }

    private void createButtons()
    {
        foreach (ComponentSave c in SaveLoadComp.savedComponents)
        {
            GameObject prefab = Instantiate(buttonPrefab, scrollContent.transform);
            prefab.transform.position += new Vector3(0, 0, -1);
            prefab.GetComponent<ComponentSaveUIButtonController>().CUI = this;
            prefab.GetComponent<ComponentSaveUIButtonController>().myComp = c;
            prefab.GetComponent<ComponentSaveUIButtonController>().setText();
        }
    }

    private void destroyButtons()
    {
        for (int i = 1; i < scrollContent.transform.childCount; i++)
        {
            Destroy(scrollContent.transform.GetChild(i).gameObject);
        }
    }

    public void loadComp(ComponentSave comp, Vector3 pos)
    {
        if(placer == null)
        {
            GameObject prefab = Instantiate(componentPlacerPrefab, Camera.main.ScreenToWorldPoint(new Vector3(Screen.width/2, Screen.height/2, 10)), Quaternion.identity);
            prefab.GetComponent<ComponentPlacerController>().myComp = comp;
            prefab.GetComponent<ComponentPlacerController>().GC = GC;
            prefab.GetComponent<ComponentPlacerController>().CUI = this;
            prefab.GetComponent<ComponentPlacerController>().UC = UC;
            placer = prefab;
        }
        else
        {
            placer.GetComponent<ComponentPlacerController>().myComp = comp;
            placer.GetComponent<ComponentPlacerController>().setBackdrop();
        }
    }

    public void createOverlay(ComponentSave myComp, List<GameObject> tiles, List<GameObject> floorTiles)
    {
        GameObject prefab = Instantiate(compOverlayPrefab, overlayFather.transform);
        prefab.transform.position = placer.transform.position;
        ComponentOverlay overlay = prefab.transform.GetChild(0).GetComponent<ComponentOverlay>();
        overlay.setParams(myComp, tiles, floorTiles);

        overlays.Add(prefab);
    }

    public void toggleOverlay()
    {
        if (overlayFather.activeSelf)
        {
            overlayFather.SetActive(false);
            foreach(GameObject go in overlays)
            {
                foreach(GameObject obj in go.transform.GetChild(0).GetComponent<ComponentOverlay>().tiles)
                {
                    if(obj != null)
                    {
                        obj.GetComponent<TileController>().compLocked = false;
                    }
                }
            }
        }
        else
        {
            overlayFather.SetActive(true);
            foreach (GameObject go in overlays)
            {
                foreach (GameObject obj in go.transform.GetChild(0).GetComponent<ComponentOverlay>().tiles)
                {
                    if(obj != null)
                    {
                        obj.GetComponent<TileController>().compLocked = true;
                    }
                }
            }
        }
    }

    public void removeOverlay(GameObject overlay)
    {
        foreach (GameObject obj in overlay.transform.GetChild(0).GetComponent<ComponentOverlay>().tiles)
        {
            if(obj != null)
            {
                obj.GetComponent<TileController>().compLocked = false;
            }
        }
        overlays.Remove(overlay);
        Destroy(overlay);
    }

    public bool isPlacerOn()
    {
        if(placer != null)
        {
            return placer.activeSelf;
        }
        return false;
    }
}
