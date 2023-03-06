using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

public class Decrypt
{
    public string DecryptText (string toDecrypt){
        try{
        byte[] keyArray = UTF8Encoding.UTF8.GetBytes ("12345678901234567890123456789012");
        byte[] toEncryptArray = Convert.FromBase64String (toDecrypt);
        RijndaelManaged rDel = new RijndaelManaged ();
        rDel.Key = keyArray;
        rDel.Mode = CipherMode.ECB;
        rDel.Padding = PaddingMode.PKCS7;
        ICryptoTransform cTransform = rDel.CreateDecryptor ();
        byte[] resultArray = cTransform.TransformFinalBlock (toEncryptArray, 0, toEncryptArray.Length);
        return UTF8Encoding.UTF8.GetString (resultArray);
        }
        catch(Exception e){
            // SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
            ExceptionMessage bebek = new ExceptionMessage();
            bebek.msg = e.Message;
            return bebek.msg;
        }
        
    }
}
