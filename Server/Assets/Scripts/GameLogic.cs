using System;
using System.Collections.Generic;

using UnityEngine;

public enum World: ushort
{
    Lobby,
    Pier,
}

public class GameLogic: MonoBehaviour
{
    public static GameLogic instance;

    public PlayerMenu playerMenu;
    
    public World spawnWorld;
    public Question[] questions;
    public FishSpecie[] fishSpecies;
    public float totalFishSpeciesChance;
    
    [Header("Upgrades")]
    public UpgradeInfo[] speedUpgrades;
    public UpgradeInfo[] baitUpgrades;
    public UpgradeInfo[] fishUpgrades;
    public UpgradeInfo[] valueUpgrades;
    public UpgradeInfo[] backpackUpgrades;
    public UpgradeInfo[] fishTimeUpgrades;

    public UpgradeInfo[] GetUpgradeInfos(UpgradePath path) {
        return path switch {
            UpgradePath.Speed => speedUpgrades,
            UpgradePath.Bait => baitUpgrades,
            UpgradePath.Fish => fishUpgrades,
            UpgradePath.Value => valueUpgrades,
            UpgradePath.Backpack => backpackUpgrades,
            UpgradePath.FishTime => fishTimeUpgrades,
            
            _ => throw new Exception("no upgrade path, mate"),
        };
    }
    
    void OnValidate() {
        totalFishSpeciesChance = 0;
        foreach (FishSpecie fishSpecie in fishSpecies)
            totalFishSpeciesChance += fishSpecie.chance;
    }

    void Awake() {
        instance = this;
    }
}
