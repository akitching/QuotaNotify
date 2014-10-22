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
}
