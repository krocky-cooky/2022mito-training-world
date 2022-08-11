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
            description = "save data";
            torqueLog = new List<float>();
            timestamp = new List<int>();
        }
        public string description;
        public List<float> torqueLog;
        public List<int> timestamp;
    }

    public static class SaveManager
    {
        const string SAVE_FILE_PATH = "save.json";
        private static SaveData sd;

        public static void load()
        {
            
            try
            {
                #if UNITY_EDITOR
                    string path = Directory.GetCurrentDirectory();
                #else
                    string path = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
                #endif
                
                FileInfo info = new FileInfo(path + "/" + SAVE_FILE_PATH);
                StreamReader reader = new StreamReader (info.OpenRead ());
                string json = reader.ReadToEnd ();
                sd = JsonUtility.FromJson<SaveData>(json);
            }
            catch (Exception e)
            {
                sd = new SaveData();

            }
            
        }

        private static void save() 
        {
            string data = JsonUtility.ToJson(sd,true);
            #if UNITY_EDITOR
                string path = Directory.GetCurrentDirectory();
            #else
                string path = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
            #endif
            path +=  ("/" + SAVE_FILE_PATH);
            StreamWriter writer = new StreamWriter (path, false);
            writer.WriteLine (data);
            writer.Flush ();
            writer.Close ();
            Debug.Log("successfully saved !!!!");
        }

        //記録したトルクおよび継続時間の保存
        public static void saveTorque(List<float> torqueList,List<int> timestampList)
        {
            Debug.Log("inside coroutine");
            //Debug.Log(sd.torqueLog);
            
            sd.torqueLog = torqueList;
            sd.timestamp = timestampList;
            save();
        }

        public static void getRegisteredTorque(ref List<float> torqueList, ref List<int> timestamp)
        {
            load();
            torqueList = sd.torqueLog;
            timestamp = sd.timestamp;
        }

    }

    public class SaveManagerSetup : MonoBehaviour 
    {
        void Start()
        {
            SaveManager.load();
        }
    }
}