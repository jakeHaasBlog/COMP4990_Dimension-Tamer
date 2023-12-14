using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System;

public class CreatureManager : MonoBehaviour
{
    public static CreatureManager instance;

    public Dictionary<BiomeID, List<CreatureData>> biomeCreatures;

    public BattleAction[] allActions = {
        new BattleAction("tackle", 0.4f, -1.0f, ElementalType.none),
        new BattleAction("punch", 0.6f, -1.0f, ElementalType.none),
        new BattleAction("eat snack", -1.0f, 0.6f, ElementalType.none),

        new BattleAction("fire breath", 0.5f, -1.0f, ElementalType.fire),
        new BattleAction("shoot lava", 0.7f, -1.0f, ElementalType.fire),
        new BattleAction("focus heat", 0.0f, 0.5f, ElementalType.fire),

        new BattleAction("healing fountain", -1.0f, 1.0f, ElementalType.water),
        new BattleAction("water gun", 0.2f, -1.0f, ElementalType.water),
        new BattleAction("power wash", 0.8f, -1.0f, ElementalType.water),

        new BattleAction("snap freeze", 0.5f, -1.0f, ElementalType.ice),
        new BattleAction("encase", -1.0f, 0.4f, ElementalType.ice),
        new BattleAction("shoot icicle", 0.6f, -1.0f, ElementalType.ice),

        new BattleAction("inject venom", 0.9f, -1.0f, ElementalType.poison),
        new BattleAction("toxic gas", 0.5f, -1.0f, ElementalType.poison),
        new BattleAction("solidify blood", -1.0f, 0.4f, ElementalType.poison),

        new BattleAction("steal life", 0.7f, 0.5f, ElementalType.dark),
        new BattleAction("spook", 0.4f, -1.0f, ElementalType.dark),
        new BattleAction("terrify", 0.7f, -1.0f, ElementalType.dark),
    };

    void Awake() {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private CreatureManager(){
        biomeCreatures = new Dictionary<BiomeID, List<CreatureData>>();
        foreach (BiomeID biome in Enum.GetValues(typeof(BiomeID))) {
            biomeCreatures[biome] = new List<CreatureData>();
        }
    }

}

public enum ElementalType {
    none = 0, 
    fire = 1,
    water = 2,
    ice = 3,
    poison = 4,
    dark = 5
}

public class BattleAction {
    public BattleAction(string name, float attackRatio, float healRatio, ElementalType element) {
        this.name = name;
        this.attackRatio = attackRatio;
        this.healRatio = healRatio;
        this.element = element;
    }

    public string name;
    public float attackRatio;
    public float healRatio;
    public ElementalType element;
}

public class CreatureData {
    public CreatureData(Texture2D image, ElementalType type, int maxHP, int defence, int speed, int attack) {
        this.image = image;
        this.type = type;
        this.maxHP = maxHP;
        this.defence = defence;
        this.speed = speed;
        this.attack = attack;
    }

    Texture2D image;
    public ElementalType type;
    public int maxHP;
    public int defence;
    public int speed;
    public int attack;
}

public class Creature {
    public Creature(BiomeID biome, int creatureDataIndex, BattleAction action1, BattleAction action2, BattleAction action3) {
        this.action1 = action1;
        this.action2 = action2;
        this.action3 = action3;

        this.biome = biome;
        this.creatureDataIndex = creatureDataIndex;

        currentHP = getMaxHP();
    }

    public int getMaxHP() {
        float hp = (float)CreatureManager.instance.biomeCreatures[biome][creatureDataIndex].maxHP;
        hp *= ((float)level / 10.0f) + 1.0f;
        return (int)hp;
    }

    public int getDefence() {
        return CreatureManager.instance.biomeCreatures[biome][creatureDataIndex].defence;
    }

    public int getSpeed() {
        return (int)((float)CreatureManager.instance.biomeCreatures[biome][creatureDataIndex].speed * ((float)level / 10.0f));
    }

    public int getAttackDamage(BattleAction action) {
        float dmg = (float)action.attackRatio * CreatureManager.instance.biomeCreatures[biome][creatureDataIndex].attack;
        dmg *= ((float)level / 10.0f) + 1.0f;
        return (int)dmg;
    }

    public int getHealAmount(BattleAction action) {
        float healing = (float)action.healRatio;
        healing *= ((float)level / 10.0f) + 1.0f;
        return (int)healing;
    }

    private BiomeID biome;
    private int creatureDataIndex;

    public int level;
    public int currentHP;
    public BattleAction action1;
    public BattleAction action2;
    public BattleAction action3;
}
