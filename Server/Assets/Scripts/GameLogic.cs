using UnityEngine;

public enum World: ushort
{
    Lobby,
    Pier,
}

public class GameLogic: MonoBehaviour
{
    public static GameLogic instance;

    public World spawnWorld;
    public Question[] questions;
    public FishSpecie[] fishSpecies;
    public float totalFishSpeciesChance;

    void OnValidate() {
        totalFishSpeciesChance = 0;
        foreach (FishSpecie fishSpecie in fishSpecies)
            totalFishSpeciesChance += fishSpecie.chance;
    }

    void Awake() {
        instance = this;
    }
}
