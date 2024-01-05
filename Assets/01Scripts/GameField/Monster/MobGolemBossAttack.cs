using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class MobGolemBossAttack : MonsterAttack
{
    int atkMaxLevel = 2;
    public int currnetAtkLevel = -1;
    Animator animator;

    MonsterAtkColliderMng[] floorings;

    float skillCool = 12f;
    float elapsedSkillCool = 0f;

    bool normalAtkFlag = false;
    bool skillAtkFlag = false;


    void Start()
    {
        DataInit();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        elapsedSkillCool = 0f;
        currnetAtkLevel = -1;
        isAtkAnimationConrolFlag = false;
        normalAtkFlag = false;
        skillAtkFlag = false;
        if(floorings!=null)
        {
            foreach (var i in floorings)
            {
                i.gameObject.SetActive(false);                   // 장판 객체 비활성화
            }
        }
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        currnetAtkLevel = -1;
    }

    private void Update()
    {
        base.Update();

        AttackProcessAI();
        EndPageChaseCheck();
        _MonsterMng._Rigidbody.velocity = Vector3.zero;
        _MonsterMng._Rigidbody.angularVelocity = Vector3.zero;
    }


    public override void MonsterAttackStart()   // 공격시작함수
    {
        base.MonsterAttackStart();

    }

    void DataInit()
    {
        animator = gameObject.GetComponent<Animator>();

        // MonsterSkillCollider 태그를 가진 자식 객체들을 찾아서 배열에 저장
        floorings = gameObject.transform.GetComponentsInChildren<Transform>()
            .Where(child => child.CompareTag("MonsterSkillCollider"))
            .Select(child => child.gameObject.GetComponent<MonsterAtkColliderMng>())
            .ToArray();

        // 장판은 우선 SetFalse
        foreach (var tmp in floorings)
        {
            tmp.gameObject.SetActive(false);
            tmp.ATK_offset = 0.75f;
        }
    }

    void AttackProcessAI()
    {
        if (GetTargetInRange() == false)
        {
            ResetDatas();
            return;
        }

        if (!isAtkAnimationConrolFlag)  // 코루틴 함수 중복 호출 방지
        {
            isAtkAnimationConrolFlag = true;
            StartCoroutine(AttackAI());
        }
    }

    IEnumerator AttackAI()
    {
        while(GetTargetInRange() == true && IsChageReturn == false)
        {
            if (elapsedSkillCool > skillCool)
            {
                if(!skillAtkFlag && !normalAtkFlag && !IsChageReturn)
                {
                    skillAtkFlag = true;
                    IsChageReturn = true;
                    SetAttackLevel(e_MonsterAttackLevel._4th);          // 대기 동작
                    yield return new WaitForSeconds(2.2f);

                    StartCoroutine(SkillStart());                       // 스킬 시전

                }
            }
            else
            {
                if(!skillAtkFlag && !normalAtkFlag)  // 반복 재생을 방지하기 위한, 제어 플래그.
                {
                    normalAtkFlag = true;
                    currnetAtkLevel++;
                    currnetAtkLevel %= atkMaxLevel;
                    switch (currnetAtkLevel)
                    {
                        case 0:
                            SetAttackLevel(e_MonsterAttackLevel._1st);
                            break;
                        case 1:
                            SetAttackLevel(e_MonsterAttackLevel._2rd);
                            break;
                    }

                    // 일반 공격 코루틴 시작
                    StartCoroutine(Attack_Normal());

                }
            }
            elapsedSkillCool += Time.deltaTime;
            yield return null;
        }
        ResetDatas();
        GetAtkColliderBox().gameObject.SetActive(false);
    }

    // 애니메이션 크기 동안 대기하는 코루틴 함수
    IEnumerator Attack_Normal()
    {

        GetAtkColliderBox().gameObject.SetActive(true);
        // 현재 재생 중인 애니메이션 클립의 이름 가져오기
        string clipName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;

        // 애니메이션 클립의 재생시간 가져오기
        float animationTime = GetAnimationTime(clipName);

        // 애니메이션 재생시간까지 대기
        float elapsedTime = 0f;
        while (elapsedTime < animationTime)
        {
            if (GetTargetInRange() == false)
            {
                ResetDatas();
                GetAtkColliderBox().gameObject.SetActive(false);
                yield break;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        GetAtkColliderBox().gameObject.SetActive(false);
        normalAtkFlag = false;

    }

    // 애니메이션 클립의 재생시간을 가져오는 함수
    float GetAnimationTime(string clipName)
    {
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        foreach (AnimationClip clip in ac.animationClips)
        {
            if (clip.name == clipName)
            {
                return clip.length;
            }
        }
        return 0;
    }

    // 스킬 공격 코루틴
    IEnumerator SkillStart()
    {
        SetAttackLevel(e_MonsterAttackLevel._3th);
        foreach(var i in floorings)
        {
            i.gameObject.SetActive(true);                   // 장판 객체 활성화
            i.MyCollider.enabled = false;                   // 콜라이더는 비활성화
        }

        // 현재 재생 중인 애니메이션 클립의 이름 가져오기
        string clipName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;

        // 애니메이션 클립의 재생시간 가져오기
        float animationTime = GetAnimationTime(clipName);

        // 애니메이션 재생시간의 80%까지 대기
        float elapsedTime = 0f;
        float waitTime1 = animationTime * 0.7f;
        float waitTime2 = animationTime * 0.3f;

        while (elapsedTime < waitTime1)
        {

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        foreach (var i in floorings)
        {
            i.MyCollider.enabled = true;
            EffectManager.Instance.EffectCreate(i.transform, 3,new Vector3 (0,1,0),2);  // 이펙트
        }

        // 나머지 시간 대기
        elapsedTime = 0f;
        while (elapsedTime < waitTime2)
        {

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        foreach (var i in floorings)
        {
            i.gameObject.SetActive(false);                   // 장판 객체 비활성화
        }

        // 플래그 변수 off
        IsChageReturn = false;
        elapsedSkillCool = 0;
        skillAtkFlag = false;
        yield break;
    }


    void ResetDatas()
    {
        isAtkAnimationConrolFlag = false;
        skillAtkFlag = false;
        normalAtkFlag = false;

    }
}
