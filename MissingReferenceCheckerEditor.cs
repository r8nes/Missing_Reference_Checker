using UnityEditor;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MissingReferenceCheckerEditor : Editor
{
    [MenuItem("Tools/Check Missing References")]
    private static void CheckMissingReferences()
    {
        GameObject selectedGameObject = Selection.activeGameObject;
        if (selectedGameObject == null)
        {
            Debug.Log("Please select a GameObject in the Hierarchy to check for missing references.");
            return;
        }

        CheckMissingReferencesRecursive(selectedGameObject);

        Debug.Log("Missing reference check completed.");
    }

    private static void CheckMissingReferencesRecursive(GameObject rootObject)
    {
        Component[] components = rootObject.GetComponents<Component>();
        
        foreach (Component component in components)
        {
            switch (component)
            {
                case MonoBehaviour:
                    break;
                default:
                    continue;
            }

            SerializedObject serializedObject = new SerializedObject(component);
            SerializedProperty prop = serializedObject.GetIterator();
            while (prop.NextVisible(true))
            {
                if (prop.propertyType == SerializedPropertyType.ObjectReference && prop.objectReferenceValue == null)
                {
                    Debug.LogError("Missing reference in " + component.GetType().ToString() + " on GameObject " + rootObject.name + ": Property " + prop.name, rootObject);
                }
            }
        }

        int childCount = rootObject.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform child = rootObject.transform.GetChild(i);
            CheckMissingReferencesRecursive(child.gameObject);
        }
    }
}