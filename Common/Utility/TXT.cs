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
		FileInfo MyFileInfo;
		string MyText => File.ReadAllText(ActualPath);
		public TXT(FileInfo fileInfo, MyFile parent) : base(fileInfo.Name, parent) {
			MyFileInfo = fileInfo;
		}

		public bool DifferentText(string text) => text != MyText;

		/*public override void LogInfo() {
			string temp = $"{CompairPath}, {ActualPath}";
			$"\n{CompairPath}\n{ActualPath}".LogSimple();
		}*/
	}
}
