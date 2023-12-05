using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ECTransformObject : EditorWindow
{
    private bool _bEditOn = true;

    [SerializeField]
    public List<GameObject> ins_GameObjects = new List<GameObject>();

    [MenuItem("Tools/ECTransformObject")]
    private static void Open()
    {
        ECTransformObject win = GetWindow<ECTransformObject>();
        win.titleContent = new GUIContent("Transform tool");
        win.Show();
    }

    private void OnEnable()
    {
        _bEditOn = true;
    }

    private void OnDisable()
    {
        _bEditOn = false;
    }

    private Vector3 _newPosition;

    private void OnGUI()
    {
        if (!_editor)
        {
            _editor = Editor.CreateEditor(this);
        }

        if (_editor)
        {
            _editor.OnInspectorGUI();
        }

        if (GUILayout.Button("리스트 초기화"))
        {
            ClearObjectList();
        }

        EditorGUILayout.Space();

        GUILayout.Label("Transform Object", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("현재 선택된 객체의 좌표 복사"))
        {
            CopySelectedObjectPosition();
        }

        _newPosition = EditorGUILayout.Vector3Field("이동할 좌표", _newPosition);

        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("객체 이동하기"))
        {
            MoveSelectedObjects();
        }
    }

    private void MoveSelectedObjects()
    {
        foreach (var obj in ins_GameObjects)
        {
            if (obj != null)
            {
                Undo.RecordObject(obj.transform, "Move Object");
                obj.transform.position = _newPosition;
            }
        }
    }

    private void ClearObjectList()
    {
        ins_GameObjects.Clear();
    }

    private void CopySelectedObjectPosition()
    {
        if (Selection.activeTransform != null)
        {
            _newPosition = Selection.activeTransform.position;
        }
    }

    private Editor _editor;
}
