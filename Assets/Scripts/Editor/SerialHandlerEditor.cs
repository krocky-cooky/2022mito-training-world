using UnityEditor;
using UnityEngine;
using communication;

[CustomEditor(typeof(SerialHandler))]
public class SerialHandlerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        SerialHandler serialHandler = target as SerialHandler;
        
        //シリアル通信
        if(GUILayout.Button("Send Text"))
        {
            serialHandler.sendFieldText();
        }
    }
}