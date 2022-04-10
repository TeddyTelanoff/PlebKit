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
    
    void Awake() {
        instance = this;
    }
}
