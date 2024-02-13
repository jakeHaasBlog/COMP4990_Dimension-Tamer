using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BattleMenu : MonoBehaviour
{
    
    public Image playerCreatureImage;
    public TMPro.TextMeshProUGUI playerCreatureHPText;
    public GameObject playerHPBar;
    private float playerHPBarMax;

    public Image enemyCreatureImage;
    public TMPro.TextMeshProUGUI enemyCreatureHPText;
    public TMPro.TextMeshProUGUI enemyCreatureTPText;
    public GameObject enemyHPBar;
    private float enemyHPBarMax;
    public GameObject enemyTPBar;
    private float enemyTPBarMax;
    private int enemyCreatureTP = 0;

    public TMPro.TextMeshProUGUI battleAction1Text;
    public TMPro.TextMeshProUGUI battleAction2Text;
    public TMPro.TextMeshProUGUI battleAction3Text;

    public TMPro.TextMeshProUGUI playerElementText;
    public TMPro.TextMeshProUGUI enemyElementText;

    public OverworldUI overworldUI;

    public Animator playerAttackAnimator;
    public Animator enemyAttackAnimator;


    private Creature enemyCreature;
    private BattleAction queuedPlayerBattleAction;
    BattleAction queuedEnemyBattleAction;
    bool waitingForInvokedAction = false; // prevents the usage of buttons while an animation is queued

    void Start()
    {
        enemyTPBarMax = enemyTPBar.transform.localScale.x;
        enemyHPBarMax = enemyHPBar.transform.localScale.x;
    }

    void Update()
    {
        CreatureData enemyCreatureData = CreatureManager.instance.biomeCreatures[enemyCreature.getBiome()][(enemyCreature.getCreatureDataIndex())];
        Creature playerCreature = CreatureManager.instance.playerCreatures[0];
        CreatureData playerCreatureData = CreatureManager.instance.biomeCreatures[playerCreature.getBiome()][(playerCreature.getCreatureDataIndex())];

        playerCreatureImage.sprite = playerCreatureData.image;
        playerCreatureHPText.text = "HP: " + playerCreature.currentHP + "/" + playerCreature.getMaxHP();

        String elementString = playerCreature.action1.element.ToString() + " : ";
        String dmgStr = "DMG " + (playerCreature.getAttackDamage(playerCreature.action1) * 0.9f) + " - " + (playerCreature.getAttackDamage(playerCreature.action1) * 1.1f);
        String healStr = "Heal" + (playerCreature.getHealAmount(playerCreature.action1) * 0.9f) + " - " + (playerCreature.getHealAmount(playerCreature.action1) * 1.1f);
        if (playerCreature.getAttackDamage(playerCreature.action1) <= 0) battleAction1Text.text = elementString + playerCreature.action1.name + " - " + healStr;
        else if (playerCreature.getHealAmount(playerCreature.action1) < 0) battleAction1Text.text = elementString + playerCreature.action1.name + " - " + dmgStr;
        else battleAction1Text.text = elementString + playerCreature.action1.name + " - " + dmgStr + " - " + healStr;

        elementString = playerCreature.action2.element.ToString() + " : ";
        dmgStr = "DMG " + (playerCreature.getAttackDamage(playerCreature.action2) * 0.9f) + " - " + (playerCreature.getAttackDamage(playerCreature.action2) * 1.1f);
        healStr = "Heal" + (playerCreature.getHealAmount(playerCreature.action2) * 0.9f) + " - " + (playerCreature.getHealAmount(playerCreature.action2) * 1.1f);
        if (playerCreature.getAttackDamage(playerCreature.action2) <= 0) battleAction2Text.text = elementString + playerCreature.action2.name + " - " + healStr;
        else if (playerCreature.getHealAmount(playerCreature.action2) < 0) battleAction2Text.text = elementString + playerCreature.action2.name + " - " + dmgStr;
        else battleAction2Text.text = elementString + playerCreature.action2.name + " - " + dmgStr + " - " + healStr;

        elementString = playerCreature.action3.element.ToString() + " : ";
        dmgStr = "DMG " + (playerCreature.getAttackDamage(playerCreature.action3) * 0.9f) + " - " + (playerCreature.getAttackDamage(playerCreature.action3) * 1.1f);
        healStr = "Heal" + (playerCreature.getHealAmount(playerCreature.action3) * 0.9f) + " - " + (playerCreature.getHealAmount(playerCreature.action3) * 1.1f);
        if (playerCreature.getAttackDamage(playerCreature.action3) <= 0) battleAction3Text.text = elementString + playerCreature.action3.name + " - " + healStr;
        else if (playerCreature.getHealAmount(playerCreature.action3) < 0) battleAction3Text.text = elementString + playerCreature.action3.name + " - " + dmgStr;
        else battleAction3Text.text = elementString + playerCreature.action3.name + " - " + dmgStr + " - " + healStr;
        
        enemyCreatureImage.sprite = enemyCreatureData.image;
        enemyCreatureHPText.text = "HP: " + enemyCreature.currentHP + "/" + enemyCreature.getMaxHP();

        playerElementText.text = "Element - " + playerCreatureData.type.ToString();
        enemyElementText.text = "Element - " + enemyCreatureData.type.ToString();

        // hp and tranq bars
        Vector3 playerHealthBarScale = playerHPBar.transform.localScale;
        playerHealthBarScale.x = playerHPBarMax * ((float)playerCreature.currentHP / playerCreature.getMaxHP());
        playerHPBar.transform.localScale = playerHealthBarScale;

        Vector3 enemyHealthBarScale = enemyHPBar.transform.localScale;
        enemyHealthBarScale.x = enemyHPBarMax * ((float)enemyCreature.currentHP / enemyCreature.getMaxHP());
        enemyTPBar.transform.localScale = enemyHealthBarScale;

        Vector3 enemyTranqBarScale = enemyTPBar.transform.localScale;
        enemyTranqBarScale.x = enemyTPBarMax * ((float)enemyCreatureTP / 100.0f);
        enemyTPBar.transform.localScale = enemyTranqBarScale;
    }

    public void beginEncounter(int encounterZoneID) {
        waitingForInvokedAction = false;

        overworldUI.switchToBattleScene();
        enemyCreatureTP = 0;
        Tuple<BiomeID, int> enemyCreatureDataIndex = CreatureManager.instance.encounterZones[encounterZoneID][UnityEngine.Random.Range(0, CreatureManager.instance.encounterZones[encounterZoneID].Count)];
        List<BattleAction> possibleActions = new List<BattleAction>();
        foreach (BattleAction action in CreatureManager.instance.allActions) {
            if (CreatureManager.instance.encounterZoneElements[encounterZoneID].Contains(action.element)) {
                possibleActions.Add(action);
            }
        }
        
        int r = UnityEngine.Random.Range(0, possibleActions.Count);
        BattleAction ba1 = possibleActions[r];
        possibleActions.RemoveAt(r);

        r = UnityEngine.Random.Range(0, possibleActions.Count);
        BattleAction ba2 = possibleActions[r];
        possibleActions.RemoveAt(r);

        r = UnityEngine.Random.Range(0, possibleActions.Count);
        BattleAction ba3 = possibleActions[r];
        possibleActions.RemoveAt(r);


        int level = CreatureManager.instance.encounterZoneLevel[encounterZoneID];
        level += UnityEngine.Random.Range(-5, 5);
        if (level < 1) level = 1;
        if (level > 100) level = 100;
        enemyCreature = new Creature(enemyCreatureDataIndex.Item1, enemyCreatureDataIndex.Item2, ba1, ba2, ba3, level);
    }

    void usePlayerBattleAction() {
        BattleAction action = queuedPlayerBattleAction;

        if (CreatureManager.instance.playerCreatures[0] == null) return;
        if (CreatureManager.instance.playerCreatures[0].currentHP <= 0) return;

        CreatureData enemyCreatureData = CreatureManager.instance.biomeCreatures[enemyCreature.getBiome()][(enemyCreature.getCreatureDataIndex())];
        int dmg = CreatureManager.instance.playerCreatures[0].getAttackDamage(action, enemyCreatureData.type);
        int heal = CreatureManager.instance.playerCreatures[0].getHealAmount(action);
        CreatureData playerCreatureData = CreatureManager.instance.biomeCreatures[CreatureManager.instance.playerCreatures[0].getBiome()][(CreatureManager.instance.playerCreatures[0].getCreatureDataIndex())];

        if (heal > 0) {
            CreatureManager.instance.playerCreatures[0].currentHP += heal;
            if (CreatureManager.instance.playerCreatures[0].currentHP > CreatureManager.instance.playerCreatures[0].getMaxHP())
                CreatureManager.instance.playerCreatures[0].currentHP = CreatureManager.instance.playerCreatures[0].getMaxHP();
        }

        if (dmg > 0) {
            enemyCreature.currentHP -= dmg;
            if (enemyCreature.currentHP <= 0) {
                enemyCreature.currentHP = 0;

                endEncounter();

            }
        }

        if (dmg > 0) {
            playerAttackAnimator.Play("CreatureAttack");
        } else {
            playerAttackAnimator.Play("CreatureHeal");
        }

        waitingForInvokedAction = false;
        CancelInvoke("usePlayerBattleAction");
    }

    void enemyCreatureTurn() {

        if (enemyCreature.currentHP <= 0) return;

        int n = UnityEngine.Random.Range(0, 3);
        if (n == 0) {
            queuedEnemyBattleAction = enemyCreature.action1;
        } else if (n == 1) {
            queuedEnemyBattleAction = enemyCreature.action2;
        } else if (n == 2) {
            queuedEnemyBattleAction = enemyCreature.action3;
        }

        useEnemyBattleAction();
        waitingForInvokedAction = false;
        CancelInvoke("enemyCreatureTurn");
    }

    void useEnemyBattleAction() {
        if (enemyCreature == null) return;
        BattleAction action = queuedEnemyBattleAction;

        int dmg = enemyCreature.getAttackDamage(action);
        int heal = enemyCreature.getHealAmount(action);
        CreatureData enemyCreatureData = CreatureManager.instance.biomeCreatures[enemyCreature.getBiome()][(enemyCreature.getCreatureDataIndex())];

        if (dmg > 0) {
            CreatureManager.instance.playerCreatures[0].currentHP -= dmg;
            if (CreatureManager.instance.playerCreatures[0].currentHP <= 0) {
                playerCreatureDefeated();
            }
        }

        if (heal > 0) {
            enemyCreature.currentHP += heal;
            if (enemyCreature.currentHP > enemyCreature.getMaxHP())
                enemyCreature.currentHP = enemyCreature.getMaxHP();
        }

        if (dmg > 0) {
            enemyAttackAnimator.Play("CreatureAttack");
        } else {
            enemyAttackAnimator.Play("CreatureHeal");
        }

    }

    void playerCreatureDefeated() {
        CreatureManager.instance.playerCreatures[0] = null;

        for (int i = 0; i < CreatureManager.instance.playerCreatures.Length; i++) {
            if (CreatureManager.instance.playerCreatures[i] != null) {
                // swap next valid creature into equip slot and continue battle

                CreatureManager.instance.playerCreatures[0] = CreatureManager.instance.playerCreatures[i];
                CreatureManager.instance.playerCreatures[i] = null;

                return;
            }
        }

        // Player has lost. All of their creatures have been defeated, so they have no way to win the game
        // TODO: implement this, for now just exit encounter
        endEncounter();
    }

    void endEncounter() {
        overworldUI.switchToOverworld();
    }

    public void battleAction1Clicked() {
        if (waitingForInvokedAction) return;

        float startAfterSeconds = 2.0f;
        queuedPlayerBattleAction = CreatureManager.instance.playerCreatures[0].action1;

        if (CreatureManager.instance.playerCreatures[0].getSpeed() >= enemyCreature.getSpeed()) {
            usePlayerBattleAction();
            InvokeRepeating("enemyCreatureTurn", startAfterSeconds, 100000);
            waitingForInvokedAction = true;
        } else {
            enemyCreatureTurn();
            queuedPlayerBattleAction = CreatureManager.instance.playerCreatures[0].action1;
            InvokeRepeating("usePlayerBattleAction", startAfterSeconds, 100000);
            waitingForInvokedAction = true;
        }
    }

    public void battleAction2Clicked() {
        if (waitingForInvokedAction) return;

        float startAfterSeconds = 2.0f;
        queuedPlayerBattleAction = CreatureManager.instance.playerCreatures[0].action2;
        if (CreatureManager.instance.playerCreatures[0].getSpeed() >= enemyCreature.getSpeed()) {
            usePlayerBattleAction();
            InvokeRepeating("enemyCreatureTurn", startAfterSeconds, 100000);
            waitingForInvokedAction = true;
        } else {
            enemyCreatureTurn();
            InvokeRepeating("usePlayerBattleAction", startAfterSeconds, 100000);
            waitingForInvokedAction = true;
        }
    }

    public void battleAction3Clicked() {
        if (waitingForInvokedAction) return;

        float startAfterSeconds = 2.0f;
        queuedPlayerBattleAction = CreatureManager.instance.playerCreatures[0].action3;
        if (CreatureManager.instance.playerCreatures[0].getSpeed() >= enemyCreature.getSpeed()) {
            usePlayerBattleAction();
            InvokeRepeating("enemyCreatureTurn", startAfterSeconds, 100000);
            waitingForInvokedAction = true;
        } else {
            enemyCreatureTurn();
            InvokeRepeating("usePlayerBattleAction", startAfterSeconds, 100000);
            waitingForInvokedAction = true;
        }
    }

    public void tranqClicked() {
        if (waitingForInvokedAction) return;

        useTranquilizer();
        float startAfterSeconds = 2.0f;
        waitingForInvokedAction = true;
        InvokeRepeating("enemyCreatureTurn", startAfterSeconds, 100000);
    }

    public void useTranquilizer() {

        float tranqEffectiveness = 70.0f; // must be a positive number, 100 would fill bar if enemy had 1% hp
        int tranqMin = 10;
        enemyCreatureTP += (int)((1.0f - ((float)enemyCreature.currentHP / enemyCreature.getMaxHP())) * tranqEffectiveness) + tranqMin;
        Debug.Log("current hp: " + enemyCreature.currentHP + "  max hp: " + enemyCreature.getMaxHP());

        if (enemyCreatureTP < 100) return;

        int i = 0;
        for (i = 0; i < 6; i++) {
            if (CreatureManager.instance.playerCreatures[i] == null) {
                CreatureManager.instance.playerCreatures[i] = enemyCreature;
                CreatureData newCreatureData = CreatureManager.instance.biomeCreatures[CreatureManager.instance.playerCreatures[i].getBiome()][(CreatureManager.instance.playerCreatures[i].getCreatureDataIndex())];
                CreatureManager.instance.playerCreatures[i].currentHP = enemyCreature.getMaxHP();

                break;
            }
        }

        if (i == 7) {
            // no empty slots were in inventory, creature not captured
        }

        endEncounter();

    }

}
