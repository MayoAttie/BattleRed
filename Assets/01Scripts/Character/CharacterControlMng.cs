using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TouchPadController;

public class CharacterControlMng : Subject, Observer
{
    #region 변수
    [SerializeField] CharacterController controller;      // 캐릭터 컨트롤러
    [SerializeField] Transform groundCheck;               // 지면 체크를 위한 위치 정보를 저장하는 변수
    [SerializeField] LayerMask groundMask;                // 지면을 나타내는 레이어 정보를 저장하는 변수


    Vector3 velocity;
    bool isGrounded;                            // 지면인지 체크
    bool isJump;                                // 점프중인지 체크
    bool isBlinking;                            // 회피중인지 체크
    public bool isBattle;                              // 전투중인지 체크
    bool isBlinkCoolTimeFleg;                   // 쿨타임 코루틴함수 제어 플래그
    [SerializeField]bool isBlinkStart;                          // 블링크 애니메이션이 시작되었는지 체크
    bool isConsecutiveBlink;                    //

    float jumpHeight = 2f;                      // 점프 높이
    float groundDistance = 1.4f;                // 지면과의 거리
    public float zPos;                          // 제어용 좌표 값
    public float xPos;                          // 제어용 좌표 값
    public float runX;                          // 달리기 제어용 변수
    public float runZ;                          // 달리기 제어용 변수
    float rotationSpeed = 100f;                 // 캐릭터 회전 속도
    float gravity = -9.18f;                     // 중력
    float fBliknkCoolTime = 3.0f;               // 회피기 충전 주기
    
    [SerializeField]int nBlinkNumber = 2;                       // 회피기 숫자
    Coroutine blinkCoolTimeCoroutine;           // Coroutine 객체를 저장할 변수
    CharacterManager characMng;                 // 캐릭터 매니저 싱글턴
    CharacterAniEventFinder eventInputer;
    [SerializeField]e_BlinkPos blinkpos;
    #endregion


    #region 구조체
    public enum e_BlinkPos
    {
        None,
        Front,
        Back,
        Right,
        Left,
        Max
    }
    #endregion


    private void Awake()
    {
        isBlinkCoolTimeFleg = false;
        isJump       = false;
        isBlinking   = false;
        isBattle     = false;
        blinkpos = e_BlinkPos.None;
        eventInputer = gameObject.GetComponent<CharacterAniEventFinder>();
    }

    void Start()
    {
        eventInputer.Attach(this);
        characMng = CharacterManager.Instance;
    }

    // Update is called once per frame
    private void Update()
    {
        isBattle = characMng.getIsBattle();
        // 코루틴이 실행 중이지 않은 경우에만 코루틴을 시작.
        if (blinkCoolTimeCoroutine == null && nBlinkNumber<=0)
        {
            blinkCoolTimeCoroutine = StartCoroutine(BlinkCoolTimeReset());
        }

        GravityFunc();
        ControllerGetInputData();
        RotateCharacter();
        if(!isBlinking)
        {
            if(isBattle)
            {
                groundDistance = 6f;
                RunCharacterFunction();
            }
            else
            {
                groundDistance = 4f;
                MoveCharacterFunction();
            }
        }
        JumpCharacterFunction();

    }

    // 조이스틱 컨트롤러, xy좌표 값 Get 함수
    void ControllerGetInputData()
    {
        zPos = JoyStickController.Instance.GetVerticalValue();
        xPos = JoyStickController.Instance.GetHorizontalValue();

        //Debug.Log(nameof(zPos)+":" +zPos);
        //Debug.Log(nameof(xPos) + ":" + xPos);

        if(!isBlinking)
        {
            if (!isBattle)
                characMng.GetCharacterClass().SetState(CharacterClass.eCharactgerState.e_WALK);
            else
                characMng.GetCharacterClass().SetState(CharacterClass.eCharactgerState.e_RUN);

        }

    }

