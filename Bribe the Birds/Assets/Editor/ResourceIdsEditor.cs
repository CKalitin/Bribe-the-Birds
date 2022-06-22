using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ResourceIds))]
public class ResourceIdsEditor : Editor {
    ResourceIds resourceIds;

    float verticalBreakSize;

    GUIStyle tooltipStyle;
    GUIStyle helpBoxStyle;

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        #region Variables

        resourceIds = (ResourceIds)target; // Get ResourceIds script which is the target

        verticalBreakSize = 5;

        tooltipStyle = new GUIStyle(GUI.skin.label) { fontSize = 13 };
        helpBoxStyle = new GUIStyle(GUI.skin.GetStyle("HelpBox")) { fontSize = 13 };

        #endregion

        VerticalBreak();

        // This button is used for updating resourceIds at runtime
        if (GUILayout.Button("Generate Resource Enums")) {
            resourceIds.UpdateEnums();

            // This is used to refresh the assets which adds the newly generated enums
            AssetDatabase.Refresh();
        }

        // A tooltip to inform the user about the use of the button above
        GUILayout.TextArea("Press this button every time you update the resource Ids array.", tooltipStyle);

        VerticalBreak();

        HelpBox();

        // This is so the Scriptable Objects save when Unity closes
        EditorUtility.SetDirty(target);
    }

    private void VerticalBreak() {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("", GUILayout.Height(verticalBreakSize)); // This is a break
        EditorGUILayout.EndHorizontal();
    }

    private void HelpBox() {
        string helpMessage =
            "This is where resource enums (The dropdown of resources) is configured. \n" +
            "In the array simply type in the resource names you want and click the generate button when finished. \n" +
            "From here you can use \"Resources.(ResourceName)\" in code when needing the resource id.\n" +
            "The Resource Enum Path is the path where the enum script will be stored.\n" +
            "Resource Enum Name is a custom name for the enums instead of \"Resources\", So you would do \"(Resource Enum Name).(Resource Name)\".";

        GUILayout.TextArea(helpMessage, helpBoxStyle);
    }
}
