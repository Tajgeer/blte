using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace blte
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length < 1)
			{
				Console.WriteLine("Usage: <input file>");
				return;
			}

			if (!File.Exists (args [0])) {
				Console.WriteLine ("Can't open file {0}", args [0]);
			}

			var file = File.OpenRead (args[0]);
			var br = new BinaryReader (file, Encoding.ASCII);


			while (br.BaseStream.Position != br.BaseStream.Length) {

				long start = br.BaseStream.Position;

				byte[] unkHash = br.ReadBytes(16);
				int size = br.ReadInt32();
				int unk1 = br.ReadInt32 ();
				int unk2 = br.ReadInt16 ();
				int unk3 = br.ReadInt32 ();

				BLTEHandler h = new BLTEHandler(br);
				h.ExtractData("out", unkHash.ToHexString(), size);
			}
		}
	}

	static class Extensions
	{
		public static int ReadInt32BE(this BinaryReader reader)
		{
			return BitConverter.ToInt32(reader.ReadBytes(4).Reverse().ToArray(), 0);
		}

		public static short ReadInt16BE(this BinaryReader reader)
		{
			return BitConverter.ToInt16(reader.ReadBytes(2).Reverse().ToArray(), 0);
		}

		public static string ToHexString(this byte[] data)
		{
			var str = String.Empty;
			for (var i = 0; i < data.Length; ++i)
				str += data[i].ToString("X2", CultureInfo.InvariantCulture);
			return str;
		}

		public static bool VerifyHash(this byte[] hash, byte[] other)
		{
			for (var i = 0; i < hash.Length; ++i)
			{
				if (hash[i] != other[i])
					return false;
			}
			return true;
		}
	}

	public static class CStringExtensions
	{
		/// <summary> Reads the NULL terminated string from 
		/// the current stream and advances the current position of the stream by string length + 1.
		/// <seealso cref="BinaryReader.ReadString"/>
		/// </summary>
		public static string ReadCString(this BinaryReader reader)
		{
			return reader.ReadCString(Encoding.UTF8);
		}

		/// <summary> Reads the NULL terminated string from 
		/// the current stream and advances the current position of the stream by string length + 1.
		/// <seealso cref="BinaryReader.ReadString"/>
		/// </summary>
		public static string ReadCString(this BinaryReader reader, Encoding encoding)
		{
			try
			{
				var bytes = new List<byte>();
				byte b;
				while ((b = reader.ReadByte()) != 0)
					bytes.Add(b);
				return encoding.GetString(bytes.ToArray());
			}
			catch (EndOfStreamException)
			{
				return String.Empty;
			}
		}

		public static void WriteCString(this BinaryWriter writer, string str)
		{
			var bytes = Encoding.UTF8.GetBytes(str);
			writer.Write(bytes);
			writer.Write((byte)0);
		}
	}
}
