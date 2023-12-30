using UnityEditor;
using UnityEngine;
using UnityEditor.Animations;
using System.Collections.Generic;

public class ECN_AnimationSetupWindow : EditorWindow
{
    // �� ������Ʈ�� ���� �Ķ���� ���� ������ ��ųʸ�
    public Dictionary<AnimatorState, int> stateMatchValue = new Dictionary<AnimatorState, int>();
    int parameter = 0;
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
        
        parameter = 0;
        stateMatchValue.Clear();

        // Ŀ���� â���� ����ڰ� parameterName�� �Է��� �� �ֵ��� TextField �߰�
        parameterName = EditorGUILayout.TextField("Parameter Name", parameterName);

        if (GUILayout.Button("�ִϸ��̼� ���� �ڵ�ȭ"))
        {
            AutomateAnimationSetup();
        }

        if (GUILayout.Button("��� Ʈ������ ����"))
        {
            RemoveAllTransitions();
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
                    foreach(ChildAnimatorState state in layer.stateMachine.states)
                    {   // �� ������Ʈ�� �Ķ���͸� �Ҵ�.
                        AnimatorState animatorState = state.state;
                        stateMatchValue.Add(animatorState, parameter);
                        parameter++;
                    }

                    foreach (ChildAnimatorState state in layer.stateMachine.states)
                    {
                        AnimatorState animatorState = state.state;
                        
                        // �ڱ� �ڽ��� ������ �ٸ� ���µ�� Ʈ������ ����
                        foreach (ChildAnimatorState otherState in layer.stateMachine.states)
                        {
                            AnimatorState otherAniState = otherState.state;
                            if (otherState.state != animatorState)
                            {

                                // �� Ʈ�����ǿ� ���� ������ �Ķ���� �� ����
                                int currentUniqueParameter = stateMatchValue[otherAniState];

                                // Ʈ�����ǰ� �Ķ���͸� ��ųʸ��� �߰�
                                AddTransitionWithParameter(animatorState, otherAniState, parameterName, currentUniqueParameter);
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

    AnimatorStateTransition AddTransitionWithParameter(AnimatorState sourceState, AnimatorState destinationState, string parameterName, int parameterValue)
    {
        // Ʈ������ ���� �� ����
        AnimatorStateTransition transition = sourceState.AddTransition(destinationState);

        // �Ķ���� ���� �߰� (��Ʈ�� �Ķ���� ����)
        transition.AddCondition(AnimatorConditionMode.Equals, parameterValue, parameterName);

        return transition;
    }


    // ��� Ʈ������ ����
    void RemoveAllTransitions()
    {
        // ������ ��ü�� �ִϸ����� ��������
        Animator animator = Selection.activeGameObject.GetComponent<Animator>();

        if (animator != null)
        {
            // Animator Controller ��������
            AnimatorController controller = animator.runtimeAnimatorController as AnimatorController;

            if (controller != null)
            {
                // �� ���̾ ���� ó��
                foreach (AnimatorControllerLayer layer in controller.layers)
                {
                    // �� ������Ʈ�� ���� ó��
                    foreach (ChildAnimatorState state in layer.stateMachine.states)
                    {
                        // �ش� ������Ʈ�� Ʈ������ ����
                        RemoveTransitions(state.state);
                    }
                }

                Debug.Log("All Transitions Removed!");
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

        parameter = 0;
        stateMatchValue.Clear();
    }

    void RemoveTransitions(AnimatorState state)
    {
        // �ش� ������Ʈ�� ��� Ʈ������ ��������
        AnimatorStateTransition[] transitions = state.transitions;

        // �� Ʈ������ ����
        foreach (AnimatorStateTransition transition in transitions)
        {
            state.RemoveTransition(transition);
        }
    }
}
