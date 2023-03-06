using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;
using System.Net;
using UnityEngine.iOS;

public class ActivateLicense : MonoBehaviour
{

    public GameObject Success;
    public GameObject Error;
    public GameObject ServerError;
    public GameObject NextButton;
    public GameObject ActivateButton;

    public void ActivateSerialNumber(){
        var check = CheckForInternetConnection(3000,"http://34.133.85.123:4000");

        if (check == false){
            // gagal ping server
            ServerError.SetActive(true);
        }else{
            ServerError.SetActive(false);
        }
        GameObject serial_number_input = GameObject.Find("InputField");

        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaClass up = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject> ("currentActivity");
            AndroidJavaObject contentResolver = currentActivity.Call<AndroidJavaObject> ("getContentResolver");
            AndroidJavaClass secure = new AndroidJavaClass ("android.provider.Settings$Secure");
            string deviceId = secure.CallStatic<string> ("getString", contentResolver, "android_id");
            string reqBody = string.Format("{2}\"serial\":\"{0}\",\"machineId\":\"{1}\"{3}", serial_number_input.GetComponent<TMP_InputField>().text, deviceId, "{", "}");
            StartCoroutine(GetText("http://34.133.85.123:4001/api/seat/activate", reqBody));
        }else if (Application.platform == RuntimePlatform.IPhonePlayer){
            string reqBody = string.Format("{2}\"serial\":\"{0}\",\"machineId\":\"{1}\"{3}", serial_number_input.GetComponent<TMP_InputField>().text, Device.vendorIdentifier, "{", "}");
            StartCoroutine(GetText("http://34.133.85.123:4001/api/seat/activate", reqBody));
        }else{
            string reqBody = string.Format("{2}\"serial\":\"{0}\",\"machineId\":\"{1}\"{3}", serial_number_input.GetComponent<TMP_InputField>().text, SystemInfo.deviceUniqueIdentifier, "{", "}");
            StartCoroutine(GetText("http://34.133.85.123:4001/api/seat/activate", reqBody));
        }            
    }

    public void saveMachineUID(string machineUID, DateTime startdate, DateTime expirydate){
        Encrypt writter = new Encrypt();
        string encryptedUID;
        string startingDate;
        string expiredDate;
        var path = Application.persistentDataPath;
        // string filename = Application.platform == RuntimePlatform.Android ? "sl1.xml" : "sl1.xml";
        string filename = "sl1.xml";
        encryptedUID = writter.EncryptText(machineUID);
        startingDate = writter.EncryptText(startdate.ToString());
        expiredDate = writter.EncryptText(expirydate.ToString());
        string[] values = { encryptedUID, startingDate, expiredDate };
        System.IO.File.WriteAllLines(string.Format(@"{0}\{1}", path, filename), values);
    }

    IEnumerator GetText(string url, string bodyJsonString) {    
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.timeout = 10000;
        yield return request.SendWebRequest();

        // Debug.Log("Status Code: " + request.responseCode);
        
        if (request.responseCode == 200){
            Success.SetActive(true);
            Error.SetActive(false);
            NextButton.SetActive(true);
            ActivateButton.SetActive(false);
        }else{
            Error.SetActive(true);
            Error.GetComponent<TextMeshProUGUI>().text = "Can't activate the application!<br>Error Code : "+request.responseCode.ToString();
            // kalau udah production delete
            // Error.GetComponent<TextMeshProUGUI>().text = bodyJsonString;
            Success.SetActive(false);
        };

        string res = request.downloadHandler.text;
        LicenseDetail sapi = JsonUtility.FromJson<LicenseDetail>(res);
        try{
            DateTime startdate = DateTime.Parse(sapi.startDate, null, System.Globalization.DateTimeStyles.RoundtripKind);
            DateTime expirydate = DateTime.Parse(sapi.expiryDate, null, System.Globalization.DateTimeStyles.RoundtripKind);
            saveMachineUID(sapi.machineId, startdate, expirydate);
        }catch{
            // Debug.Log("Serial number wrong!");
        }
        
    }

    public bool CheckForInternetConnection(int timeoutMs = 300, string url = null)
    {
        try
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.KeepAlive = false;
            request.Timeout = timeoutMs;
            using (var response = (HttpWebResponse)request.GetResponse())
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void GoToHome(){
       SceneManager.LoadScene((1), LoadSceneMode.Single);
    }
}
