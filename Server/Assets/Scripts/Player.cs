using System;
using System.Collections.Generic;

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

	public Dictionary<UpgradePath, int> upgradeStatus;

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

	void Awake() {
		upgradeStatus = new Dictionary<UpgradePath, int>();
		for (byte i = 0; i < (byte) UpgradePath.Count; i++)
			upgradeStatus.Add((UpgradePath) i, 0);
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

	public (UpgradeInfo info, bool max) GetUpgradeInfo(UpgradePath path) {
		int idx = upgradeStatus[path];
		UpgradeInfo[] infos = GameLogic.instance.GetUpgradeInfos(path);

		if (idx >= infos.Length)
			return (infos[infos.Length - 1], true);

		return (infos[idx], false);
	}

	void ApplyUpgrade(UpgradePath path, UpgradeInfo info) {
		switch (path)
		{
		case UpgradePath.Speed:
			movement.speed *= info.effect;
			break;
		case UpgradePath.Bait:
			quiz.baitPerQuestion += (int) info.effect;
			break;
		case UpgradePath.Fish:
			fish.luck *= info.effect;
			break;
		case UpgradePath.Value:
			fish.value *= info.effect;
			break;
		case UpgradePath.Backpack:
			fish.fishCapactiyPerSpecie += (int) info.effect;
			break;
		case UpgradePath.FishTime:
			fish.waitTime *= info.effect;
			break;
		
		default:
			throw new Exception("ye, mate that upgrade path no exist");
		}

		upgradeStatus[path]++;
		SendUpgradeResult();
		SendUpgradeInfo(path);
	}

	void SendUpgradeResult() {
		Packet packet = Packet.Create(ServerToClient.UpgradeResult);
		packet.AddFloat(money);
		packet.AddFloat(movement.speed);
		// packet.AddInt(quiz.baitPerQuestion);
		// packet.AddInt(fish.fishPerCatch);
		// packet.AddFloat(fish.valueBonus);
		// packet.AddInt(fish.fishCapactiyPerSpecie);
		Server.instance.Send(packet, client.id);
	}
	
	public void SendUpgradeInfo(UpgradePath path) {
		Packet packet = Packet.Create(ServerToClient.UpgradeInfo);
		(UpgradeInfo info, bool max) = GetUpgradeInfo(path);
		if (max)
		{
			packet.AddByte((byte) path);
			packet.AddBool(true);
		}
		else
		{
			packet.AddByte((byte) path);
			packet.AddBool(false);
			packet.AddFloat(info.cost);
			packet.AddFloat(info.effect);

			string op = path switch {
				UpgradePath.Bait => $"+{info.effect}",
				UpgradePath.Backpack => $"+{info.effect}",
				_ => $"{info.effect}x",
			};
			
			packet.AddString(op);
		}

		Server.instance.Send(packet, client.id);
	}

	public void Upgrade(UpgradePath path) {
		(UpgradeInfo info, bool max) = GetUpgradeInfo(path);

		if (max)
			return; // nice try teddy
		
		if (money < info.cost)
		{
			SendUpgradeResult();
			return;
		}

		money -= info.cost;
		ApplyUpgrade(path, info);
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
			UpgradePath path = (UpgradePath) packet.GetByte();
			client.player.Upgrade(path);
		}
	}

	[PacketHandler(ClientToServer.UpgradeInfo)]
	static void UpgradeInfo(ushort clientId, Packet packet) {
		if (Server.instance.clients.TryGetValue(clientId, out Client client))
		{
			UpgradePath path = (UpgradePath) packet.GetByte();
			client.player.SendUpgradeInfo(path);
		}
	}
}
