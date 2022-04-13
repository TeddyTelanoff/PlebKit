using PacketExt;

using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerQuiz))]
[RequireComponent(typeof(Collider))]
public class Player : MonoBehaviour
{
	public Client client;

	public PlayerMovement movement;
	public PlayerListItem listItem;
	public PlayerQuiz quiz;
	public PlayerFish fish;

	public string username;
	public World world;

	public float money;

	public Upgrade upgrades;

	void OnValidate() {
		if (client == null)
			client = GetComponentInParent<Client>();
		
		if (movement == null)
			movement = GetComponent<PlayerMovement>();
		
		if (quiz == null)
			quiz = GetComponent<PlayerQuiz>();
		
		if (fish == null)
			fish = GetComponent<PlayerFish>();
	}

	void OnDestroy() {
		PlayerListManager.instance.RemoveItem(listItem);
	}

	public void SwitchWorldsAndSend(World newWorld) {
		Packet packet = Packet.Create(ServerToClient.SwitchWorlds);
		packet.AddUShort(client.id);
		packet.AddUShort((ushort) newWorld);

		world = newWorld;
		transform.position = Vector3.zero;
		Server.instance.SendAll(packet);
	}

	public static void Spawn(ushort clientId, string username) {
		Client client = Server.instance.clients[clientId];
		client.player.gameObject.SetActive(true);
		client.player.username = username;
		client.player.name = $"{username} #{clientId}";
		
		PlayerListManager.instance.AddItem(client.player);
	}

	public void SendInventoryUpdate() {
		Packet packet = Packet.Create(ServerToClient.InventoryUpdate);
		packet.AddFloat(money);
		packet.AddInt(fish.bait);
		packet.AddInts(fish.fishes);
		Server.instance.Send(packet, client.id);
	}

	public void Upgrade(Upgrade upgrade) {
		int val = (int) upgrade;
		
		int i;
		for (i = 0; i < sizeof(Upgrade) * 8; i++)
		{
			if ((val & 1) == 1)
			{
				if (money >= GameLogic.instance.upgradeInfos[i].cost)
				{
					upgrades |= (Upgrade) (val << i);
					money -= GameLogic.instance.upgradeInfos[i].cost;
				}
			}
			
			val >>= 1;
		}

		SendUpgradeInfo();
	}

	public void SendUpgradeInfo() {
		Packet packet = Packet.Create(ServerToClient.UpgradeResult);
		packet.AddInt((int) upgrades);
		packet.AddFloat(money);
		Server.instance.Send(packet, client.id);
	}

	[PacketHandler(ClientToServer.Quiz)]
	static void Quiz(ushort clientId, Packet packet) {
		if (Server.instance.clients.TryGetValue(clientId, out Client client))
		{
			client.player.quiz.NewQuestion();
			client.player.quiz.SendQuestion();
		}
	}

	[PacketHandler(ClientToServer.QuizGuess)]
	static void QuizGuess(ushort clientId, Packet packet) {
		if (Server.instance.clients.TryGetValue(clientId, out Client client))
			client.player.quiz.Guess(packet.GetInt());
	}

	[PacketHandler(ClientToServer.Fish)]
	static void Fish(ushort clientId, Packet packet) {
		if (Server.instance.clients.TryGetValue(clientId, out Client client))
			client.player.fish.GoFishing();
	}

	[PacketHandler(ClientToServer.SellFish)]
	static void SellFish(ushort clientId, Packet packet) {
		if (Server.instance.clients.TryGetValue(clientId, out Client client))
			client.player.fish.SellFish();
	}

	[PacketHandler(ClientToServer.Upgrade)]
	static void OnUpgrade(ushort clientId, Packet packet) {
		if (Server.instance.clients.TryGetValue(clientId, out Client client))
		{
			Upgrade upgrade = (Upgrade) packet.GetInt();
			client.player.Upgrade(upgrade);
		}
	}
}
