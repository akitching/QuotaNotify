/*
 * Copyright (C) 2014 Alex Kitching <alex@kitching.info>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using Microsoft.Win32;

namespace QuotaNotify
{
	public class DiskChecker : Form
	{
		static System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
	
		private bool warning;
		private string message;
		private Config config;
	
		private DateTime nextCheck;
	
		public DiskChecker()
		{
			this.Hide();
			config = new Config();
	
			nextCheck = DateTime.Now;
			timer.Tick += new EventHandler(checkDriveSpace);
			timer.Interval = config.initialInterval();
			timer.Start();
			this.Hide();
		}
	
		private void checkDriveSpace(Object sender, EventArgs args)
		{
			if (timer.Interval < config.checkInterval())
			{
				timer.Interval = config.checkInterval();
			}
			warning = false;
			message = "";
			// TODO: Refactor foreach to loop over config.drives rather than DriveInfo.GetDrives
			foreach (DriveInfo drive in DriveInfo.GetDrives())
			{
				if (drive.IsReady)
				{
					double percentFree = ((double)drive.TotalFreeSpace / drive.TotalSize) * 100;
					char driveLetter = drive.Name.ToCharArray()[0];
					if ( config.drives().Exists(x => x.letter() == driveLetter) )
					{
						Drive drv = config.drives().Find(x => x.letter() == driveLetter);
						if ((percentFree < config.warnPercent()) && ((percentFree < drv.percentFree() || config.obsess() == true)) && (drive.TotalFreeSpace < config.warnBelow()))
						{
							warning = true;
							message += "Drive " + drive.Name + " has only " + String.Format("{0:F2}", percentFree) + "% free space.\n";
						}
						drv.percentFree(percentFree);
					}
				}
			}
			if (warning)
			{
				message += "Please delete unnecessary files.\n";
				if (config.warnMessage() != null)
					message += config.warnMessage();
				MessageBox.Show (message, "Low disk space");
				warning = false;
				message = "";
			}
		}
	}
}
