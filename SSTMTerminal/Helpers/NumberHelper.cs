using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SSTMTerminal.Helpers
{
	public class NumberHelper
	{
		private static readonly List<char> ValidChars = new List<char> { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};
		public static bool ValidateCellNumber(string input)
		{
			if (string.IsNullOrEmpty(input) || input.Length < 11)
			{
				return false;
			}

			foreach(var c in input)
			{
				if(!ValidChars.Contains(c))
				{
					return false;
				}
			}

			return true;
			//Regex regex = new Regex("^1[345789]\\d{9}$");
			//return regex.IsMatch(input);
		}

		public static bool ValidateIdentityNumber(string input)
		{
			Regex regex = new Regex("^[a-zA-Z0-9]+$");
			return regex.IsMatch(input);
		}
	}
}