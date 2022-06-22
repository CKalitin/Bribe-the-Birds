using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName ="ResourceIds", menuName = "Scriptable Objects/Resources/ResourceIds", order =0)]
public class ResourceIds : ScriptableObject {
    [SerializeField] private string[] resourceIds;
    [Space]
    [SerializeField] private string resourceEnumPath = "Assets/Other/Enums/";
    [SerializeField] private string resourceEnumName = "Resources";

    public string[] ResourceIdsArray { get => resourceIds; set => resourceIds = value; }
    public string ResourceEnumPath { get => resourceEnumPath; set => resourceEnumPath = value; }
    public string ResourceEnumName { get => resourceEnumName; set => resourceEnumName = value; }

    // This is used in the custom inspector / editor script
    public void UpdateEnums() {
        string filePathAndName = resourceEnumPath + resourceEnumName + ".cs"; //The folder where the enum script is expected to exist

        using (StreamWriter streamWriter = new StreamWriter(filePathAndName)) {
            // Put the damned bracket on the right line
            streamWriter.WriteLine("public enum " + resourceEnumName + " { ");
            for (int i = 0; i < resourceIds.Length; i++) {
                streamWriter.WriteLine("\t" + resourceIds[i] + ",");
            }
            streamWriter.WriteLine("}");
        }
    }
}
