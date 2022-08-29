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
	public class HashSetEmmitter : ModHandlerEmitter<HashSet<short>>
	{
		public override HashSet<short> Read(BinaryReader reader) {
			var count = reader.ReadByte();
			var hashSet = new HashSet<short>();

			for (byte j = 0; j < count; j++) {
				hashSet.Add(reader.ReadInt16());
			}

			return hashSet;
		}

		public override void Write(BinaryWriter writer, HashSet<short> ins) {
			writer.Write((byte)ins.Count);
			foreach (var id in ins) {
				writer.Write(id);
			}
		}
	}
}
