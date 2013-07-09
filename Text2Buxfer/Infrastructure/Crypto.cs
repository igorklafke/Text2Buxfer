using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Text2Buxfer
{
    public static class Crypto
    {
        public static string Encrypt(string textToEncrypt, string key)
        {
            TripleDESCryptoServiceProvider cryptoService = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider md5Crypto = new MD5CryptoServiceProvider();

            byte[] byteHash, byteBuff;
            string strTempKey = key;

            byteHash = md5Crypto.ComputeHash(ASCIIEncoding.ASCII.GetBytes(strTempKey));
            md5Crypto = null;
            cryptoService.Key = byteHash;
            cryptoService.Mode = CipherMode.ECB;

            byteBuff = ASCIIEncoding.ASCII.GetBytes(textToEncrypt);
            return Convert.ToBase64String(cryptoService.CreateEncryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));

        }

        public static string Decrypt(string textToDecrypt, string key)
        {
            TripleDESCryptoServiceProvider cryptoService = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider md5Crypto = new MD5CryptoServiceProvider();

            byte[] byteHash, byteBuff;
            string strTempKey = key;

            byteHash = md5Crypto.ComputeHash(ASCIIEncoding.ASCII.GetBytes(strTempKey));
            md5Crypto = null;
            cryptoService.Key = byteHash;
            cryptoService.Mode = CipherMode.ECB;

            byteBuff = Convert.FromBase64String(textToDecrypt);
            string strDecrypted = ASCIIEncoding.ASCII.GetString(cryptoService.CreateDecryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
            cryptoService = null;

            return strDecrypted;
        }
    }
}