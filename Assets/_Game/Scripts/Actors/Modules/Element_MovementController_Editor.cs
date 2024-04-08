
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Element_MovementController))]
public class Element_MovementController_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Element_MovementController elementMovement = (Element_MovementController)target;
        if (GUILayout.Button("Update Movement Type"))
        {
            elementMovement.UpdateMovement();
        }
    }
}
