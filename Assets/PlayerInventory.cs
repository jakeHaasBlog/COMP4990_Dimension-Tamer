using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{

    public TMPro.TextMeshProUGUI[] battleActionText;
    public GameObject[] creatureCanvases;

    private Image[] creatureCanvases_canvasImages;
    private TMPro.TextMeshProUGUI[] creatureCanvases_nameAndLevelText;
    private TMPro.TextMeshProUGUI[] creatureCanvases_infoText;

    public ref Creature getCreatureInSlot(int slotIndex) {
        return ref CreatureManager.instance.playerCreatures[slotIndex];
    }

    void Start()
    {
        creatureCanvases_canvasImages = new Image[6];
        creatureCanvases_nameAndLevelText = new TMPro.TextMeshProUGUI[6];
        creatureCanvases_infoText = new TMPro.TextMeshProUGUI[6];
        for (int i = 0; i < 6; i++) {
            creatureCanvases_canvasImages[i] = creatureCanvases[i].transform.Find("creatureImage").GetComponent<Image>();
            creatureCanvases_nameAndLevelText[i] = creatureCanvases[i].transform.Find("nameAndLevel").GetComponent<TMPro.TextMeshProUGUI>();
            creatureCanvases_infoText[i] = creatureCanvases[i].transform.Find("creatureInfo").GetComponent<TMPro.TextMeshProUGUI>();
        }

        setButtonLambdas();

    }

    void setButtonLambdas() {

        creatureCanvases[1].transform.Find("moveRight").GetComponent<Button>().onClick.AddListener(()=>{swapLogic(1, true);});
        creatureCanvases[1].transform.Find("moveLeft").GetComponent<Button>().onClick.AddListener(()=>{swapLogic(1, false);});
        creatureCanvases[1].transform.Find("removeButton").GetComponent<Button>().onClick.AddListener(()=>{CreatureManager.instance.playerCreatures[1] = null;});

        creatureCanvases[2].transform.Find("moveRight").GetComponent<Button>().onClick.AddListener(()=>{swapLogic(2, true);});
        creatureCanvases[2].transform.Find("moveLeft").GetComponent<Button>().onClick.AddListener(()=>{swapLogic(2, false);});
        creatureCanvases[2].transform.Find("removeButton").GetComponent<Button>().onClick.AddListener(()=>{CreatureManager.instance.playerCreatures[2] = null;});

        creatureCanvases[3].transform.Find("moveRight").GetComponent<Button>().onClick.AddListener(()=>{swapLogic(3, true);});
        creatureCanvases[3].transform.Find("moveLeft").GetComponent<Button>().onClick.AddListener(()=>{swapLogic(3, false);});
        creatureCanvases[3].transform.Find("removeButton").GetComponent<Button>().onClick.AddListener(()=>{CreatureManager.instance.playerCreatures[3] = null;});

        creatureCanvases[4].transform.Find("moveRight").GetComponent<Button>().onClick.AddListener(()=>{swapLogic(4, true);});
        creatureCanvases[4].transform.Find("moveLeft").GetComponent<Button>().onClick.AddListener(()=>{swapLogic(4, false);});
        creatureCanvases[4].transform.Find("removeButton").GetComponent<Button>().onClick.AddListener(()=>{CreatureManager.instance.playerCreatures[4] = null;});

        creatureCanvases[5].transform.Find("moveRight").GetComponent<Button>().onClick.AddListener(()=>{swapLogic(5, true);});
        creatureCanvases[5].transform.Find("moveLeft").GetComponent<Button>().onClick.AddListener(()=>{swapLogic(5, false);});
        creatureCanvases[5].transform.Find("removeButton").GetComponent<Button>().onClick.AddListener(()=>{CreatureManager.instance.playerCreatures[5] = null;});

    }

    void swapLogic(int i, bool right) {

        int swapWith = i + (right?1:-1);
        if (swapWith >= 6) swapWith = 0;
        if (swapWith < 0) swapWith = 5;

        Creature c = CreatureManager.instance.playerCreatures[i];
        CreatureManager.instance.playerCreatures[i] = CreatureManager.instance.playerCreatures[swapWith];
        CreatureManager.instance.playerCreatures[swapWith] = c;

    }

    void Update()
    {
        for (int i = 0; i < CreatureManager.instance.playerCreatures.Length; i++) {
            if (CreatureManager.instance.playerCreatures[i] == null) {
                creatureCanvases[i].SetActive(false);
            } else {
                creatureCanvases[i].SetActive(true);
            }
        }

        updateCreatureDisplays();
    }

    void updateCreatureDisplays() {
        for (int i = 0; i < 6; i++) {
            if (CreatureManager.instance.playerCreatures[i] == null) continue;

            CreatureData creatureData = CreatureManager.instance.biomeCreatures[CreatureManager.instance.playerCreatures[i].getBiome()][(CreatureManager.instance.playerCreatures[i].getCreatureDataIndex())];
            creatureCanvases_canvasImages[i].sprite = creatureData.image;
            
            creatureCanvases_nameAndLevelText[i].text = "name - Lvl: " + CreatureManager.instance.playerCreatures[i].getLevel();

            creatureCanvases_infoText[i].text = "";
            creatureCanvases_infoText[i].text += "HP: " + CreatureManager.instance.playerCreatures[i].currentHP + " / " + CreatureManager.instance.playerCreatures[i].getMaxHP() + "\n";
            creatureCanvases_infoText[i].text += "Type: " + creatureData.type.ToString() + "\n";
            creatureCanvases_infoText[i].text += "Defence: " + creatureData.defence + "\n";
            creatureCanvases_infoText[i].text += "Speed: " + creatureData.speed + "\n";
            creatureCanvases_infoText[i].text += "Attack: " + creatureData.attack;

            // could change bg color/image based on element type or biome here


            // setting battle action text for equipped creature
            if (i == 0) {
                battleActionText[0].text = "<< " + CreatureManager.instance.playerCreatures[0].action1.name + " >>\n";
                battleActionText[0].text += "Element: " + CreatureManager.instance.playerCreatures[0].action1.element.ToString() + "\n";
                if (CreatureManager.instance.playerCreatures[0].action1.attackRatio > 0) battleActionText[0].text += "Damage: " + CreatureManager.instance.playerCreatures[0].getAttackDamage(CreatureManager.instance.playerCreatures[0].action1) + "\n";
                if (CreatureManager.instance.playerCreatures[0].action1.healRatio > 0) battleActionText[0].text += "Heal Amount: " + CreatureManager.instance.playerCreatures[0].getHealAmount(CreatureManager.instance.playerCreatures[0].action1) + "\n";

                battleActionText[1].text = "<< " + CreatureManager.instance.playerCreatures[0].action2.name + " >>\n";
                battleActionText[1].text += "Element: " + CreatureManager.instance.playerCreatures[0].action2.element.ToString() + "\n";
                if (CreatureManager.instance.playerCreatures[0].action2.attackRatio > 0) battleActionText[1].text += "Damage: " + CreatureManager.instance.playerCreatures[0].getAttackDamage(CreatureManager.instance.playerCreatures[0].action2) + "\n";
                if (CreatureManager.instance.playerCreatures[0].action2.healRatio > 0) battleActionText[1].text += "Heal Amount: " + CreatureManager.instance.playerCreatures[0].getHealAmount(CreatureManager.instance.playerCreatures[0].action2) + "\n";

                battleActionText[2].text = "<< " + CreatureManager.instance.playerCreatures[0].action3.name + " >>\n";
                battleActionText[2].text += "Element: " + CreatureManager.instance.playerCreatures[0].action3.element.ToString() + "\n";
                if (CreatureManager.instance.playerCreatures[0].action3.attackRatio > 0) battleActionText[2].text += "Damage: " + CreatureManager.instance.playerCreatures[0].getAttackDamage(CreatureManager.instance.playerCreatures[0].action3) + "\n";
                if (CreatureManager.instance.playerCreatures[0].action3.healRatio > 0) battleActionText[2].text += "Heal Amount: " + CreatureManager.instance.playerCreatures[0].getHealAmount(CreatureManager.instance.playerCreatures[0].action3) + "\n";
            }


        }

    }


}
