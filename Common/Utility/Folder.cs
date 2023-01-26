using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponEnchantments.Common.Utility
{
	public class Folder : MyFile
	{
		DirectoryInfo MyDirectoryInfo;
		public Dictionary<string, MyFile> files = new();
		private List<TXT> txtFiles = null;
		public List<TXT> TXTFiles {
			get {
				if (txtFiles == null) {
					txtFiles = new();
					foreach (TXT txt in files.Values.OfType<TXT>()) {
						txtFiles.Add(txt);
					}

					foreach (Folder folder in files.Values.OfType<Folder>()) {
						List<TXT> folderFiles = folder.TXTFiles;
						txtFiles = txtFiles.Concat(folderFiles).ToList();
					}
				}

				return txtFiles;
			}
		}
		public bool HasFile(string path, out TXT txt) {
			List<TXT> txtFiles = TXTFiles.Where(t => t.CompairPath == path).ToList();
			txt = txtFiles.Count > 0 ? txtFiles[0] : null;

			return txt != null;
		}
		public bool NewFile(string path, string text, out bool textChanged) {
			bool hasFile = HasFile(path, out TXT txt);
			textChanged = hasFile ? txt.DifferentText(text) : false;

			return !hasFile;
		}
		public Folder(string name) : base(name, null) {
			Name = name;
			MyDirectoryInfo = new DirectoryInfo(ActualPath);
			GetFiles();
		}

		private Folder(DirectoryInfo directoryInfo, Folder parent) : base(directoryInfo.Name, parent) {
			MyDirectoryInfo = directoryInfo;
			GetFiles();
		}

		public void SetParent() {
			IsMaster = true;
		}

		private void GetFiles() {
			foreach (DirectoryInfo directoryInfo in MyDirectoryInfo.GetDirectories()) {
				Folder newFolder = new(directoryInfo, this);
				files.Add(newFolder.Name, newFolder);
			}

			foreach (FileInfo fileInfo in MyDirectoryInfo.GetFiles()) {
				TXT newTXT = new(fileInfo, this);
				files.Add(newTXT.Name, newTXT);
			}
		}

		/*public override void LogInfo() {
			foreach(MyFile myFile in files.Values) {
				Type type = myFile.GetType();
				myFile.LogInfo();
			}
		}*/
	}
}
