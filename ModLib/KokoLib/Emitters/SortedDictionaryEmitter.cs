using KokoLib.Emitters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponEnchantments.ModLib.KokoLib.Emitters
{
	public class SortedDictionaryEmitter : ModHandlerEmitter<SortedDictionary<int, SortedSet<short>>>
	{
		public override SortedDictionary<int, SortedSet<short>> Read(BinaryReader reader) {
			short dictionaryCount = reader.ReadInt16();
			SortedDictionary<int, SortedSet<short>> dict = new();
			for (int i = 0; i < dictionaryCount; i++) {
				int key = reader.ReadInt32();
				short count = reader.ReadInt16();
				SortedSet<short> set = new();
				for (int c = 0; c < count; c++) {
					short value = reader.ReadInt16();
					set.Add(value);
				}

				dict.Add(key, set);
			}

			return dict;
		}

		public override void Write(BinaryWriter writer, SortedDictionary<int, SortedSet<short>> ins) {
			writer.Write((short)ins.Count);
			foreach (KeyValuePair<int, SortedSet<short>> pair in ins) {
				writer.Write(pair.Key);
				writer.Write((short)pair.Value.Count);
				foreach (short value in pair.Value) {
					writer.Write(value);
				}
			}
		}
	}
}
