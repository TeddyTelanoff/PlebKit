using UnityEngine;

public enum World: ushort
{
    Lobby,
    Pier,
}

public class GameLogic: MonoBehaviour
{
    private static GameLogic _instance;
    public static GameLogic instance {
        get => _instance;
        private set {
            if (_instance is null)
                _instance = value;
            else if (_instance != value)
            {
                Debug.LogWarning($"instance of {nameof(GameLogic)} exists, destroying duplicate");
            }
        }
    }

    public Transform playerList;
    public World spawnWorld;

    public Question[] questions;

    public float[] fishSpeciesValue;

    [Header("Prefabs")]
    public GameObject playerPrefab;
    public GameObject playerUIPrefab;

    void OnValidate() {
        spawnWorld = World.Lobby;
    }

    void Awake() {
        instance = this;
    }
}
