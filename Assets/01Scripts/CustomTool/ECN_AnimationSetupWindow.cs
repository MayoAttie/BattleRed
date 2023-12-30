using UnityEditor;
using UnityEngine;
using UnityEditor.Animations;
using System.Collections.Generic;

public class ECN_AnimationSetupWindow : EditorWindow
{
    // 각 스테이트에 대한 파라미터 값을 저장할 딕셔너리
    public Dictionary<AnimatorState, int> stateMatchValue = new Dictionary<AnimatorState, int>();
    int parameter = 0;
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
        
        parameter = 0;
        stateMatchValue.Clear();

        // 커스텀 창에서 사용자가 parameterName을 입력할 수 있도록 TextField 추가
        parameterName = EditorGUILayout.TextField("Parameter Name", parameterName);

        if (GUILayout.Button("애니메이션 설정 자동화"))
        {
            AutomateAnimationSetup();
        }

        if (GUILayout.Button("모든 트랜지션 제거"))
        {
            RemoveAllTransitions();
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
                    foreach(ChildAnimatorState state in layer.stateMachine.states)
                    {   // 각 스테이트에 파라미터를 할당.
                        AnimatorState animatorState = state.state;
                        stateMatchValue.Add(animatorState, parameter);
                        parameter++;
                    }

                    foreach (ChildAnimatorState state in layer.stateMachine.states)
                    {
                        AnimatorState animatorState = state.state;
                        
                        // 자기 자신을 제외한 다른 상태들과 트랜지션 설정
                        foreach (ChildAnimatorState otherState in layer.stateMachine.states)
                        {
                            AnimatorState otherAniState = otherState.state;
                            if (otherState.state != animatorState)
                            {

                                // 각 트랜지션에 대한 고유한 파라미터 값 설정
                                int currentUniqueParameter = stateMatchValue[otherAniState];

                                // 트랜지션과 파라미터를 딕셔너리에 추가
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
        // 트랜지션 생성 및 설정
        AnimatorStateTransition transition = sourceState.AddTransition(destinationState);

        // 파라미터 조건 추가 (인트형 파라미터 설정)
        transition.AddCondition(AnimatorConditionMode.Equals, parameterValue, parameterName);

        return transition;
    }


    // 모든 트랜지션 제거
    void RemoveAllTransitions()
    {
        // 선택한 객체의 애니메이터 가져오기
        Animator animator = Selection.activeGameObject.GetComponent<Animator>();

        if (animator != null)
        {
            // Animator Controller 가져오기
            AnimatorController controller = animator.runtimeAnimatorController as AnimatorController;

            if (controller != null)
            {
                // 각 레이어에 대한 처리
                foreach (AnimatorControllerLayer layer in controller.layers)
                {
                    // 각 스테이트에 대한 처리
                    foreach (ChildAnimatorState state in layer.stateMachine.states)
                    {
                        // 해당 스테이트의 트랜지션 제거
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
        // 해당 스테이트의 모든 트랜지션 가져오기
        AnimatorStateTransition[] transitions = state.transitions;

        // 각 트랜지션 제거
        foreach (AnimatorStateTransition transition in transitions)
        {
            state.RemoveTransition(transition);
        }
    }
}
