using System;
using System.Text;
using Microsoft.VisualBasic;
using System.Security.Cryptography;
using System.IO;

namespace Aisino.MES.Client.Common
{
    public class XXTEA
    {
        public static Byte[] Encrypt(Byte[] Data, Byte[] Key)
        {
            if (Data.Length == 0)
            {
                return Data;
            }
            return ToByteArray(Encrypt(ToUInt32Array(Data, true), ToUInt32Array(Key, false)), false);
        }
        public static Byte[] Decrypt(Byte[] Data, Byte[] Key)
        {
            if (Data.Length == 0)
            {
                return Data;
            }
            return ToByteArray(Decrypt(ToUInt32Array(Data, false), ToUInt32Array(Key, false)), true);
        }

        public static UInt32[] Encrypt(UInt32[] v, UInt32[] k)
        {
            Int32 n = v.Length - 1;
            if (n < 1)
            {
                return v;
            }
            if (k.Length < 4)
            {
                UInt32[] Key = new UInt32[4];
                k.CopyTo(Key, 0);
                k = Key;
            }
            UInt32 z = v[n], y = v[0], delta = 0x9E3779B9, sum = 0, e;
            Int32 p, q = 6 + 52 / (n + 1);
            while (q-- > 0)
            {
                sum = unchecked(sum + delta);
                e = sum >> 2 & 3;
                for (p = 0; p < n; p++)
                {
                    y = v[p + 1];
                    z = unchecked(v[p] += (z >> 5 ^ y << 2) + (y >> 3 ^ z << 4) ^ (sum ^ y) + (k[p & 3 ^ e] ^ z));
                }
                y = v[0];
                z = unchecked(v[n] += (z >> 5 ^ y << 2) + (y >> 3 ^ z << 4) ^ (sum ^ y) + (k[p & 3 ^ e] ^ z));
            }
            return v;
        }
        public static UInt32[] Decrypt(UInt32[] v, UInt32[] k)
        {
            Int32 n = v.Length - 1;
            if (n < 1)
            {
                return v;
            }
            if (k.Length < 4)
            {
                UInt32[] Key = new UInt32[4];
                k.CopyTo(Key, 0);
                k = Key;
            }
            UInt32 z = v[n], y = v[0], delta = 0x9E3779B9, sum, e;
            Int32 p, q = 6 + 52 / (n + 1);
            sum = unchecked((UInt32)(q * delta));
            while (sum != 0)
            {
                e = sum >> 2 & 3;
                for (p = n; p > 0; p--)
                {
                    z = v[p - 1];
                    y = unchecked(v[p] -= (z >> 5 ^ y << 2) + (y >> 3 ^ z << 4) ^ (sum ^ y) + (k[p & 3 ^ e] ^ z));
                }
                z = v[n];
                y = unchecked(v[0] -= (z >> 5 ^ y << 2) + (y >> 3 ^ z << 4) ^ (sum ^ y) + (k[p & 3 ^ e] ^ z));
                sum = unchecked(sum - delta);
            }
            return v;
        }
        private static UInt32[] ToUInt32Array(Byte[] Data, Boolean IncludeLength)
        {
            Int32 n = (((Data.Length & 3) == 0) ? (Data.Length >> 2) : ((Data.Length >> 2) + 1));
            UInt32[] Result;
            if (IncludeLength)
            {
                Result = new UInt32[n + 1];
                Result[n] = (UInt32)Data.Length;
            }
            else
            {
                Result = new UInt32[n];
            }
            n = Data.Length;
            for (Int32 i = 0; i < n; i++)
            {
                Result[i >> 2] |= (UInt32)Data[i] << ((i & 3) << 3);
            }
            return Result;
        }
        private static Byte[] ToByteArray(UInt32[] Data, Boolean IncludeLength)
        {
            Int32 n;
            if (IncludeLength)
            {
                n = (Int32)Data[Data.Length - 1];
            }
            else
            {
                n = Data.Length << 2;
            }
            Byte[] Result = new Byte[n];
            for (Int32 i = 0; i < n; i++)
            {
                Result[i] = (Byte)(Data[i >> 2] >> ((i & 3) << 3));
            }
            return Result;
        }

        private const string KEY = "1234509876";

        public static String Encrypt(String sourceString)
        {
            Char[] sourceArray = sourceString.ToCharArray();
            System.Text.StringBuilder temp = new System.Text.StringBuilder();
            for (Int32 i = 0; i < sourceArray.Length; i++)
            {
                sourceArray[i] = (Char)(Convert.ToInt16(sourceArray[i]) + Convert.ToInt16(KEY[i % KEY.Length] / 2));
                temp.Append(sourceArray[i].ToString());
            }
            return temp.ToString();
        }

        public static String Dencrypt(String ciphertext)
        {
            Char[] ciphertextArray = ciphertext.ToCharArray();
            System.Text.StringBuilder temp = new System.Text.StringBuilder();
            for (Int32 i = 0; i < ciphertextArray.Length; i++)
            {
                ciphertextArray[i] = (Char)(Convert.ToInt16(ciphertextArray[i]) - Convert.ToInt16(KEY[i % KEY.Length] / 2));
                temp.Append(ciphertextArray[i].ToString());
            }
            return temp.ToString();
        }

        #region   字符串的简单加密、解密
        ///   <summary>   
        ///   对字符串的加密   
        ///   </summary>   
        ///   <param   name="str">输入字符串</param>   
        ///   <returns>加密后的字符串</returns>   
        public static string DesString(string str)
        {
            string strReturn = "";
            if (str == "")
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < str.Length; i++)
            {
                sb.Append((char)(str[i] + 4));
            }

            strReturn = sb.ToString();

            return strReturn;
        }

        ///   <summary>   
        ///   对字符串的解密   
        ///   </summary>   
        ///   <param   name="str">输入字符串</param>   
        ///   <returns>解密后的字符串</returns>   
        public static string EntryString(string str)
        {
            string strReturn = "";

            if (str == "")
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < str.Length; i++)
            {
                sb.Append((char)(str[i] - 4));
            }

            strReturn = sb.ToString();

            return strReturn;
        }
        #endregion
    }
}