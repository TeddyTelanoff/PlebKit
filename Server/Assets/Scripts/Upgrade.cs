using System;

public enum UpgradePath: byte
{
	Speed,
	Bait,
	Fish,
	Value,
	Backpack,
	FishTime,
	
	Count,
}

[Serializable]
public struct UpgradeInfo
{
	public float cost;
	public float effect;
}