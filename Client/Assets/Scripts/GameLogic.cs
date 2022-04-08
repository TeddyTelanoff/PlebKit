using UnityEngine;

public class GameLogic: MonoBehaviour
{
	public static GameLogic instance;

	public Camera mainCamera;

	[Header("Prefabs")]
	public GameObject localPlayerPrefab;
	public GameObject otherPlayerPrefab;
	
	void Awake() {
		instance = this;
	}
}
