using UnityEditor;
using UnityEngine;
using UnityEditor.Animations;
using System.Collections.Generic;

public class ECN_AnimationSetupWindow : EditorWindow
{
    // �� Ʈ�����ǿ� ���� �Ķ���� ���� ������ ��ųʸ�
    public Dictionary<AnimatorStateTransition, int> transitionParameters = new Dictionary<AnimatorStateTransition, int>();

    // ����ڰ� �Է��� �Ķ������ �̸�
    public string parameterName = "MyIntParameter";

    [MenuItem("Tools/ECN_AnimationConnectAutometic")]
    public static void ShowWindow()
    {
        GetWindow(typeof(ECN_AnimationSetupWindow), false, "Animation Setup");
    }

    void OnGUI()
    {
        GUILayout.Label("�ִϸ��̼� ����", EditorStyles.boldLabel);

        // Ŀ���� â���� ����ڰ� parameterName�� �Է��� �� �ֵ��� TextField �߰�
        parameterName = EditorGUILayout.TextField("Parameter Name", parameterName);

        if (GUILayout.Button("�ִϸ��̼� ���� �ڵ�ȭ"))
        {
            AutomateAnimationSetup();
        }
    }

    void AutomateAnimationSetup()
    {
        // ������ ��ü�� �ִϸ����� ��������
        Animator animator = Selection.activeGameObject.GetComponent<Animator>();

        if (animator != null)
        {
            // Animator Controller ��������
            AnimatorController controller = animator.runtimeAnimatorController as AnimatorController;

            if (controller != null)
            {
                int uniqueParameter = 0; // ���� �� ����

                // �� �ִϸ��̼� Ŭ���� ���� ���� ����
                foreach (AnimatorControllerLayer layer in controller.layers)
                {
                    foreach (ChildAnimatorState state in layer.stateMachine.states)
                    {
                        AnimatorState animatorState = state.state;

                        // �ڱ� �ڽ��� ������ �ٸ� ���µ�� Ʈ������ ����
                        foreach (ChildAnimatorState otherState in layer.stateMachine.states)
                        {
                            if (otherState.state != animatorState)
                            {
                                // �� Ʈ�����ǿ� ���� ������ �Ķ���� �� ����
                                int currentUniqueParameter = uniqueParameter;

                                // Ʈ�����ǰ� �Ķ���͸� ��ųʸ��� �߰�
                                AnimatorStateTransition transition = AddTransitionWithParameter(animatorState, otherState.state, parameterName, currentUniqueParameter);

                                // ��ųʸ��� Ʈ�����ǰ� �Ķ���� �߰�
                                transitionParameters.Add(transition, currentUniqueParameter);
                            }
                        }
                        uniqueParameter++;
                    }
                }

                Debug.Log("Animation Setup Completed!");
            }
            else
            {
                Debug.LogError("No Animator Controller found on the selected object.");
            }
        }
        else
        {
            Debug.LogError("No Animator component found on the selected object.");
        }
    }

    int GetUniqueParameterForTransition(AnimatorState sourceState, AnimatorState destinationState)
    {
        // �� Ʈ�����ǿ� ���� ������ �Ķ���� ���� �����ϰų� �������� ������ �߰�
        // ���⼭�� ������ �� ������ �ؽ��ڵ带 ���� ���� ���
        return sourceState.name.GetHashCode() + destinationState.name.GetHashCode();
    }

    AnimatorStateTransition AddTransitionWithParameter(AnimatorState sourceState, AnimatorState destinationState, string parameterName, int parameterValue)
    {
        // Ʈ������ ���� �� ����
        AnimatorStateTransition transition = sourceState.AddTransition(destinationState);

        // �Ķ���� ���� �߰� (��Ʈ�� �Ķ���� ����)
        transition.AddCondition(AnimatorConditionMode.Equals, parameterValue, parameterName);

        return transition;
    }
}
