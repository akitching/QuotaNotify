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
}
