using System;
using System.Text;

namespace MannLabels
{
    public class VoiceCode
    {
        /// <summary>
        /// Compute a 4 digit VoiceCode from specified GTIN, lot, and optional pack date
        /// </summary>
        /// <param name="GTIN">14 digit GTIN number.</param>
        /// <param name="lot">Lot information.</param>
        /// <param name="packDate">Pack Date used with the GTIN and Lot.</packDate>
         /// <returns>4 digit VoiceCode computed from a hash of the input parameters.</returns>
        public static string Compute(string GTIN, string lot, DateTime? packDate)
        {
            ushort crc = CrcMy16.ComputeChecksum(Encoding.ASCII.GetBytes(string.Format("{0}{1}{2}",
                   GTIN, lot, packDate.HasValue ? packDate.Value.ToString("yyMMdd") : string.Empty)));

            return string.Format("{0:0000}", crc % 10000);
        }

        public static class CrcMy16 //funny name because I already had a Crc16 class
        {
            #region static members
            private const ushort polynomial = 0xA001;
            private static ushort[] table = new ushort[256];
            static CrcMy16()
            {
                ushort value;
                ushort temp;
                for (ushort i = 0; i < table.Length; ++i)
                {
                    value = 0;
                    temp = i;
                    for (byte j = 0; j < 8; ++j)
                    {
                        if (0 != ((value ^ temp) & 0x0001))
                        {
                            value = (ushort) ((value >> 1) ^ polynomial);
                        }
                        else
                        {
                            value >>= 1;
                        }
                        temp >>= 1;
                    }
                    table[i] = value;
                }
            }
            #endregion

            public static ushort ComputeChecksum(byte[] bytes)
            {
                ushort crc = 0;
                for (int i = 0; i < bytes.Length; ++i)
                {
                    byte index = (byte) (crc ^ bytes[i]);
                    crc = (ushort) ((crc >> 8) ^ table[index]);
                }
                return crc;
            }
        }
    }
}
