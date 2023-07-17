using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    CharacterClass clsCharacter;
    Animator aniController;
    [SerializeField] JoyStickController joyStick;    // 조이스틱 객체
    public CharacterController controller;      // 캐릭터 컨트롤 콜라이더
    public Transform groundCheck;
    public LayerMask groundMask;
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
        AniControllerConvert(0);
    }

    void Update()
    {
        JoyStickGetPos();
        MoveCharacterFunction();
        JumpCharacterFunction();
    }

    void CharacterStateActor()
    {
        switch(clsCharacter.getState())
        {
            case CharacterClass.e_NONE:
                break;

        }
    }

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

        // 애니메이션 컨트롤러 파라미터 변경
        if (Mathf.Approximately(zPos, 0f) && Mathf.Approximately(xPos, 0f))
        {
            AniControllerConvert(0); // 이동 중이 아니므로 0번 값으로 변경
        }
        else
        {
            AniControllerConvert(-1); // 이동 중이므로 -1로 변경
        }
    }

    void JoyStickGetPos()
    {
        zPos = joyStick.GetVerticalValue();
        xPos = joyStick.GetHorizontalValue();
    }

    public void JumpCommand()
    {
        if (isGrounded)
        {
            isJump = true;
        }
    }

    void JumpCharacterFunction()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && isJump)
        {
            AniControllerConvert(-1);
            aniController.SetInteger("Jumping", 1); // 점프 동작 실행
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            isJump = false;
        }
        else
        {
            AniControllerConvert(0);
            aniController.SetInteger("Jumping", 0); // 지면으로 내려왔으므로 점프 동작 중지
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    void AniControllerConvert(int num)
    {
        aniController.SetInteger("Controller", num);
    }
}