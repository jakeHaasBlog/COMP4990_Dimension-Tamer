using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System;


public enum ElementalType {
    none = 0, 
    fire = 1,
    water = 2,
    ice = 3,
    poison = 4,
    dark = 5,
    grass = 6,
}

public class CreatureManager : MonoBehaviour
{
    public static CreatureManager instance;

    public Dictionary<BiomeID, List<CreatureData>> biomeCreatures;
    public Dictionary<int, List<Tuple<BiomeID, int>>> encounterZones;
    public Dictionary<int, List<ElementalType>> encounterZoneElements;
    public Dictionary<int, int> encounterZoneLevel;

    public Creature[] playerCreatures = { null, null, null, null, null, null };

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

    public Creature generateRandomCreature() {
        BiomeID biome = (BiomeID)Enum.GetValues(typeof(BiomeID)).GetValue(UnityEngine.Random.Range(0, Enum.GetValues(typeof(BiomeID)).Length));
        int creatureIndex = UnityEngine.Random.Range(0, biomeCreatures[biome].Count);

        BattleAction action1 = allActions[UnityEngine.Random.Range(0, allActions.Length)];
        BattleAction action2 = allActions[UnityEngine.Random.Range(0, allActions.Length)];
        BattleAction action3 = allActions[UnityEngine.Random.Range(0, allActions.Length)];

        Creature creature = new Creature(biome, creatureIndex, action1, action2, action3, UnityEngine.Random.Range(0, 100));
        return creature;
    }

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
        encounterZones = new Dictionary<int, List<Tuple<BiomeID, int>>>();
        encounterZoneElements = new Dictionary<int, List<ElementalType>>();
        encounterZoneLevel = new Dictionary<int, int>();
    }

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
    public CreatureData(Texture2D tex, ElementalType type, int maxHP, int defence, int speed, int attack) {
        this.image = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        this.type = type;
        this.maxHP = maxHP;
        this.defence = defence;
        this.speed = speed;
        this.attack = attack;
    }

    public Sprite image;
    public ElementalType type;
    public int maxHP;
    public int defence;
    public int speed;
    public int attack;
}

public class Creature {
    public Creature(BiomeID biome, int creatureDataIndex, BattleAction action1, BattleAction action2, BattleAction action3, int level) {
        this.action1 = action1;
        this.action2 = action2;
        this.action3 = action3;

        this.biome = biome;
        this.creatureDataIndex = creatureDataIndex;
        this.level = level;

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

    public int getAttackDamage(BattleAction action, ElementalType against) {

        List<Tuple<ElementalType, ElementalType>> fourTimesEffective = new List<Tuple<ElementalType, ElementalType>>() 
        {
            Tuple.Create(ElementalType.grass, ElementalType.water),
            Tuple.Create(ElementalType.water, ElementalType.fire),
            Tuple.Create(ElementalType.fire, ElementalType.grass),

            Tuple.Create(ElementalType.poison, ElementalType.grass),
            Tuple.Create(ElementalType.poison, ElementalType.fire),

            Tuple.Create(ElementalType.ice, ElementalType.grass),
            Tuple.Create(ElementalType.ice, ElementalType.water),

            Tuple.Create(ElementalType.dark, ElementalType.water),
            Tuple.Create(ElementalType.dark, ElementalType.fire),
        }; 
        
        bool isFourTimesEffective = false;
        for (int i = 0; i < fourTimesEffective.Count; i++) {
            if (action.element == fourTimesEffective[i].Item1 && fourTimesEffective[i].Item2 == against) {
                isFourTimesEffective = true;
            }
        }

        List<Tuple<ElementalType, ElementalType>> twoTimesEffective = new List<Tuple<ElementalType, ElementalType>>() 
        {
            Tuple.Create(ElementalType.none, ElementalType.poison),
            Tuple.Create(ElementalType.none, ElementalType.ice),
            Tuple.Create(ElementalType.none, ElementalType.dark),
        }; 
        
        bool isTwoTimesEffective = false;
        for (int i = 0; i < twoTimesEffective.Count; i++) {
            if (action.element == twoTimesEffective[i].Item1 && twoTimesEffective[i].Item2 == against) {
                isTwoTimesEffective = true;
            }
        }
        
        if (isFourTimesEffective) return getAttackDamage(action) * 4;
        if (isTwoTimesEffective) return getAttackDamage(action) * 2;
        return getAttackDamage(action);

    }

    public int getHealAmount(BattleAction action) {
        float healing = (float)action.healRatio;
        healing *= ((float)level) + 1.0f;
        return (int)healing;
    }

    public int getCreatureDataIndex() {
        return creatureDataIndex;
    }

    public BiomeID getBiome() {
        return biome;
    }

    private BiomeID biome;
    private int creatureDataIndex;

    private int level;
    public int getLevel() {
        return level;
    }
    public int currentHP;
    public BattleAction action1;
    public BattleAction action2;
    public BattleAction action3;
}
