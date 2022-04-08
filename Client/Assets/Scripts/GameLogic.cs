using UnityEngine;

public enum World: ushort
{
	Lobby,
	Pier,
}

public class GameLogic: MonoBehaviour
{
	public static GameLogic instance;

	public GameObject[] worlds;

	[Header("Prefabs")]
	public GameObject localPlayerPrefab;
	public GameObject otherPlayerPrefab;
	
	void Awake() {
		instance = this;
	}
	
	// todo more code
    public void SwitchWorlds(World newWorld) {
        worlds[(ushort) Player.localPlayer.world].SetActive(false);
        worlds[(ushort) newWorld].SetActive(true);
    }
}
