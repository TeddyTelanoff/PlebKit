using UnityEngine;

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

    [Header("Prefabs")]
    public GameObject localPlayerPrefab;
    public GameObject otherPlayerPrefab;

    void Awake() {
        instance = this;
    }
}
