using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEngine;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class PacketHandlerAttribute: Attribute
{
	public readonly ServerToClient id;

	public PacketHandlerAttribute(ServerToClient id) {
		this.id = id;
	}
}

public static class PacketHandling
{
	public delegate void PacketHandler(Packet packet);

	public static Dictionary<ServerToClient, PacketHandler> handlers = new Dictionary<ServerToClient, PacketHandler>()
	{
		{ ServerToClient.Welcome, Client.OnWelcome },
		{ ServerToClient.Spawn, Player.OnSpawn },
		{ ServerToClient.Disconnect, Player.OnDisconnect },
		{ ServerToClient.PlayerMovement, Player.PlayerMovement },
		{ ServerToClient.SwitchWorlds, Player.SwitchWorlds },
		{ ServerToClient.Question, PlayerQuiz.Question },
		{ ServerToClient.QuestionFeedback, PlayerQuiz.QuestionFeedback },
	};

	// when will my reflection show....
	// but reflection doesn't work in webgl builds ig
	public static void VerifyDictionary() {
		MethodInfo[] methods = Assembly.GetCallingAssembly()
									   .GetTypes()
									   .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
									   .Where(m => m.GetCustomAttributes(typeof(PacketHandlerAttribute), false).Length > 0)
									   .ToArray();
		
		Dictionary<ServerToClient, PacketHandler> foundHandlers = new Dictionary<ServerToClient, PacketHandler>();
		
		foreach (MethodInfo method in methods)
		{
			PacketHandlerAttribute attribute = method.GetCustomAttribute<PacketHandlerAttribute>();
			
			if (!method.IsStatic)
				throw new Exception($"packet handler {method.DeclaringType}.{method.Name} is not static");
			
			Delegate handler = Delegate.CreateDelegate(typeof(PacketHandler), method, false);

			if (handler != null)
			{
				if (foundHandlers.TryGetValue(attribute.id, out PacketHandler twin))
				{
					MethodInfo twinMethod = twin.GetMethodInfo();

					throw new Exception(
						$"to method-handlers with same packet id: {method.DeclaringType}.{method.Name} & {twinMethod.DeclaringType}.{twinMethod.Name}"
					);
				}
				
				foundHandlers.Add(attribute.id, (PacketHandler) handler);
				if (!handlers.ContainsValue((PacketHandler) handler))
					Debug.LogWarning($"no packet handler for {method.DeclaringType}.{method.Name}");
			}
			else
				throw new Exception($"{method.DeclaringType}.{method.Name} has incorrect signature");
		}
		
		foreach (KeyValuePair<ServerToClient, PacketHandler> foundHandler in foundHandlers)
			if (!handlers.ContainsValue(foundHandler.Value))
				handlers.Add(foundHandler.Key, foundHandler.Value);
	}
}