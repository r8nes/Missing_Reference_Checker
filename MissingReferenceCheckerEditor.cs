using UnityEditor;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MissingReferenceCheckerEditor : Editor
{
    // Menu item to check for missing references
    [MenuItem("Tools/Check Missing References")]
    private static void CheckMissingReferences()
    {
        GameObject selectedGameObject = Selection.activeGameObject;
        if (selectedGameObject == null)
        {
            Debug.Log("Please select a GameObject in the Hierarchy to check for missing references.");
            return;
        }
        
        Component[] components = selectedGameObject.GetComponents<Component>();
        
        foreach (Component component in components)
        {
            //Add specific component to ignore it.
            switch (component)
            {
                case MeshRenderer:
                case TextMeshPro:
                case TextMeshProUGUI:
                case Button:
                case Image:
                    continue;
            }

            SerializedObject serializedObject = new SerializedObject(component);
            SerializedProperty prop = serializedObject.GetIterator();

            while (prop.NextVisible(true))
            {
                if (prop.propertyType == SerializedPropertyType.ObjectReference && prop.objectReferenceValue == null)
                {
                    Debug.LogError("Missing reference in " + component.GetType().ToString() + " on GameObject " + selectedGameObject.name + ": Property " + prop.name, selectedGameObject);
                }
            }
        }

        Debug.Log("Missing reference check completed.");
    }
}
