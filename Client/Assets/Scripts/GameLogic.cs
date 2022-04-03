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

    [Header("World")]
    public GameObject[] worlds;
    
    [Header("Prefabs")]
    public GameObject localPlayerPrefab;
    public GameObject otherPlayerPrefab;

    void Awake() {
        instance = this;
    }

    // todo more code
    public void SwitchWorlds(World oldWorld, World newWorld) {
        worlds[(ushort) oldWorld].SetActive(false);
        worlds[(ushort) newWorld].SetActive(true);
    }
}
