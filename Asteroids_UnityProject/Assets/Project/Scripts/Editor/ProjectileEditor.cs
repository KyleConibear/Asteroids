using UnityEditor;
using UnityEngine;

namespace KyleConibear
{
    [CustomEditor(typeof(Projectile))]
    public class ProjectileEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            Projectile projectileScript = (Projectile)target;
            if (GUILayout.Button("Fire"))
            {
                projectileScript.Fire();
            }
        }
    }
}