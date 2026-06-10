using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTMTerminal
{
    public static class CardInfoExtension
    {
        private const int CountryCodeLength = 3;

        /// <summary>
        /// Get the correct ID for all versions of the Foreign ID card.
        /// The old version Foreign ID card ID is like: "PAK310080010103PAK" which the last PAK is not necessary.
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public static string GetCompatibleID(this CVR_Reader.CardInfo card)
        {
            if(card == null || string.IsNullOrEmpty(card.ID))
            {
                return string.Empty;
            }

            var first3Chars = card.ID.Substring(0, CountryCodeLength);
            bool isNumeric = first3Chars.All(char.IsDigit);
            if(isNumeric || card.ID.Length <= CountryCodeLength)
            {
                return card.ID;
            }
            else
            {
                var id = card.ID;
                var newId = id.Substring(0, id.Length - CountryCodeLength);
                return newId;
            }
        }
    }
}
    