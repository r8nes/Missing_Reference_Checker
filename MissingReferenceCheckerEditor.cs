using UnityEditor;
using UnityEngine;

public class MissingReferenceCheckerWindow : EditorWindow
{
    [MenuItem("Tools/Missing Reference Checker")]
    private static void ShowWindow()
    {
        MissingReferenceCheckerWindow window = GetWindow<MissingReferenceCheckerWindow>();
        window.titleContent = new GUIContent("Missing Reference Checker");
        window.Show();
    }

    private GameObject _selectedGameObject;
    private bool _isChecking;
    private string _logMessages;

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLogMessage;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLogMessage;
    }

    private void HandleLogMessage(string logString, string stackTrace, LogType type)
    {
        _logMessages += logString + "\n";
    }

    private void OnGUI()
    {
        GUIStyle logStyle = new GUIStyle(GUI.skin.label);
        logStyle.wordWrap = true;
        logStyle.normal.textColor = Color.red;

        GUILayout.Label("Selected GameObject:", EditorStyles.boldLabel);
        _selectedGameObject = EditorGUILayout.ObjectField(_selectedGameObject, typeof(GameObject), true) as GameObject;

        if (!_isChecking && GUILayout.Button("Check Missing References"))
        {
            if (_selectedGameObject == null)
            {
                Debug.Log("Please select a GameObject in the Hierarchy to check for missing references.");
            }
            else
            {
                _isChecking = true;
                _logMessages = "";

                CheckMissingReferencesRecursive(_selectedGameObject);
                
                _isChecking = false;
                Debug.Log("Missing reference check completed.");
            }
        }

        if (_isChecking)
        {
            GUILayout.Label("Checking for missing references...");
        }
        else
        {
            GUILayout.Label(_logMessages, logStyle);
        }
    }


    /// <summary>
    /// Checks empty links in property
    /// </summary>
    /// <param name="rootObject">Checked object</param>
    private void CheckMissingReferencesRecursive(GameObject rootObject)
    {
        Component[] components = rootObject.GetComponents<Component>();

        foreach (Component component in components)
        {
            // May change switch to add or remove components
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