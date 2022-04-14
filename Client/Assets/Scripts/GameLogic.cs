using UnityEngine;
using UnityEngine.UI;

public enum World: ushort
{
	Lobby,
	Pier,
}

public class GameLogic: MonoBehaviour
{
	public static GameLogic instance;

	public GameObject[] worlds;
	public GameObject supplyPanel;
	public Text supplyText;

	public GameObject upgradeScreen;

	[Header("Activity")]
	public string[] activityTexts;
	public GameObject activityButton;
	public Text activityText;
	
	[Header("Quiz")]
	public QuizScreen quizScreen;
	public QuizFeedbackScreen quizFeedbackScreen;

	[Header("Fish")]
	public FishScreen fishScreen;
	public FishSpecie[] fishSpecies;

	[Header("Prefabs")]
	public GameObject localPlayerPrefab;
	public GameObject otherPlayerPrefab;
	
	void Awake() {
		instance = this;
	}

#if UNITY_EDITOR
	void Start() {
		foreach (GameObject world in worlds)
			world.SetActive(false);
	}
#endif

	// todo more code
    public void SwitchWorlds(World newWorld) {
        worlds[(ushort) Player.localPlayer.world].SetActive(false);
        worlds[(ushort) newWorld].SetActive(true);
    }

	public void BackToLobby() {
		for (ushort i = 0; i < worlds.Length; i++)
			if (worlds[i])
				worlds[i].SetActive(i == (ushort) World.Lobby);
	}

    public void DoActivity() =>
        Player.localPlayer.GetComponent<PlayerActivity>().StartActivity();

    public void QuizGuess(int guess) =>
        Player.localPlayer.GetComponent<PlayerQuiz>().Guess(guess);
}
