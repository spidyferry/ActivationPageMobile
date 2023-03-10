using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

public class Encrypt
{
    public string EncryptText (string toEncrypt){
        byte[] keyArray = UTF8Encoding.UTF8.GetBytes ("12345678901234567890123456789012");
        byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes (toEncrypt);
        RijndaelManaged rDel = new RijndaelManaged ();
        rDel.Key = keyArray;
        rDel.Mode = CipherMode.ECB;
        rDel.Padding = PaddingMode.PKCS7;
        ICryptoTransform cTransform = rDel.CreateEncryptor ();
        byte[] resultArray = cTransform.TransformFinalBlock (toEncryptArray, 0, toEncryptArray.Length);
        return Convert.ToBase64String (resultArray, 0, resultArray.Length);
    }
}
