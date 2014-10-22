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
			aboutMessage = "Quota Notify 1.1.0. Copyright \u00a9 2014 Alex Kitching.\n";
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
}
