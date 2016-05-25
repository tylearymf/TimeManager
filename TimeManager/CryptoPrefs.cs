﻿//using System;
//using System.IO;
//using System.Security.Cryptography;
//using System.Text;
//using UnityEngine;

//public class CryptoPrefs
//{
//    static string sKEY = "ZTdkNTNmNDE1NTM2MWM0NDFhNTEzNzU3";
//    static string sIV = "1rZymEMfa/PpeJ23qY4gyA==";

//    public static void SetInt(string key, int val)
//    {
//        PlayerPrefs.SetString(CryptoPrefs.GetHash(key), CryptoPrefs.Encrypt(val.ToString()));
//    }

//    public static int GetInt(string key, int defaultValue = 0)
//    {
//        string @string = CryptoPrefs.GetString(key, defaultValue.ToString());
//        int result = defaultValue;
//        int.TryParse(@string, out result);
//        return result;
//    }

//    public static void SetFloat(string key, float val)
//    {
//        PlayerPrefs.SetString(CryptoPrefs.GetHash(key), CryptoPrefs.Encrypt(val.ToString()));
//    }

//    public static float GetFloat(string key, float defaultValue = 0f)
//    {
//        string @string = CryptoPrefs.GetString(key, defaultValue.ToString());
//        float result = defaultValue;
//        float.TryParse(@string, out result);
//        return result;
//    }

//    public static void SetString(string key, string val)
//    {
//        PlayerPrefs.SetString(CryptoPrefs.GetHash(key), CryptoPrefs.Encrypt(val));
//    }

//    public static string GetString(string key, string defaultValue = "")
//    {
//        string text = defaultValue;
//        string @string = PlayerPrefs.GetString(CryptoPrefs.GetHash(key), defaultValue.ToString());
//        if (!text.Equals(@string))
//        {
//            text = CryptoPrefs.Decrypt(@string);
//        }
//        return text;
//    }

//    public static bool HasKey(string key)
//    {
//        string hash = CryptoPrefs.GetHash(key);
//        return PlayerPrefs.HasKey(hash);
//    }

//    public static void DeleteKey(string key)
//    {
//        string hash = CryptoPrefs.GetHash(key);
//        PlayerPrefs.DeleteKey(hash);
//    }

//    public static void DeleteAll()
//    {
//        PlayerPrefs.DeleteAll();
//    }

//    public static void Save()
//    {
//        PlayerPrefs.Save();
//    }

//    static string Decrypt(string encString)
//    {
//        RijndaelManaged rijndaelManaged = new RijndaelManaged
//        {
//            Padding = PaddingMode.Zeros,
//            Mode = CipherMode.CBC,
//            KeySize = 128,
//            BlockSize = 128
//        };
//        byte[] bytes = Encoding.UTF8.GetBytes(CryptoPrefs.sKEY);
//        byte[] rgbIV = Convert.FromBase64String(CryptoPrefs.sIV);
//        ICryptoTransform transform = rijndaelManaged.CreateDecryptor(bytes, rgbIV);
//        byte[] array = Convert.FromBase64String(encString);
//        byte[] array2 = new byte[array.Length];
//        MemoryStream stream = new MemoryStream(array);
//        CryptoStream cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Read);
//        cryptoStream.Read(array2, 0, array2.Length);
//        return Encoding.UTF8.GetString(array2).TrimEnd(new char[1]);
//    }

//    static string Encrypt(string rawString)
//    {
//        RijndaelManaged rijndaelManaged = new RijndaelManaged
//        {
//            Padding = PaddingMode.Zeros,
//            Mode = CipherMode.CBC,
//            KeySize = 128,
//            BlockSize = 128
//        };
//        byte[] bytes = Encoding.UTF8.GetBytes(CryptoPrefs.sKEY);
//        byte[] rgbIV = Convert.FromBase64String(CryptoPrefs.sIV);
//        ICryptoTransform transform = rijndaelManaged.CreateEncryptor(bytes, rgbIV);
//        MemoryStream memoryStream = new MemoryStream();
//        CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
//        byte[] bytes2 = Encoding.UTF8.GetBytes(rawString);
//        cryptoStream.Write(bytes2, 0, bytes2.Length);
//        cryptoStream.FlushFinalBlock();
//        byte[] inArray = memoryStream.ToArray();
//        return Convert.ToBase64String(inArray);
//    }

//    static string GetHash(string key)
//    {
//        MD5 mD = new MD5CryptoServiceProvider();
//        byte[] array = mD.ComputeHash(Encoding.UTF8.GetBytes(key));
//        StringBuilder stringBuilder = new StringBuilder();
//        for (int i = 0; i < array.Length; i++)
//        {
//            stringBuilder.Append(array[i].ToString("x2"));
//        }
//        return stringBuilder.ToString();
//    }
//}