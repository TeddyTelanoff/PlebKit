using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton: MonoBehaviour
{
	public static Dictionary<UpgradePath, UpgradeButton> buttons = new Dictionary<UpgradePath, UpgradeButton>();

	public UpgradePath path;
	public Text text;

	public string displayText;

	void OnValidate() {
		text.text = displayText;
	}

	void Awake() {
		if (!buttons.ContainsKey(path))
			buttons.Add(path, this);

		UpdateText();
	}

	public void SendBuy() {
		Packet packet = Packet.Create(ClientToServer.Upgrade);
		packet.AddByte((byte) path);
		Client.instance.Send(packet);
	}

	public void UpdateText() {
		Packet packet = Packet.Create(ClientToServer.UpgradeInfo);
		packet.AddByte((byte) path);
		Client.instance.Send(packet);
	}

	[PacketHandler(ServerToClient.UpgradeInfo)]
	public static void UpgradeInfo(Packet packet) {
		UpgradePath path = (UpgradePath) packet.GetByte();
		bool max = packet.GetBool();
		if (max)
		{
			buttons[path].text.text = $"{buttons[path].displayText} MAXED OUT";
		}
		else
		{
			float cost = packet.GetFloat();
			float effect = packet.GetFloat();
			string op = packet.GetString();
			buttons[path].text.text = $"{op} {buttons[path].displayText}  ${cost}";
		}
	}
}
