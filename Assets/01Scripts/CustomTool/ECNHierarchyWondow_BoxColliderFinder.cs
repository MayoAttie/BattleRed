using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class ECNHierarchyWondow_BoxColliderFinder : EditorWindow
{
    private List<GameObject> objectsWithBoxCollider = new List<GameObject>();
    private string searchString = "";

    [MenuItem("Find_Tools/ECNHierarchyWondow_BoxColliderFinder")]
    private static void ShowWindow()
    {
        EditorWindow.GetWindow<ECNHierarchyWondow_BoxColliderFinder>("계층 도구");
    }

    private void OnEnable()
    {
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyItemOnGUI;
        FindObjectsWithBoxCollider();
    }

    private void OnDisable()
    {
        EditorApplication.hierarchyWindowItemOnGUI -= HierarchyItemOnGUI;
    }

    private void OnGUI()
    {
        GUILayout.Label("박스 콜라이더를 가진 객체 찾기", EditorStyles.boldLabel);

        searchString = EditorGUILayout.TextField("검색", searchString);

        if (GUILayout.Button("새로고침"))
        {
            FindObjectsWithBoxCollider();
        }

        if (GUILayout.Button("검색 지우기"))
        {
            searchString = "";
        }
    }

    private void FindObjectsWithBoxCollider()
    {
        objectsWithBoxCollider.Clear();

        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.GetComponent<BoxCollider>() != null)
            {
                objectsWithBoxCollider.Add(obj);
            }
        }
    }

    private void HierarchyItemOnGUI(int instanceID, Rect selectionRect)
    {
        GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

        if (obj != null && objectsWithBoxCollider.Contains(obj))
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.green;

            EditorGUI.LabelField(selectionRect, obj.name, style);
        }
    }
}
    