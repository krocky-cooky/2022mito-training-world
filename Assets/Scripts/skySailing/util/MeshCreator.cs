using UnityEditor;
using UnityEngine;

public class MeshCreator : MonoBehaviour
{
    [SerializeField] private string path;

    [ContextMenu("Make mesh from model")]
    public void MakeMesh()
    {
#if UNITY_EDITOR
        var mesh = GetComponent<MeshFilter>();
        AssetDatabase.CreateAsset(mesh.mesh, path);
        AssetDatabase.SaveAssets();
#endif
    }
}