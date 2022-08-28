using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System ;
using System.IO;


namespace util 
{
    public class SaveData 
    {
        public SaveData()
        {
            username = "username";
            description = "save data";
            torqueLog = new List<float>();
            timestamp = new List<int>();
        }
        public string username;
        public string description;
        public List<float> torqueLog;
        public List<int> timestamp;
    }

    public static class SaveManager
    {
        const string SAVE_FILE_PATH = "Assets/Scripts/data/";
        private static SaveData sd;

        public static void load(string loadingFileName)
        {
            
            try
            {
                #if UNITY_EDITOR
                    string path = Directory.GetCurrentDirectory();
                #else
                    string path = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
                #endif
                
                FileInfo info = new FileInfo(path + "/" + SAVE_FILE_PATH + loadingFileName);
                StreamReader reader = new StreamReader (info.OpenRead ());
                string json = reader.ReadToEnd ();
                sd = JsonUtility.FromJson<SaveData>(json);
            }
            catch (Exception e)
            {
                sd = new SaveData();

            }
            
        }

        private static void save(string filename) 
        {
            string data = JsonUtility.ToJson(sd,true);
            #if UNITY_EDITOR
                string path = Directory.GetCurrentDirectory();
            #else
                string path = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
            #endif
            path +=  ("/" + SAVE_FILE_PATH + filename);
            StreamWriter writer = new StreamWriter (path, false);
            writer.WriteLine (data);
            writer.Flush ();
            writer.Close ();
            Debug.Log("successfully saved !!!!");
        }

        //記録したトルクおよび継続時間の保存
        public static void saveTorque(List<float> torqueList,List<int> timestampList, string username)
        {
            Debug.Log("inside coroutine");
            //Debug.Log(sd.torqueLog);

            // ファイル名を作成
            DateTime TodayNow = DateTime.Now;
            string filename = TodayNow.Year.ToString() + "_" + TodayNow.Month.ToString() + "_" + TodayNow.Day.ToString() + "_" + TodayNow.Hour.ToString() + "_" + TodayNow.Minute.ToString() + "_" + TodayNow.Second.ToString() + "_record_" + username + ".json";
            Debug.Log(filename);
            // クラスの中身を記録
            sd.username = username;
            sd.torqueLog = torqueList;
            sd.timestamp = timestampList;

            save(filename);
        }

        public static void getRegisteredTorque(ref List<float> torqueList, ref List<int> timestamp, string loadingFileName)
        {
            load(loadingFileName);
            torqueList = sd.torqueLog;
            timestamp = sd.timestamp;
        }

    }

    public class SaveManagerSetup : MonoBehaviour 
    {
        void Start()
        {
            SaveManager.load("save.json");
        }
    }
}