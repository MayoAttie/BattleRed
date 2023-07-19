using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TouchPadController;

public class CharacterManager : MonoBehaviour
{
    CharacterClass clsCharacter;
    Animator aniController;
    public CharacterController controller;      // 캐릭터 컨트롤 콜라이더
    public Transform groundCheck;               // 지면 체크를 위한 위치 정보를 저장하는 변수
    public LayerMask groundMask;                // 지면을 나타내는 레이어 정보를 저장하는 변수
    Vector3 velocity;
    bool isGrounded;                            // 지면인지 체크
    bool isJump;
    public float jumpHeight = 3f;               // 점프 높이
    float groundDistance = 0.4f;                // 지면과의 거리
    float zPos;                                 // 제어용 좌표 값
    float xPos;                                 // 제어용 좌표 값
    float yPos;                                 // 제어용 좌표 값
    float rotationSpeed = 100f;                   // 캐릭터 회전 속도
    public float gravity = -9.18f;              // 중력

    private void Awake()
    {
        aniController = gameObject.GetComponent<Animator>();
    }

    void Update()
    {
        clsCharacter = GameManager.Instance.characterCls;
        ControllerGetInputData();
        RotateCharacter();
        MoveCharacterFunction();
        JumpCharacterFunction();

        Debug.Log(clsCharacter.getState());
        CharacterStateActor();  // 캐릭터 애니메이터 제어 함수
    }

    // 캐릭터 애니메이터 제어 함수
    public void CharacterStateActor()
    {
        

        switch (clsCharacter.getState())
        {
            case CharacterClass.eCharactgerState.e_Idle:
                aniController.SetInteger("Controller", 0);
                break;
            case CharacterClass.eCharactgerState.e_WALK:
                aniController.SetInteger("Controller", -1);
                break;
            case CharacterClass.eCharactgerState.e_RUN:
                break;
            case CharacterClass.eCharactgerState.e_JUMP:
                aniController.SetInteger("Controller", 1);
                break;
            case CharacterClass.eCharactgerState.e_AVOID:
                break;
            case CharacterClass.eCharactgerState.e_ATTACK:
                break;
            case CharacterClass.eCharactgerState.e_HIT:
                break;
            case CharacterClass.eCharactgerState.e_DEAD: 
                break;
            default:
                aniController.SetInteger("Controller", -1);
                break;
        }
    }

    // 워크 애니메이션 함수
    void MoveCharacterFunction()
    {
        // 애니메이션을 실행할 때 필요한 파라미터 설정
        aniController.SetFloat("zPos", zPos);
        aniController.SetFloat("xPos", xPos);

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // 객체 이동
        Vector3 move = transform.right * xPos + transform.forward * zPos;
        controller.Move(move * 5 * Time.deltaTime);

        // x,y 값이 0에 가까우면, 이동을 멈추고 iDle상태로 바꿈
        if (Mathf.Approximately(zPos, 0f) && Mathf.Approximately(xPos, 0f))
            clsCharacter.setState(CharacterClass.eCharactgerState.e_Idle);

    }

    // 조이스틱 컨트롤러, xy좌표 값 Get 함수
    void ControllerGetInputData()
    {
        zPos = JoyStickController.Instance.GetVerticalValue();
        xPos = JoyStickController.Instance.GetHorizontalValue();
        
        clsCharacter.setState(CharacterClass.eCharactgerState.e_WALK);
    }
    

    // 점프버튼 함수
    public void JumpCommand()
    {
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
            clsCharacter.setState(CharacterClass.eCharactgerState.e_JUMP);
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            isJump = false;
        }


        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }



    private void RotateCharacter()
    {

        Vector2 vec_inputDirection = TouchPadController.Instance.GetDirection();



        Debug.Log(nameof(vec_inputDirection) + ":" + vec_inputDirection);

        // 캐릭터의 방향을 터치 입력에 따라 회전시킵니다.
        if (vec_inputDirection != Vector2.zero)
        {
            // 현재 터치 입력에 대한 각도를 계산합니다.
            float targetAngle = Mathf.Atan2(vec_inputDirection.x, vec_inputDirection.y) * Mathf.Rad2Deg;

            // 캐릭터의 현재 회전 각도를 가져옵니다.
            float currentAngle = transform.eulerAngles.y;
            Debug.Log(nameof(currentAngle) + ":" + currentAngle);

            // 목표 각도와 현재 각도의 차이를 계산합니다.
            float angleDifference = Mathf.DeltaAngle(currentAngle, targetAngle);

            // 보정된 회전 각도 계산: 회전 각도가 -180도에서 180도 사이가 되도록 보정합니다.
            if (Mathf.Abs(angleDifference) > 180f)
            {
                angleDifference -= Mathf.Sign(angleDifference) * 360f;
            }

            // 회전 속도에 따라 적절한 회전 각도를 구합니다.
            float rotationStep = Mathf.Sign(angleDifference) * Mathf.Min(Mathf.Abs(angleDifference), rotationSpeed * Time.deltaTime);

            // 회전 각도를 적용하여 캐릭터를 회전시킵니다.
            transform.rotation = Quaternion.Euler(0f, currentAngle + rotationStep, 0f);
        }
    }


}