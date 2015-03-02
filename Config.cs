/*
 * Copyright (C) 2014-2015 Alex Kitching <alex@kitching.info>
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
using System.Xml;
using System.Text;

namespace QuotaNotify
{
	class Config
	{
		private List<Drive> drives;
		private int initialInterval;
		private int checkInterval;
		private int warnPercent;
		private int warnBelow;
		private string warnMessage;
		private bool obsess;

		public List<Drive> Drives {
			get { return drives; }
			private set { drives = value; }
		}
		public int InitialInterval {
			get { return initialInterval; }
			set { initialInterval = value; }
		}
		public int CheckInterval {
			get { return checkInterval; }
			private set { checkInterval = value; }
		}
		public int WarnPercent {
			get { return warnPercent; }
			private set { warnPercent = value; }
		}
		public int WarnBelow {
			get { return warnBelow; }
			private set { warnBelow = value; }
		}
		public string WarnMessage {
			get { return warnMessage; }
			private set { warnMessage = value; }
		}
		public bool Obsess {
			get { return obsess; }
			private set { obsess = value; }
		}

		public Config()
		{
			Drives = new List<Drive>();
			// Set default values
			InitialInterval = 5000;
			CheckInterval = 300000;
			WarnPercent = 10;
			WarnBelow = 100 * 1024 * 1024;
			WarnMessage = null;
			Obsess = false;
			// Override defaults with cutomized settings
			this.loadFromFile();
			this.loadFromRegistry(Microsoft.Win32.RegistryHive.LocalMachine);
			this.loadFromRegistry(Microsoft.Win32.RegistryHive.CurrentUser);
			if (Drives.Count == 0)
			{
				// No drives configured
				// Default to home drive H
				Drives.Add(new Drive('H'));
			}
		}
	
		private void loadFromFile()
		{
			XmlDocument xml = null;
			try {
				xml = new XmlDocument();
				xml.Load("config.xml");
			} catch (FileNotFoundException ex) {
				// File does not exist
			} catch (Exception ex) {
				// Other exception
				// TODO: Log error
			}
			if (xml != null)
			{
				XmlNode initialIntervalNode = xml.SelectSingleNode("//Config/initialInterval");
				if (initialIntervalNode != null)
					InitialInterval = Convert.ToInt32(initialIntervalNode.InnerText);
				XmlNode checkIntervalNode = xml.SelectSingleNode("//Config/checkInterval");
				if (checkIntervalNode != null)
					CheckInterval = Convert.ToInt32(checkIntervalNode.InnerText);
				XmlNode warnPercentNode = xml.SelectSingleNode("//Config/warnPercent");
				if (warnPercentNode != null)
					WarnPercent = Convert.ToInt32(warnPercentNode.InnerText);
				XmlNode warnBelowNode = xml.SelectSingleNode("//Config/warnBelow");
				if (warnBelowNode != null)
					WarnBelow = Convert.ToInt32(warnBelowNode.InnerText);
				XmlNode warnMessageNode = xml.SelectSingleNode("//Config/warnMessage");
				if (warnMessageNode != null)
					WarnMessage = warnMessageNode.InnerText;
				XmlNode obsessNode = xml.SelectSingleNode("//Config/obsess");
				if (obsessNode != null)
					Obsess = Convert.ToBoolean(obsessNode.InnerText);

				XmlNodeList driveNodes = xml.SelectNodes("//Config/driveList/Drive");
				foreach (XmlNode driveNode in driveNodes)
	            {
					if (driveNode.Name.ToLower() == "drive")
					{
						if (driveNode.Attributes["letter"] != null
						    && driveNode.Attributes["letter"].Value.Length == 1)
						{
							char letter = driveNode.Attributes["letter"].Value.ToCharArray()[0];
							if (Char.IsLetter(letter))
								Drives.Add(new Drive(letter));
						}
					}
	            }
			}
		}
	
		private void loadFromRegistry(Microsoft.Win32.RegistryHive hive)
		{
			string[] driveList = null;
			string keyLocation = "SOFTWARE\\AmalgamStudios\\QuotaNotify";
			RegistryKey registry64 = RegistryKey.OpenBaseKey(hive, RegistryView.Registry64);
			RegistryKey registry32 = RegistryKey.OpenBaseKey(hive, RegistryView.Registry32);
			RegistryKey key = null;
			try {
				key = registry64.OpenSubKey(keyLocation);
				if (key == null)
				{
					key = registry32.OpenSubKey(keyLocation);
				}
	
				if (key != null)
				{
					InitialInterval = readRegistryKey(InitialInterval, "initialInterval", key);
					CheckInterval = readRegistryKey(CheckInterval, "checkInterval", key);
					WarnPercent = readRegistryKey(WarnPercent, "warnPercent", key);
					WarnBelow = readRegistryKey(WarnBelow, "warnBelow", key);
					WarnMessage = readRegistryKey(WarnMessage, "warnMessage", key);
					Obsess = readRegistryKey(Obsess, "obsess", key);
	
					driveList = readRegistryKey(driveList, "Drives", key);
					if (driveList != null && driveList.Length > 0)
					{
						// Drives assigned in registry should override config file.
						// Thus, this.drives must be emptied before adding drives definded in the registry. 
						Drives.Clear();
						foreach (string drive in driveList)
						{
							Drives.Add(new Drive(drive.ToCharArray()[0]));
						}
					}
				}
			} catch (Exception ex) {
				MessageBox.Show( "Error " + ex.ToString() );
			} finally {
				registry32.Close();
				registry64.Close();
				if (key != null) {
					key.Close();
				}
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
			if (String.IsNullOrWhiteSpace((string) val))
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
				// MULTI_SZ registry values _should_ never contain empty strings, however it is possible
				// when using Group Policy Preferences, so we strip them out to be safe
				return Array.FindAll((string[]) val, isNotEmpty);
			}
		}

		private bool isNotEmpty(string s)
		{
			return !String.IsNullOrWhiteSpace(s);
		}
	}
}
