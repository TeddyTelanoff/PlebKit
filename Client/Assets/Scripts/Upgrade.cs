using System;

[Flags]
public enum Upgrade
{
	None = 0,
	Speed = 1 << 0,
	Bait = 1 << 1,
	Fish = 1 << 2,
	Value = 1 << 3,
	Backpack = 1 << 4,
}