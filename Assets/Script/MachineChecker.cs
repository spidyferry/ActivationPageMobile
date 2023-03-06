using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.iOS;

public class MachineChecker : MonoBehaviour
{
    public GameObject canvas;
    // public GameObject errorcheck;
    
    void Start()
    {   
        bool active = expiredDateChecker() && machineUIDChecker();
        if (active){
            // masuk ke scene utama
            // Debug.Log("Masuk ke scene utama");
            SceneManager.LoadScene((1), LoadSceneMode.Single);
        }else{            
            canvas.SetActive(!active);
            // Debug.Log("Engga ada filenya, codenya atau tanggal nya error");
        }
    }

    public bool machineUIDChecker(){
        var path = Application.persistentDataPath;
        
        string filename = Application.platform == RuntimePlatform.Android ? "sl1.xml" : "sl1.xml";
        // Debug.Log(filename);

        if (File.Exists(string.Format(@"{0}\{1}", path, filename))){
            // errorcheck.GetComponent<TextMeshProUGUI>().text = (path+filename+" :ketemu mesin");
            string[] lines = System.IO.File.ReadAllLines(string.Format(@"{0}\{1}", path, filename));
            Decrypt checker = new Decrypt();
            var decryptedUID = checker.DecryptText(lines[0]);
            if(Application.platform == RuntimePlatform.Android){
                AndroidJavaClass up = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject> ("currentActivity");
                AndroidJavaObject contentResolver = currentActivity.Call<AndroidJavaObject> ("getContentResolver");
                AndroidJavaClass secure = new AndroidJavaClass ("android.provider.Settings$Secure");
                string deviceId = secure.CallStatic<string> ("getString", contentResolver, "android_id");
                var confirmation = deviceId == decryptedUID?true:false;
                return confirmation;
            }else if(Application.platform == RuntimePlatform.IPhonePlayer){
                var confirmation = Device.vendorIdentifier == decryptedUID?true:false;
                return confirmation;
            }else{
                var confirmation = SystemInfo.deviceUniqueIdentifier == decryptedUID?true:false;
                // Debug.Log(decryptedUID);
                // Debug.Log(SystemInfo.deviceUniqueIdentifier);
                // Debug.Log(confirmation);
                return confirmation;
            }            
        }else{
            // errorcheck.GetComponent<TextMeshProUGUI>().text = "Ga ketemu filenya mesin";
            // Debug.Log("Mesin belum diaktivasi");
            return false;
        }        
    }

    public bool expiredDateChecker(){
        var confirmation = true;
        string filename = Application.platform == RuntimePlatform.Android ? "sl1.xml" : "sl1.xml";
        var path = Application.persistentDataPath;

        if (File.Exists(string.Format(@"{0}\{1}", path, filename))){
            // errorcheck.GetComponent<TextMeshProUGUI>().text = (path+filename+" :ketemu date");
            string[] lines = System.IO.File.ReadAllLines(string.Format(@"{0}\{1}", path, filename));
            Decrypt checker = new Decrypt();
            var decryptedStartingDate = checker.DecryptText(lines[1]);
            var decryptedExpiredDate = checker.DecryptText(lines[2]);
            try{
                DateTime d1 = DateTime.Parse(decryptedStartingDate, null, System.Globalization.DateTimeStyles.RoundtripKind);
                DateTime d2 = DateTime.Parse(decryptedExpiredDate, null, System.Globalization.DateTimeStyles.RoundtripKind);
                DateTime now = DateTime.Now;
                confirmation = now >= d1 && now <= d2?true:false;
                // Debug.Log(confirmation);
                // Debug.Log(d1.ToString()+d2.ToString()+now.ToString());
            }catch{
                confirmation = false;
                // Debug.Log("error");
            }
            return confirmation;
        }else{
            // errorcheck.GetComponent<TextMeshProUGUI>().text = "Ga ketemu filenya tanggal";
            // Debug.Log("sudah expired");
            return false;
        } 
    }
}
