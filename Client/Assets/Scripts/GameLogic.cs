using UnityEngine;

public class GameLogic: MonoBehaviour
{
	public static GameLogic instance;

	[Header("Prefabs")]
	public GameObject localPlayerPrefab;
	public GameObject otherPlayerPrefab;
	
	void Awake() {
		instance = this;
	}
}
