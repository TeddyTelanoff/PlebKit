using UnityEngine;

namespace PacketExt
{
	public static class VectorExt
	{
		public static Packet AddVector2(this Packet packet, Vector2 a) {
			return packet.AddFloat(a.x).AddFloat(a.y);
		}
		
		public static Packet AddVector3(this Packet packet, Vector3 a) {
			return packet.AddFloat(a.x).AddFloat(a.y).AddFloat(a.z);
		}
		
		public static Packet AddVector4(this Packet packet, Vector4 a) {
			return packet.AddFloat(a.x).AddFloat(a.y).AddFloat(a.z).AddFloat(a.w);
		}
		
		
		public static Vector2 GetVector2(this Packet packet) {
			return new Vector2(packet.GetFloat(), packet.GetFloat());
		}
		
		public static Vector3 GetVector3(this Packet packet) {
			return new Vector3(packet.GetFloat(), packet.GetFloat(), packet.GetFloat());
		}
		
		public static Vector4 GetVector4(this Packet packet) {
			return new Vector4(packet.GetFloat(), packet.GetFloat(), packet.GetFloat(), packet.GetFloat());
		}
	}
}