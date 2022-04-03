using RiptideNetworking;

using UnityEngine;
using UnityEngine.UI;

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

    [Header("Activity Stuff")]
    public string[] activityNames;
    public GameObject activityButton;
    public Text activityText;

    public QuizScreen quizScreen;
    public QuizFeedbackScreen quizFeedbackScreen;

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

    public void DoActivity() {
        Player.localPlayer.GetComponent<PlayerActivity>().DoActivity();
    }

    public void QuizGuess(int guess) =>
        Player.localPlayer.GetComponent<PlayerQuiz>().Guess(guess);
}
