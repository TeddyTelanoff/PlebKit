using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

public class Packet
{
	public List<byte> buffer = new List<byte>();
	public byte[] readBuffer;
	public int readPos;
	public int unreadLen => readBuffer.Length - readPos;
	
	public bool finished;

	public Packet() {}

	public static Packet Create(ushort id) {
		Packet packet = new Packet();
		packet.AddUShort(id);
		return packet;
	}
	
	// [!]: enum will be casted to ushort
	public static Packet Create(Enum id) {
		return Create((ushort)(object) id);
	}

	public Packet(byte[] readBuffer) {
		this.readBuffer = readBuffer;
	}

	public void Reset() {
		buffer.Clear();
		readPos = 0;
		readBuffer = null;
	}

	public void Finish() {
		if (finished)
			throw new Exception("packet already finished");
		
		// write packet length
		buffer.InsertRange(0, BitConverter.GetBytes(buffer.Count));
		finished = true;

		readPos = 0;
		ReadReady();
	}

	// or shall it be called ReadyRead
	public void ReadReady() {
		readBuffer = buffer.ToArray();
	}

	public Packet AddByte(byte a) {
		buffer.Add(a);
		return this;
	}

	public Packet AddChar(char a) {
		AddBytes(BitConverter.GetBytes(a));
		return this;
	}

	public Packet AddBytes(byte[] a) {
		buffer.AddRange(a);
		return this;
	}

	public Packet AddShort(short a) {
		AddBytes(BitConverter.GetBytes(a));
		return this;
	}

	public Packet AddInt(int a) {
		AddBytes(BitConverter.GetBytes(a));
		return this;
	}

	public Packet AddLong(long a) {
		AddBytes(BitConverter.GetBytes(a));
		return this;
	}

	public Packet AddUShort(ushort a) {
		AddBytes(BitConverter.GetBytes(a));
		return this;
	}

	public Packet AddUInt(uint a) {
		AddBytes(BitConverter.GetBytes(a));
		return this;
	}

	public Packet AddULong(ulong a) {
		AddBytes(BitConverter.GetBytes(a));
		return this;
	}

	public Packet AddFloat(float a) {
		AddBytes(BitConverter.GetBytes(a));
		return this;
	}

	public Packet AddDouble(double a) {
		AddBytes(BitConverter.GetBytes(a));
		return this;
	}

	public Packet AddBool(bool a) {
		AddBytes(BitConverter.GetBytes(a));
		return this;
	}

	public Packet AddString(string a) {
		AddInt(a.Length);
		AddBytes(Encoding.UTF8.GetBytes(a));

		return this;
	}

	
	public char GetChar() {
		char a = BitConverter.ToChar(readBuffer, readPos);
		readPos += sizeof(char);
		return a;
	}
	
	public byte GetByte() {
		return readBuffer[readPos++];
	}
	
	public byte[] GetBytes(int count) {
		byte[] a = new byte[count];
		Array.Copy(readBuffer, readPos, a, 0, count);
		readPos += count;

		return a;
	}
	
	public short GetShort() {
		short a = BitConverter.ToInt16(readBuffer, readPos);
		readPos += sizeof(short);
		return a;
	}
	
	public int GetInt() {
		int a = BitConverter.ToInt32(readBuffer, readPos);
		readPos += sizeof(int);
		return a;
	}
	
	public long GetLong() {
		long a = BitConverter.ToInt64(readBuffer, readPos);
		readPos += sizeof(long);
		return a;
	}
	
	public ushort GetUShort() {
		ushort a = BitConverter.ToUInt16(readBuffer, readPos);
		readPos += sizeof(ushort);
		return a;
	}
	
	public uint GetUInt() {
		uint a = BitConverter.ToUInt32(readBuffer, readPos);
		readPos += sizeof(uint);
		return a;
	}
	
	public ulong GetULong() {
		ulong a = BitConverter.ToUInt64(readBuffer, readPos);
		readPos += sizeof(ulong);
		return a;
	}
	
	public float GetFloat() {
		float a = BitConverter.ToSingle(readBuffer, readPos);
		readPos += sizeof(float);
		return a;
	}
	
	public double GetDouble() {
		double a = BitConverter.ToDouble(readBuffer, readPos);
		readPos += sizeof(double);
		return a;
	}
	
	public bool GetBool() {
		bool a = BitConverter.ToBoolean(readBuffer, readPos);
		readPos += sizeof(bool);
		return a;
	}

	public string GetString() {
		int len = GetInt();
		string str = Encoding.UTF8.GetString(readBuffer, readPos, len);
		readPos += len;
		return str;
	}
}