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
	
	public static class ArrayExt
	{
		public static Packet AddChars(this Packet packet, char[] a) {
			packet.AddInt(a.Length);
			foreach (char b in a)
				packet.AddChar(b);

			return packet;
		}
		
		public static char[] GetChars(this Packet packet) {
			int len = packet.GetInt();
			char[] arr = new char[len];
			for (int i = 0; i < len; i++)
				arr[i] = packet.GetChar();
			return arr;
		}
		public static Packet AddShort(this Packet packet, short[] a) {
			packet.AddInt(a.Length);
			foreach (short b in a)
				packet.AddShort(b);

			return packet;
		}
		
		public static short[] GetShorts(this Packet packet) {
			int len = packet.GetInt();
			short[] arr = new short[len];
			for (int i = 0; i < len; i++)
				arr[i] = packet.GetShort();
			return arr;
		}
		public static Packet AddInts(this Packet packet, int[] a) {
			packet.AddInt(a.Length);
			foreach (int b in a)
				packet.AddInt(b);

			return packet;
		}
		
		public static int[] GetInts(this Packet packet) {
			int len = packet.GetInt();
			int[] arr = new int[len];
			for (int i = 0; i < len; i++)
				arr[i] = packet.GetInt();
			return arr;
		}
		public static Packet AddLongs(this Packet packet, long[] a) {
			packet.AddInt(a.Length);
			foreach (long b in a)
				packet.AddLong(b);

			return packet;
		}
		
		public static long[] GetLongs(this Packet packet) {
			int len = packet.GetInt();
			long[] arr = new long[len];
			for (int i = 0; i < len; i++)
				arr[i] = packet.GetLong();
			return arr;
		}
		
		public static Packet AddUShort(this Packet packet, ushort[] a) {
			packet.AddInt(a.Length);
			foreach (ushort b in a)
				packet.AddUShort(b);

			return packet;
		}
		
		public static ushort[] GetUShorts(this Packet packet) {
			int len = packet.GetInt();
			ushort[] arr = new ushort[len];
			for (int i = 0; i < len; i++)
				arr[i] = packet.GetUShort();
			return arr;
		}
		public static Packet AddUInt(this Packet packet, uint[] a) {
			packet.AddInt(a.Length);
			foreach (uint b in a)
				packet.AddUInt(b);

			return packet;
		}
		
		public static uint[] GetUInts(this Packet packet) {
			int len = packet.GetInt();
			uint[] arr = new uint[len];
			for (int i = 0; i < len; i++)
				arr[i] = packet.GetUInt();
			return arr;
		}
		public static Packet AddULongs(this Packet packet, ulong[] a) {
			packet.AddInt(a.Length);
			foreach (ulong b in a)
				packet.AddULong(b);

			return packet;
		}
		
		public static ulong[] GetULongs(this Packet packet) {
			int len = packet.GetInt();
			ulong[] arr = new ulong[len];
			for (int i = 0; i < len; i++)
				arr[i] = packet.GetULong();
			return arr;
		}
		
		public static Packet AddStrings(this Packet packet, string[] a) {
			packet.AddInt(a.Length);
			foreach (string b in a)
				packet.AddString(b);

			return packet;
		}
		
		public static string[] GetStrings(this Packet packet) {
			int len = packet.GetInt();
			string[] strs = new string[len];
			for (int i = 0; i < len; i++)
				strs[i] = packet.GetString();
			return strs;
		}
	}
}