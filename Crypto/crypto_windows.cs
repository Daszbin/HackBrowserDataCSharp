using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace YourNamespace
{
    public static class CryptoExtensions
    {
        public static byte[] DecryptPass(byte[] key, byte[] encryptPass)
        {
            return encryptPass.Length < 15
                ? throw new Exception("Password is empty")
                : AesGCMDecrypt(encryptPass[15..], key, encryptPass[3..15]);
        }

        public static byte[] DecryptPassForYandex(byte[] key, byte[] encryptPass)
        {
            if (encryptPass.Length < 3)
            {
                throw new Exception("Password is empty");
            }

            // Remove Prefix 'v10'
            return AesGCMDecrypt(encryptPass[12..], key, encryptPass[0..12]);
        }

        public static byte[] DPAPI(byte[] data)
        {
            DATA_BLOB inputData = new()
            {
                cbData = (uint)data.Length,
                pbData = Marshal.AllocHGlobal(data.Length)
            };
            Marshal.Copy(data, 0, inputData.pbData, data.Length);

            DATA_BLOB outputData = new();

            bool success = CryptUnprotectData(ref inputData, null, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 0, ref outputData);

            if (!success)
            {
                throw new Exception("CryptUnprotectData failed");
            }

            byte[] result = new byte[(int)outputData.cbData];
            Marshal.Copy(outputData.pbData, result, 0, (int)outputData.cbData);

            Marshal.FreeHGlobal(inputData.pbData);
            Marshal.FreeHGlobal(outputData.pbData);

            return result;
        }

        [DllImport("Crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CryptUnprotectData(
            ref DATA_BLOB pDataIn,
            StringBuilder szDataDescr,
            IntPtr pOptionalEntropy,
            IntPtr pvReserved,
            IntPtr pPromptStruct,
            uint dwFlags,
            ref DATA_BLOB pDataOut);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct DATA_BLOB
        {
            public uint cbData;
            public IntPtr pbData;
        }

        public static byte[] AesGCMDecrypt(byte[] crypted, byte[] key, byte[] nonce)
        {
            IBufferedCipher cipher = CipherUtilities.GetCipher("AES/GCM/NoPadding");
            KeyParameter keyParam = ParameterUtilities.CreateKeyParameter("AES", key);
            ParametersWithIV parameters = new(keyParam, nonce);

            cipher.Init(false, parameters);

            byte[] decryptedBytes = new byte[cipher.GetOutputSize(crypted.Length)];
            int len = cipher.ProcessBytes(crypted, 0, crypted.Length, decryptedBytes, 0);
            _ = cipher.DoFinal(decryptedBytes, len);

            return decryptedBytes;
        }

        public static byte[] PKCS5UnPadding(byte[] src, int blockSize)
        {
            int paddingNum = src[^1];
            if (paddingNum < 1 || paddingNum > blockSize)
            {
                throw new Exception("Invalid PKCS#5 padding");
            }

            byte[] unpadded = new byte[src.Length - paddingNum];
            Array.Copy(src, unpadded, unpadded.Length);

            return unpadded;
        }

        public static byte[] PaddingZero(byte[] s, int l)
        {
            byte[] padded = new byte[l];
            Array.Copy(s, padded, s.Length);

            return padded;
        }
    }
}
