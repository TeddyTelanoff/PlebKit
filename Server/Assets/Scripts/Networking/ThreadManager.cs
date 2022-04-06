using System;
using System.Collections.Generic;

using UnityEngine;

public class ThreadManager: MonoBehaviour
{
	static readonly List<Action> toExecute = new List<Action>();
	static readonly List<Action> toExecuteCopy = new List<Action>();
	static bool thingsTodo = false;
	
	void FixedUpdate() {
		if (!thingsTodo)
			return;

		toExecuteCopy.Clear();
		lock (toExecute)
		{
			toExecuteCopy.AddRange(toExecute);
			toExecute.Clear();
			thingsTodo = false;
		}

		foreach (Action todo in toExecuteCopy)
			todo(); // this line is counter-intuitive--it's like a big T0DO when coding
	}

	public static void ExecuteOnMainThread(Action action) {
		lock (toExecute)
		{
			toExecute.Add(action);
			thingsTodo = true;
		}
	}
}
