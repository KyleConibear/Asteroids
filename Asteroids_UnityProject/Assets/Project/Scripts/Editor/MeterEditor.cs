using UnityEditor;
using UnityEngine;

namespace KyleConibear
{
    [CustomEditor(typeof(Meter))]
    public class MeterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            Meter meterScript = (Meter)target;
            if (GUILayout.Button("Add"))
            {
                meterScript.Add(1);
            }

            if (GUILayout.Button("Subtract"))
            {
                meterScript.Subtract(1);
            }
        }
    }
}