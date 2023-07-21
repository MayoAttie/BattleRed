using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TouchPadController;

public class CharacterControlMng : MonoBehaviour
{
    [SerializeField] CharacterController controller;      // 캐릭터 컨트롤러
    [SerializeField] Transform groundCheck;               // 지면 체크를 위한 위치 정보를 저장하는 변수
    [SerializeField] LayerMask groundMask;                // 지면을 나타내는 레이어 정보를 저장하는 변수
    Vector3 velocity;
    bool isGrounded;                            // 지면인지 체크
    bool isJump;
    float jumpHeight = 2f;                      // 점프 높이
    float groundDistance = 1.2f;                // 지면과의 거리
    float zPos;                                 // 제어용 좌표 값
    float xPos;                                 // 제어용 좌표 값
    float rotationSpeed = 100f;                 // 캐릭터 회전 속도
    float gravity = -9.18f;                     // 중력
    bool isBattle;
    CharacterManager characMng;

    void Start()
    {
        characMng = CharacterManager.Instance;
    }

    // Update is called once per frame
    private void Update()
    {
        isBattle = characMng.getIsBattle();
        ControllerGetInputData();
        RotateCharacter();
        MoveCharacterFunction();
        JumpCharacterFunction();
    }

    // 조이스틱 컨트롤러, xy좌표 값 Get 함수
    void ControllerGetInputData()
    {
        if (isBattle)
            return;
        zPos = JoyStickController.Instance.GetVerticalValue();
        xPos = JoyStickController.Instance.GetHorizontalValue();

        characMng.GetCharacterClass().setState(CharacterClass.eCharactgerState.e_WALK);

    }



    // 워크 애니메이션 함수
    void MoveCharacterFunction()
    {
        if (isBattle)
            return;
        // 애니메이션을 실행할 때 필요한 파라미터 설정
        characMng.AnimatorFloatValueSetter(zPos, xPos);

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // 지면에 닿아있을 때 y 속도를 초기화
        }

        // 중력 적용
        velocity.y += gravity * Time.deltaTime;

        // 객체 이동
        Vector3 move = transform.right * xPos + transform.forward * zPos;
        controller.Move((move * 5 + velocity) * Time.deltaTime); // 중력이 적용된 이동

        // x,y 값이 0에 가까우면, 이동을 멈추고 iDle상태로 바꿈
        if (Mathf.Approximately(zPos, 0f) && Mathf.Approximately(xPos, 0f))
        {
            characMng.GetCharacterClass().setState(CharacterClass.eCharactgerState.e_Idle);
        }
    }

    // 점프버튼 함수
    public void JumpCommand()
    {
        if (isBattle)
            return;
        if (isGrounded)
        {
            isJump = true;
        }
    }

    // 점프 애니메이션 함수
    void JumpCharacterFunction()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        Debug.Log(isGrounded);
        if (isGrounded && isJump)
        {
            characMng.GetCharacterClass().setState(CharacterClass.eCharactgerState.e_JUMP);
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            isJump = false;
        }


        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }



    private void RotateCharacter()
    {

        e_TouchSlideDic touchDic;
        touchDic = TouchPadController.Instance.GetDirectionHorizontal();
        if (touchDic == e_TouchSlideDic.Right)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
        else if (touchDic == e_TouchSlideDic.Left)
        {
            transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
        }
    }

}
