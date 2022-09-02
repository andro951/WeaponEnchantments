using KokoLib.Emitters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace WeaponEnchantments.ModLib.KokoLib.KokoLibSourceFiles.Emitters
{
	public class DebuffsEmitter : ModHandlerEmitter<Dictionary<short, int>>
	{
		public override Dictionary<short, int> Read(BinaryReader reader) {
			var pairs = reader.ReadByte();
			var dict = new Dictionary<short, int>();

			for (byte j = 0; j < pairs; j++) {
				dict.Add(reader.ReadInt16(), reader.ReadInt32());
			}

			return dict;
		}

		public override void Write(BinaryWriter writer, Dictionary<short, int> ins) {
			writer.Write((byte)ins.Count);
			foreach (var pair in ins) {
				writer.Write(pair.Key);
				writer.Write(pair.Value);
			}
		}
	}
}
