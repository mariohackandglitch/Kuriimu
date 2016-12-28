﻿using ext_fenceposts.Properties;
using KuriimuContract;
using System.Drawing;
using System.Windows.Forms;

namespace ext_fenceposts
{
	public class Fenceposts : IExtension
	{
		#region Properties

		public string Name
		{
			get { return Settings.Default.PluginName; }
		}

		public Image Icon
		{
			get { return Resources.icon; }
		}

		#endregion

		public void Show()
		{
			frmExtension configure = new frmExtension();
			configure.StartPosition = FormStartPosition.CenterParent;
			configure.Show();
		}

		public override string ToString()
		{
			return Name;
		}
	}
}