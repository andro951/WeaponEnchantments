using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponEnchantments.Common.Utility
{
	public class MyFile
	{
		private MyFile Parent;
		protected bool IsMaster = false;
		public string Name { get => name;
			set {
				pathName = value;
				int i = value.IndexOf("(") - 1;
				if (i > 0) {
					name = value.Substring(0, i);
					int dot = value.IndexOf(".");
					if (dot > 0)
						name += value.Substring(dot);
				}
				else {
					name = value;
				}

				name = name.GetFileName('\\');
			}
		}
		private string name;
		public string PathName { get => pathName; }
		protected string pathName;
		protected string compairPath = null;
		public string CompairPath {
			get {
				if (compairPath == null) {
					compairPath = Parent != null ? Parent.IsMaster ? $"{Name}" : $"{Parent.CompairPath}\\{Name}" : "";
				}

				return compairPath;
			}
		}
		protected string actualPath = null;
		public string ActualPath {
			get {
				if (actualPath == null) {
					actualPath = Parent != null ? $"{Parent.ActualPath}\\{PathName}" : PathName;
				}

				return actualPath;
			}
		}

		public MyFile(string name, MyFile parent = null) {
			Name = name;
			Parent = parent;
			if (parent == null) {
				if (this is Folder folder)
					folder.SetParent();
			}
		}

		//public virtual void LogInfo() { }
	}
}
