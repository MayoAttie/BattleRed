using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;


public class ECNHierarchyWondow_ScriptFinder : EditorWindow
{
    private MonoScript targetScript;
    private List<GameObject> filteredObjects = new List<GameObject>();
    private string searchString = "";

    [MenuItem("Find_Tools/ECNHierarchyWondow_ScriptFinder")]
    private static void ShowWindow()
    {
        EditorWindow.GetWindow<ECNHierarchyWondow_ScriptFinder>("계층 도구");
    }

    private void OnEnable()
    {
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyItemOnGUI;
    }

    private void OnDisable()
    {
        EditorApplication.hierarchyWindowItemOnGUI -= HierarchyItemOnGUI;
    }

    private void OnGUI()
    {
        GUILayout.Label("사용자 정의 계층 창", EditorStyles.boldLabel);

        targetScript = EditorGUILayout.ObjectField("대상 스크립트", targetScript, typeof(MonoScript), false) as MonoScript;

        searchString = EditorGUILayout.TextField("검색", searchString);

        if (GUILayout.Button("객체 찾기"))
        {
            FindObjectsWithComponent();
        }

        if (GUILayout.Button("검색 지우기"))
        {
            searchString = "";
            filteredObjects.Clear();
        }
    }

    private void FindObjectsWithComponent()
    {
        filteredObjects.Clear();

        if (targetScript != null)
        {
            System.Type targetType = targetScript.GetClass();

            GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

            foreach (GameObject obj in allObjects)
            {
                Component component = obj.GetComponent(targetType);

                if (component != null)
                {
                    filteredObjects.Add(obj);
                }
            }

            // 검색 필터 적용
            if (!string.IsNullOrEmpty(searchString))
            {
                filteredObjects = filteredObjects.FindAll(obj => obj.name.Contains(searchString));
            }
        }
        else
        {
            Debug.LogWarning("대상 스크립트가 할당되지 않았습니다.");
        }
    }

    private void HierarchyItemOnGUI(int instanceID, Rect selectionRect)
    {
        GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

        if (obj != null && filteredObjects.Contains(obj))
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.green;

            EditorGUI.LabelField(selectionRect, obj.name, style);
        }
    }
}
