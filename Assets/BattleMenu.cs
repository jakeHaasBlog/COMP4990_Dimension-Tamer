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

    public Image enemyCreatureImage;
    public TMPro.TextMeshProUGUI enemyCreatureHPText;
    public TMPro.TextMeshProUGUI enemyCreatureTPText;
    public GameObject enemyHPBar;
    public GameObject enemyTPBar;

    public TMPro.TextMeshProUGUI battleAction1Text;
    public TMPro.TextMeshProUGUI battleAction2Text;
    public TMPro.TextMeshProUGUI battleAction3Text;

    public OverworldUI overworldUI;


    private Creature enemyCreature;

    void Start()
    {
        
    }

    void Update()
    {
        CreatureData enemyCreatureData = CreatureManager.instance.biomeCreatures[enemyCreature.getBiome()][(enemyCreature.getCreatureDataIndex())];
        Creature playerCreature = CreatureManager.instance.playerCreatures[0];
        CreatureData playerCreatureData = CreatureManager.instance.biomeCreatures[playerCreature.getBiome()][(playerCreature.getCreatureDataIndex())];

        playerCreatureImage.sprite = playerCreatureData.image;
        playerCreatureHPText.text = "HP: " + playerCreature.currentHP + "/" + playerCreatureData.maxHP;
        battleAction1Text.text = playerCreature.action1.name + " - DMG: " + playerCreature.getAttackDamage(playerCreature.action1) + " HP: " + playerCreature.getHealAmount(playerCreature.action1);
        battleAction2Text.text = playerCreature.action2.name + " - DMG: " + playerCreature.getAttackDamage(playerCreature.action2) + " HP: " + playerCreature.getHealAmount(playerCreature.action2);
        battleAction3Text.text = playerCreature.action3.name + " - DMG: " + playerCreature.getAttackDamage(playerCreature.action3) + " HP: " + playerCreature.getHealAmount(playerCreature.action3);

        enemyCreatureImage.sprite = enemyCreatureData.image;
        enemyCreatureHPText.text = "HP: " + enemyCreature.currentHP + "/" + enemyCreatureData.maxHP;
    }

    public void beginEncounter(int encounterZoneID) {
        overworldUI.switchToBattleScene();
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


        enemyCreature = new Creature(enemyCreatureDataIndex.Item1, enemyCreatureDataIndex.Item2, ba1, ba2, ba3);
        enemyCreature.level = CreatureManager.instance.encounterZoneLevel[encounterZoneID];
        enemyCreature.level += UnityEngine.Random.Range(-5, 5);
        if (enemyCreature.level < 1) enemyCreature.level = 1;
        if (enemyCreature.level > 100) enemyCreature.level = 100;
    }

    void usePlayerBattleAction(BattleAction action) {
        if (CreatureManager.instance.playerCreatures[0] == null) return;
        if (CreatureManager.instance.playerCreatures[0].currentHP <= 0) return;

        int dmg = CreatureManager.instance.playerCreatures[0].getAttackDamage(action);
        int heal = CreatureManager.instance.playerCreatures[0].getHealAmount(action);
        CreatureData playerCreatureData = CreatureManager.instance.biomeCreatures[CreatureManager.instance.playerCreatures[0].getBiome()][(CreatureManager.instance.playerCreatures[0].getCreatureDataIndex())];

        if (heal > 0) {
            CreatureManager.instance.playerCreatures[0].currentHP += heal;
            if (CreatureManager.instance.playerCreatures[0].currentHP > playerCreatureData.maxHP)
                CreatureManager.instance.playerCreatures[0].currentHP = playerCreatureData.maxHP;
        }

        if (dmg > 0) {
            enemyCreature.currentHP -= dmg;
            if (enemyCreature.currentHP <= 0) {
                enemyCreature.currentHP = 0;

                endEncounter();

            }
        }
    }

    void enemyCreatureTurn() {
        if (enemyCreature.currentHP <= 0) return;

        int n = UnityEngine.Random.Range(0, 3);
        if (n == 0) {
            useEnemyBattleAction(enemyCreature.action1);
        } else if (n == 1) {
            useEnemyBattleAction(enemyCreature.action2);
        } else if (n == 2) {
            useEnemyBattleAction(enemyCreature.action3);
        }
    }

    void useEnemyBattleAction(BattleAction action) {
        if (enemyCreature == null) return;

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
            if (enemyCreature.currentHP > enemyCreatureData.maxHP)
                enemyCreature.currentHP = enemyCreatureData.maxHP;
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
        if (CreatureManager.instance.playerCreatures[0].getSpeed() >= enemyCreature.getSpeed()) {
            usePlayerBattleAction(CreatureManager.instance.playerCreatures[0].action1);
            enemyCreatureTurn();
        } else {
            enemyCreatureTurn();
            usePlayerBattleAction(CreatureManager.instance.playerCreatures[0].action1);
        }
    }

    public void battleAction2Clicked() {
        if (CreatureManager.instance.playerCreatures[0].getSpeed() >= enemyCreature.getSpeed()) {
            usePlayerBattleAction(CreatureManager.instance.playerCreatures[0].action2);
            enemyCreatureTurn();
        } else {
            enemyCreatureTurn();
            usePlayerBattleAction(CreatureManager.instance.playerCreatures[0].action2);
        }
    }

    public void battleAction3Clicked() {
        if (CreatureManager.instance.playerCreatures[0].getSpeed() >= enemyCreature.getSpeed()) {
            usePlayerBattleAction(CreatureManager.instance.playerCreatures[0].action3);
            enemyCreatureTurn();
        } else {
            enemyCreatureTurn();
            usePlayerBattleAction(CreatureManager.instance.playerCreatures[0].action3);
        }
    }

    public void tranquilizerUsed() {

        // TODO: Make tranquilization bar increase based on the enemy's (health / maxHealth)

        int i = 0;
        for (i = 0; i < 6; i++) {
            if (CreatureManager.instance.playerCreatures[i] == null) {
                CreatureManager.instance.playerCreatures[i] = enemyCreature;
                CreatureData newCreatureData = CreatureManager.instance.biomeCreatures[CreatureManager.instance.playerCreatures[i].getBiome()][(CreatureManager.instance.playerCreatures[i].getCreatureDataIndex())];
                CreatureManager.instance.playerCreatures[i].currentHP = newCreatureData.maxHP;

                break;
            }
        }

        if (i == 7) {
            // no empty slots were in inventory, creature not captured
        }

        endEncounter();

    }


}
