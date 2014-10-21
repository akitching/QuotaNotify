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

	public sealed class NotificationIcon
	{
		public NotifyIcon notifyIcon;
		private ContextMenu notificationMenu;

		private DateTime nextCheck;
		
		#region Initialize icon and menu
		public NotificationIcon()
		{
			notifyIcon = new NotifyIcon();
			notificationMenu = new ContextMenu(InitializeMenu());
			
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NotificationIcon));
			notifyIcon.Icon = new Icon(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("QuotaNotify.systray-icon.ico"));
			notifyIcon.ContextMenu = notificationMenu;

			nextCheck = DateTime.Now;
		}

		private MenuItem[] InitializeMenu()
		{
			MenuItem[] menu = new MenuItem[] {
				new MenuItem("About", menuAboutClick)
			};
			return menu;
		}
		#endregion
		
		#region Main - Program entry point
		/// <summary>Program entry point.</summary>
		/// <param name="args">Command Line Arguments</param>
		[STAThread]
		public static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			
			bool isFirstInstance;
			// Please use a unique name for the mutex to prevent conflicts with other programs
			using (Mutex mtx = new Mutex(true, "QuotaNotify", out isFirstInstance)) {
				if (isFirstInstance) {
					NotificationIcon notificationIcon = new NotificationIcon();
					notificationIcon.notifyIcon.Visible = true;
					DiskChecker d = new DiskChecker();
					d.FormBorderStyle = FormBorderStyle.FixedToolWindow;
					d.ShowInTaskbar = false;
					d.StartPosition = FormStartPosition.Manual;
					d.Location = new System.Drawing.Point(-2000, -2000);
					d.Size = new System.Drawing.Size(1, 1);
					Application.Run(d);
					notificationIcon.notifyIcon.Dispose();
				} else {
					// The application is already running
					// TODO: Display message box or change focus to existing application instance
				}
				
			} // releases the Mutex
		}
		#endregion

		#region Event Handlers
		private void menuAboutClick(object sender, EventArgs e)
		{
			string aboutMessage;
			aboutMessage = "Quota Notify. Copyright \u00a9 2014 Alex Kitching.\n";
			aboutMessage += "\n";
			aboutMessage += "This program comes with ABSOLUTELY NO WARRANTY.\n";
    		aboutMessage += "This is free software, and you are welcome to redistribute it under the terms of the GNU GPL v3.\n";
    		aboutMessage += "See http://www.gnu.org/licenses/ for details.\n";
    		aboutMessage += "\n";
    		aboutMessage += "Source code is available at https://github.com/akitching/QuotaNotify\n";
			MessageBox.Show(aboutMessage, "About Quota Notify");
		}
		#endregion
	}
	
	class Drive : IEquatable<Drive>
	{
		private char letterValue;
		private double percentFreeValue;
		
		public Drive(char letter)
		{
			letterValue = letter;
			percentFreeValue = 100.0f;
		}
		
		#region Equals and GetHashCode implementation
		public override bool Equals(object obj)
		{
			Drive other = obj as Drive;
			if (other == null)
				return false;
			return this.letterValue == other.letterValue;
		}

		public bool Equals(Drive other)
		{
			if (other == null)
				return false;
			return (this.letterValue.Equals(other.letterValue));
		}
		
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				hashCode += 1000000007 * letterValue.GetHashCode();
			}
			return hashCode;
		}

		public static bool operator ==(Drive lhs, Drive rhs)
		{
			if (ReferenceEquals(lhs, rhs))
				return true;
			if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
				return false;
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Drive lhs, Drive rhs)
		{
			return !(lhs == rhs);
		}
		#endregion

		public char letter()
		{
			return letterValue;
		}

		public double percentFree()
		{
			return percentFreeValue;
		}

		public void percentFree(double percentFreeValue)
		{
			this.percentFreeValue = percentFreeValue;
		}
	}

	class Config
	{
		private List<Drive> driveList;
		private int initialIntervalValue;
		private int checkIntervalValue;
		private int warnPercentValue;
		private int warnBelowValue;
		private string warnMessageValue;
		private bool obsessValue;

		public Config()
		{
			driveList = new List<Drive>();
			// Set default values
			initialIntervalValue = 5000;
			checkIntervalValue = 300000;
			warnPercentValue = 10;
			warnBelowValue = 100 * 1024 * 1024;
			warnMessageValue = null;
			obsessValue = false;
			// Override defaults with cutomized settings
			this.loadFromFile();
			this.loadFromRegistry();
			if (driveList.Count == 0)
			{
				// No drives configured
				// Default to home drive H
				driveList.Add(new Drive('H'));
			}
		}

		private void loadFromFile()
		{
			// TODO: Load config data from file in install directory
		}

		private void loadFromRegistry()
		{
			string[] driveList = null;
			string keyLocation = "SOFTWARE\\Amalgam";
			RegistryKey registry64 = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry64);
			RegistryKey registry32 = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry32);
			RegistryKey key = null;
			try {
				key = registry64.OpenSubKey(keyLocation);
				if (key == null)
				{
					key = registry32.OpenSubKey(keyLocation);
				}

				if (key != null)
				{
					initialIntervalValue = readRegistryKey(initialIntervalValue, "initialInterval", key);
					checkIntervalValue = readRegistryKey(checkIntervalValue, "checkInterval", key);
					warnPercentValue = readRegistryKey(warnPercentValue, "warnPercent", key);
					warnBelowValue = readRegistryKey(warnBelowValue, "warnBelow", key);
					warnMessageValue = readRegistryKey(warnMessageValue, "warnMessage", key);
					obsessValue = readRegistryKey(obsessValue, "obsess", key);

					driveList = readRegistryKey(driveList, "Drives", key);
					foreach (string drive in driveList)
					{
						this.driveList.Add(new Drive(drive.ToCharArray()[0]));
					}
					foreach (Drive drive in this.driveList) {
						drive.percentFree(100.0f);
					}
				}
			} catch (Exception ex) {
				MessageBox.Show( "Error " + ex.ToString() );
			} finally {
				registry32.Close();
				registry64.Close();
				key.Close();
			}
		}

		private bool readRegistryKey(bool variable, string name, RegistryKey key)
		{
			object val = key.GetValue(name);
			if (val == null)
			{
				return variable;
			}
			else
			{
				if ((int) val > 0)
					return true;
				else
					return false;
			}
		}

		private int readRegistryKey(int variable, string name, RegistryKey key)
		{
			object val = key.GetValue(name);
			if (val == null)
			{
				return variable;
			}
			else
			{
				return (int) val;
			}
		}

		private string readRegistryKey(string variable, string name, RegistryKey key)
		{
			object val = key.GetValue(name);
			if (val == null)
			{
				return variable;
			}
			else
			{
				return (string) val;
			}
		}

		private string[] readRegistryKey(string[] variable, string name, RegistryKey key)
		{
			object val = key.GetValue(name);
			if (val == null)
			{
				return variable;
			}
			else
			{
				return (string[]) val;
			}
		}

		public List<Drive> drives()
		{
			return driveList;
		}

		public int initialInterval()
		{
			return initialIntervalValue;
		}

		public int checkInterval()
		{
			return checkIntervalValue;
		}

		public int warnPercent()
		{
			return warnPercentValue;
		}

		public int warnBelow()
		{
			return warnBelowValue;
		}

		public string warnMessage()
		{
			return warnMessageValue;
		}

		public bool obsess()
		{
			return obsessValue;
		}
	}

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
