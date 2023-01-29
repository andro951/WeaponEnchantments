using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace WeaponEnchantments.Common.Utility
{
	public class TXT : MyFile
	{
		string MyText => File.ReadAllText(ActualPath);
		public TXT(string name, MyFile parent) : base(name, parent) {}

		public bool DifferentText(string text) => text != MyText;
	}
}
