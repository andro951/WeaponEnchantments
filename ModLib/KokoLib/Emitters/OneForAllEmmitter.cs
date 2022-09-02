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
	public class OneForAllEmmitter : ModHandlerEmitter<Dictionary<NPC, (int, bool)>>
	{
		public override Dictionary<NPC, (int, bool)> Read(BinaryReader reader) {
			var pairs = reader.ReadByte();
			var dict = new Dictionary<NPC, (int, bool)>();

			for (byte j = 0; j < pairs; j++) {
				dict.Add(Main.npc[reader.ReadByte()], (reader.ReadInt32(), reader.ReadBoolean()));
			}

			return dict;
		}

		public override void Write(BinaryWriter writer, Dictionary<NPC, (int, bool)> ins) {
			writer.Write((byte)ins.Count);
			foreach (var pair in ins) {
				writer.Write((byte)pair.Key.whoAmI);
				writer.Write(pair.Value.Item1);
				writer.Write(pair.Value.Item2);
			}
		}
	}
}
