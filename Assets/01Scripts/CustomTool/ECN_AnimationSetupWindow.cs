using UnityEditor;
using UnityEngine;
using UnityEditor.Animations;
using System.Collections.Generic;

public class ECN_AnimationSetupWindow : EditorWindow
{
    // 각 트랜지션에 대한 파라미터 값을 저장할 딕셔너리
    public Dictionary<AnimatorStateTransition, int> transitionParameters = new Dictionary<AnimatorStateTransition, int>();

    // 사용자가 입력할 파라미터의 이름
    public string parameterName = "MyIntParameter";

    [MenuItem("Tools/ECN_AnimationConnectAutometic")]
    public static void ShowWindow()
    {
        GetWindow(typeof(ECN_AnimationSetupWindow), false, "Animation Setup");
    }

    void OnGUI()
    {
        GUILayout.Label("애니메이션 설정", EditorStyles.boldLabel);

        // 커스텀 창에서 사용자가 parameterName을 입력할 수 있도록 TextField 추가
        parameterName = EditorGUILayout.TextField("Parameter Name", parameterName);

        if (GUILayout.Button("애니메이션 설정 자동화"))
        {
            AutomateAnimationSetup();
        }
    }

    void AutomateAnimationSetup()
    {
        // 선택한 객체의 애니메이터 가져오기
        Animator animator = Selection.activeGameObject.GetComponent<Animator>();

        if (animator != null)
        {
            // Animator Controller 가져오기
            AnimatorController controller = animator.runtimeAnimatorController as AnimatorController;

            if (controller != null)
            {
                int uniqueParameter = 0; // 시작 값 설정

                // 각 애니메이션 클립에 대한 설정 수행
                foreach (AnimatorControllerLayer layer in controller.layers)
                {
                    foreach (ChildAnimatorState state in layer.stateMachine.states)
                    {
                        AnimatorState animatorState = state.state;

                        // 자기 자신을 제외한 다른 상태들과 트랜지션 설정
                        foreach (ChildAnimatorState otherState in layer.stateMachine.states)
                        {
                            if (otherState.state != animatorState)
                            {
                                // 각 트랜지션에 대한 고유한 파라미터 값 설정
                                int currentUniqueParameter = uniqueParameter;

                                // 트랜지션과 파라미터를 딕셔너리에 추가
                                AnimatorStateTransition transition = AddTransitionWithParameter(animatorState, otherState.state, parameterName, currentUniqueParameter);

                                // 딕셔너리에 트랜지션과 파라미터 추가
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
        // 각 트랜지션에 대한 고유한 파라미터 값을 생성하거나 가져오는 로직을 추가
        // 여기서는 간단히 두 상태의 해시코드를 더한 값을 사용
        return sourceState.name.GetHashCode() + destinationState.name.GetHashCode();
    }

    AnimatorStateTransition AddTransitionWithParameter(AnimatorState sourceState, AnimatorState destinationState, string parameterName, int parameterValue)
    {
        // 트랜지션 생성 및 설정
        AnimatorStateTransition transition = sourceState.AddTransition(destinationState);

        // 파라미터 조건 추가 (인트형 파라미터 설정)
        transition.AddCondition(AnimatorConditionMode.Equals, parameterValue, parameterName);

        return transition;
    }
}