    //중력함수
    void GravityFunc()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // 지면에 닿아있을 때 y 속도를 초기화
        }

        // 중력 적용
        velocity.y += gravity * Time.deltaTime;
    }

    #region 걷기

    // 워크 애니메이션 함수
    void MoveCharacterFunction()
    {
        if (isBattle)
            return;

        // 애니메이션을 실행할 때 필요한 파라미터 설정
        characMng.AnimatorFloatValueSetter(zPos, xPos);

        GravityFunc();

        // 객체 이동
        Vector3 move = transform.right * xPos + transform.forward * zPos;
        controller.Move((move * 5 + velocity) * Time.deltaTime); // 중력이 적용된 이동

        // x,y 값이 0에 가까우면, 이동을 멈추고 iDle상태로 바꿈
        if (Mathf.Approximately(zPos, 0f) && Mathf.Approximately(xPos, 0f))
        {
            characMng.GetCharacterClass().SetState(CharacterClass.eCharactgerState.e_Idle);
        }
    }
    #endregion

    #region 점프

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
        Debug.Log(nameof(isGrounded)+":"+isGrounded);
        if (isGrounded && isJump)
        {
            gameObject.GetComponent<CharacterAttackMng>().FlagValueReset();
            characMng.GetCharacterClass().SetState(CharacterClass.eCharactgerState.e_JUMP);
            isJump = false;
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
    #endregion

    #region 회전

    // 캐릭터 회전 함수
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
    #endregion

    #region 달리기

    //달리기 애니메이션 함수
    private void RunCharacterFunction()
    {

        characMng.AnimatorFloatValueSetter(zPos, xPos);
        GravityFunc();

        if (Mathf.Abs(xPos - 1f) < 0.1f)
        {
            // 오른쪽으로 방향 전환 (90도 회전)
            Quaternion targetRotation = Quaternion.Euler(0f, transform.eulerAngles.y + 90f, 0f);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else if (Mathf.Abs(xPos + 1f) < 0.1f)
        {
            // 왼쪽으로 방향 전환 (90도 회전)
            Quaternion targetRotation = Quaternion.Euler(0f, transform.eulerAngles.y - 90f, 0f);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else if (Mathf.Abs(zPos + 1f) < 0.06f)
        {
            // 이동 멈추기
            xPos = 0f;
            zPos = 0f;

            var instance = gameObject.GetComponent<CharacterAttackMng>();
            instance.ShildAct();
            characMng.GetCharacterClass().SetState(CharacterClass.eCharactgerState.e_ATTACK);
        }



        // 객체 이동
        Vector3 move = transform.right * xPos + transform.forward * zPos;
        controller.Move((move * 12 + velocity) * Time.deltaTime); // 중력이 적용된 이동


        // x,y 값이 0에 가까우면, 이동을 멈추고 ATTACK 상태로 바꿈
        if (Mathf.Approximately(zPos, 0f) && Mathf.Approximately(xPos, 0f))
        {
            // 대기 모드 전환을 위한 함수 호출
            var instance = gameObject.GetComponent<CharacterAttackMng>();
            instance.OffBattleMode();
            characMng.GetCharacterClass().SetState(CharacterClass.eCharactgerState.e_ATTACK);
        }
    }
    #endregion

    #region 회피기

    private void ActTumblin()
    {
        if (isBlinkStart)
            return;


        // 앞점멸
        if (Mathf.Abs(zPos - 1f) < 0.05f)
        {
            blinkpos = e_BlinkPos.Front;
            // zPos 방향으로 1만큼 이동
            Vector3 moveDirection = transform.forward * 1f;
            controller.Move(moveDirection);
        }
        // 뒷점멸
        else if (Mathf.Abs(zPos + 1f) < 0.05f)
        {
            blinkpos = e_BlinkPos.Back;
            // zPos 방향으로 -1만큼 이동
            Vector3 moveDirection = -transform.forward * 1f;
            controller.Move(moveDirection);
        }
        // 우점멸
        else if (Mathf.Abs(xPos - 1f) < 0.05f)
        {
            blinkpos = e_BlinkPos.Right;
            // xPos 방향으로 1만큼 이동
            Vector3 moveDirection = transform.right * 1f;
            controller.Move(moveDirection);
        }
        // 좌점멸
        else if (Mathf.Abs(xPos + 1f) < 0.05f)
        {
            blinkpos = e_BlinkPos.Left;
            // xPos 방향으로 -1만큼 이동
            Vector3 moveDirection = -transform.right * 1f;
            controller.Move(moveDirection);
        }
        // 디폴트는 앞점멸
        else
        {
            blinkpos = e_BlinkPos.Front;
            // zPos 방향으로 1만큼 이동
            Vector3 moveDirection = transform.forward * 1f;
            controller.Move(moveDirection);
        }
        NotifyBlinkValue(blinkpos);
        characMng.GetCharacterClass().SetState(CharacterClass.eCharactgerState.e_AVOID);
        // 옵저버에게 블링크 값 넘기기
    }

    // 블링크 쿨타임 초기화
    IEnumerator BlinkCoolTimeReset()
    {
        if (!isBlinkCoolTimeFleg)
        {
            isBlinkCoolTimeFleg = true;
            yield return new WaitForSeconds(fBliknkCoolTime);
            nBlinkNumber = 2;

            // 코루틴이 끝났으므로, Coroutine 변수를 null로 초기.
            blinkCoolTimeCoroutine = null;
            isBlinkCoolTimeFleg = false;
        }
    }

    // 블링크 버튼 클릭 이벤트 함수
    public void BlinkClickEvent()
    {
        if (isBlinkStart)
            return;
        if (nBlinkNumber > 0 && !isBlinking)
        {
            isBlinking = true;
            nBlinkNumber--;
            ActTumblin();
        }
    }

    #endregion



    #region 옵저버 패턴

    public void AttackEventNotify(int num){}

    public void AttackEventStartNotify(){}

    public void AtkLevelNotify(CharacterAttackMng.e_AttackLevel level){}

    public void BlinkValueNotify(e_BlinkPos value){}

    public void GetBlinkEndNotify() // 블링크 종료됨을 Get하여, 반영
    {
        Debug.Log(nameof(GetBlinkEndNotify));
        blinkpos = e_BlinkPos.None;
        isBlinking = false;
        isBlinkStart = false;

    }

    public void GetBlinkStartNotify()
    {
        isBlinkStart = true;    
    }

    public void GetBrockEndNotify(){}

    public void GetEnemyFindNotify(List<Transform> findList)
    {
    }

    #endregion

}
