
using System;
using System.IO;
using System.Runtime.InteropServices;
using YourNamespace; // Replace with your actual namespace

public partial class Chromiums
{
}

public static class Crypto
{
    [DllImport("crypt32.dll", SetLastError = true)]
    private static extern bool CryptUnprotectData(ref DATA_BLOB pDataIn, IntPtr ppszDataDescr, ref DATA_BLOB pOptionalEntropy, IntPtr pvReserved, ref CRYPTPROTECT_PROMPTSTRUCT pPromptStruct, CryptProtectFlags dwFlags, ref DATA_BLOB pDataOut);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct DATA_BLOB
    {
        public int cbData;
        public IntPtr pbData;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct CRYPTPROTECT_PROMPTSTRUCT
    {
        public int cbSize;
        public CryptProtectPromptFlags dwPromptFlags;
        public IntPtr hwndApp;
        public string szPrompt;
    }

    [Flags]
    internal enum CryptProtectPromptFlags
    {
        None = 0x0,
        PromptOnUnprotect = 0x1,
        PromptOnProtect = 0x2
    }

    [Flags]
    internal enum CryptProtectFlags
    {
        None = 0x0,
        UIForbidden = 0x1,
        LocalMachine = 0x4,
        CredSync = 0x8,
        Audit = 0x10,
        ProtectMemory = 0x20,
        VerifyProtection = 0x40,
        VerifyPromptStruct = 0x80,
        NoRecovery = 0x100,
        ProtectAuditing = 0x200,
        NoUI = 0x400,
    }

    public static byte[] DecryptDPAPI(byte[] encryptedData)
    {
        DATA_BLOB dataIn = new();
        DATA_BLOB dataOut = new();
        DATA_BLOB optionalEntropy = new();
        CRYPTPROTECT_PROMPTSTRUCT promptStruct = new();

        try
        {
            dataIn.pbData = Marshal.AllocHGlobal(encryptedData.Length);
            dataIn.cbData = encryptedData.Length;
            Marshal.Copy(encryptedData, 0, dataIn.pbData, encryptedData.Length);

            if (CryptUnprotectData(ref dataIn, IntPtr.Zero, ref optionalEntropy, IntPtr.Zero, ref promptStruct, CryptProtectFlags.VerifyProtection, ref dataOut))
            {
                byte[] decryptedData = new byte[dataOut.cbData];
                Marshal.Copy(dataOut.pbData, decryptedData, 0, decryptedData.Length);
                return decryptedData;
            }

            throw new Exception("CryptUnprotectData failed.");
        }
        finally
        {
            if (dataIn.pbData != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(dataIn.pbData);
            }

            if (dataOut.pbData != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(dataOut.pbData);
            }
        }
    }
}
