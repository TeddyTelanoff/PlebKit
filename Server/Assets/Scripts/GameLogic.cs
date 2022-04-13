using UnityEngine;

public enum World: ushort
{
    Lobby,
    Pier,
}

public class GameLogic: MonoBehaviour
{
    public static GameLogic instance;

    public World spawnWorld;
    public Question[] questions;
    public FishSpecie[] fishSpecies;
    public float totalFishSpeciesChance;
    public UpgradeInfo[] upgradeInfos;

    void OnValidate() {
        totalFishSpeciesChance = 0;
        foreach (FishSpecie fishSpecie in fishSpecies)
            totalFishSpeciesChance += fishSpecie.chance;
    }

    void Awake() {
        instance = this;
    }

    [PacketHandler(ClientToServer.UpgradeInfo)]
    static void UpgradeInfo(ushort clientId, Packet packet) {
        int val = packet.GetInt(); // <-- upgrade
        
        int i = 0;
        for (; i < sizeof(Upgrade) * 8; i++)
            if (val << i == 1)
                break;
        SendUpgradeInfo(i, clientId);
    }

    static void SendUpgradeInfo(int i, ushort clientId) {
        Packet packet = Packet.Create(ServerToClient.UpgradeInfo);
        packet.AddString(instance.upgradeInfos[i].name);
        packet.AddFloat(instance.upgradeInfos[i].cost);
    }
}
