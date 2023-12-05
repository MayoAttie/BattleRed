using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class MobCactusAttack : MonsterAttack
{
    int atkMaxLevel = 2;
    public int currnetAtkLevel = -1;
    Animator animator;
    void Start()
    {
        isAtkAnimationConrolFlag = false;
        animator = gameObject.GetComponent<Animator>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        currnetAtkLevel = -1;
    }
    private void OnDisable()
    {
        base.OnDisable();
        currnetAtkLevel = -1;
    }

    void Update()
    {
        base.Update();

        AttackProcessAI();
        EndPageChaseCheck();

    }
    public MobCactusAttack(Monster monsterObj, NavMeshAgent navAgent) : base(monsterObj, navAgent) {}
    public MobCactusAttack(){ }


    public override void MonsterAttackStart()   // 공격시작함수
    {
        base.MonsterAttackStart();

    }


    void AttackProcessAI()
    {
        if (GetTargetInRange() == false)
            return;

        if (!isAtkAnimationConrolFlag)
        {
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

            // 애니메이션 크기 동안 대기하는 코루틴 시작
            StartCoroutine(WaitForAnimation());
        }
    }

    // 애니메이션 크기 동안 대기하는 코루틴 함수
    IEnumerator WaitForAnimation()
    {
        if(!isAtkAnimationConrolFlag)
        {
            GetAtkColliderBox().gameObject.SetActive(true);
            isAtkAnimationConrolFlag = true;
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
                    GetAtkColliderBox().gameObject.SetActive(false);
                    yield break;
                }
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            GetAtkColliderBox().gameObject.SetActive(false);
            isAtkAnimationConrolFlag = false;
        }
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








}
