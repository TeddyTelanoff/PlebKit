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
	
	public static Dictionary<ServerToClient, PacketHandler> handlers;

	// when will my reflection show....
	public static void MakeDictionary() {
		MethodInfo[] methods = Assembly.GetCallingAssembly()
						   .GetTypes()
						   .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
						   .Where(m => m.GetCustomAttributes(typeof(PacketHandlerAttribute), false).Length > 0)
						   .ToArray();

		handlers = new Dictionary<ServerToClient, PacketHandler>();
		
		foreach (MethodInfo method in methods)
		{
			PacketHandlerAttribute attribute = method.GetCustomAttribute<PacketHandlerAttribute>();

			if (!method.IsStatic)
				throw new Exception($"packet handler {method.DeclaringType}.{method.Name} is not static");
			
			Delegate handler = Delegate.CreateDelegate(typeof(PacketHandler), method, false);

			if (handler != null)
			{
				if (handlers.TryGetValue(attribute.id, out PacketHandler twin))
				{
					MethodInfo twinMethod = twin.GetMethodInfo();

					throw new Exception(
						$"to method-handlers with same packet id: {method.DeclaringType}.{method.Name} & {twinMethod.DeclaringType}.{twinMethod.Name}"
					);
				}
				
				handlers.Add(attribute.id, (PacketHandler) handler);
			}
			else
				throw new Exception($"{method.DeclaringType}.{method.Name} has incorrect signature");
		}
	}
}