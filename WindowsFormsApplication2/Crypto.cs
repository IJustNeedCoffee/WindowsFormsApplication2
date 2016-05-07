using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.IO;

namespace WindowsFormsApplication2
{
    class Crypto
    {
        public void EncryptFile(string inputFile, string outputFile, string password)
        {
            try
            {
                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] key = UE.GetBytes(password);

                string cryptFile = outputFile;
                FileStream fsCrypt = new FileStream(cryptFile, FileMode.Create);

                RijndaelManaged RMCrypto = new RijndaelManaged();
                RMCrypto.BlockSize = 256;

                CryptoStream cs = new CryptoStream(fsCrypt,
                    RMCrypto.CreateEncryptor(key, key),
                    CryptoStreamMode.Write);

                FileStream fsIn = new FileStream(inputFile, FileMode.Open);

                int data;
                while ((data = fsIn.ReadByte()) != -1)
                    cs.WriteByte((byte)data);


                fsIn.Close();
                cs.Close();
                fsCrypt.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show("Encryption failed!", "Error");
                MessageBox.Show(e.ToString());
            }
        }

        public void DecryptFile(string inputFile, string outputFile, string password)
        {
            {

                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] key = UE.GetBytes(password);

                FileStream fsCrypt = new FileStream(inputFile, FileMode.Open);

                RijndaelManaged RMCrypto = new RijndaelManaged();
                RMCrypto.BlockSize = 256;

                CryptoStream cs = new CryptoStream(fsCrypt,
                    RMCrypto.CreateDecryptor(key, key),
                    CryptoStreamMode.Read);

                FileStream fsOut = new FileStream(outputFile, FileMode.Create);

                int data;
                while ((data = cs.ReadByte()) != -1)
                    fsOut.WriteByte((byte)data);

                fsOut.Close();
                cs.Close();
                fsCrypt.Close();

            }
        }

        public bool IsEncrypted(string filename)
        {
            string ext = Path.GetExtension(filename);

            if (ext == ".crypt" || ext == ".cryptn")
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        public bool IsEncryptedFileName(string filename)
        {
            string ext = Path.GetExtension(filename);

            if (ext == ".cryptn")
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        /* --------------- Title Encryption --------------- */

        public string EncryptTitle(string clearText, string Password)
        {
            //Convert text to bytes
            byte[] clearBytes =
              System.Text.Encoding.Unicode.GetBytes(clearText);

            //We will derieve our Key and Vectore based on following 
            //password and a random salt value, 13 bytes in size.
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password,
                new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d,
            0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76});

            byte[] encryptedData = Encrypt(clearBytes,
                     pdb.GetBytes(32), pdb.GetBytes(16));

            return Convert.ToBase64String(encryptedData);
        }

        public byte[] Encrypt(byte[] clearData, byte[] Key, byte[] IV)
        {
            byte[] encryptedData;
            //Create stream for encryption
            using (MemoryStream ms = new MemoryStream())
            {
                //Create Rijndael object with key and vector
                using (Rijndael alg = Rijndael.Create())
                {
                    alg.Key = Key;
                    alg.IV = IV;
                    //Forming cryptostream to link with data stream.
                    using (CryptoStream cs = new CryptoStream(ms,
                       alg.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        //Write all data to stream.
                        cs.Write(clearData, 0, clearData.Length);
                    }
                    encryptedData = ms.ToArray();
                }
            }

            return encryptedData;
        }

        //Call following function to decrypt data
        public string DecryptTitle(string cipherText, string Password)
        {
            //Convert base 64 text to bytes
            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            //We will derieve our Key and Vectore based on following 
            //password and a random salt value, 13 bytes in size.
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password,
                new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65,
            0x64, 0x76, 0x65, 0x64, 0x65, 0x76});
            byte[] decryptedData = Decrypt(cipherBytes,
                pdb.GetBytes(32), pdb.GetBytes(16));

            //Converting unicode string from decrypted data
            return Encoding.Unicode.GetString(decryptedData);
        }

        public byte[] Decrypt(byte[] cipherData, byte[] Key, byte[] IV)
        {
            byte[] decryptedData;
            //Create stream for decryption
            using (MemoryStream ms = new MemoryStream())
            {
                //Create Rijndael object with key and vector
                using (Rijndael alg = Rijndael.Create())
                {
                    alg.Key = Key;
                    alg.IV = IV;
                    //Forming cryptostream to link with data stream.
                    using (CryptoStream cs = new CryptoStream(ms,
                        alg.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        //Write all data to stream.
                        cs.Write(cipherData, 0, cipherData.Length);
                    }
                    decryptedData = ms.ToArray();
                }
            }
            return decryptedData;
        }
    }
}
