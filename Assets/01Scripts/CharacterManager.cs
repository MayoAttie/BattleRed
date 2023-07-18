using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    CharacterClass clsCharacter;
    Animator aniController;
    [SerializeField] JoyStickController joyStick;    // 조이스틱 객체
    public CharacterController controller;      // 캐릭터 컨트롤 콜라이더
    public Transform groundCheck;               // 지면 체크를 위한 위치 정보를 저장하는 변수
    public LayerMask groundMask;                // 지면을 나타내는 레이어 정보를 저장하는 변수
    Vector3 velocity;
    bool isGrounded;                            // 지면인지 체크
    bool isJump;
    public float jumpHeight = 3f;               // 점프 높이
    float groundDistance = 0.4f;
    float zPos;
    float xPos;
    public float gravity = -9.18f;

    private void Awake()
    {
        aniController = gameObject.GetComponent<Animator>();
    }

    void Update()
    {
        clsCharacter = GameManager.Instance.characterCls;
        JoyStickGetPos();
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
    void JoyStickGetPos()
    {   
        zPos = joyStick.GetVerticalValue();
        xPos = joyStick.GetHorizontalValue();

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

}