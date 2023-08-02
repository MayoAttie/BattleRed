using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttack : MonoBehaviour
{
    Monster monsterObj;
    Animator animator;
    List<Transform> TargetList;
    private void Awake()
    {
    }
    void Start()
    {
    }

    void Update()
    {
        
    }

    public MonsterAttack(Monster monsterObj, Animator animator)
    {
        this.animator = animator;
        this.monsterObj = monsterObj;
    }

    public MonsterAttack(){ }


    #region 세터게터
    public Monster GetMonsterCls()
    {
        return monsterObj;
    }
    public void SetMonsetrCls(Monster monster)
    {
        monsterObj = monster;
    }

    public Animator GetAnimator()
    {
        return animator;
    }
    public void SetAnimator(Animator animator)
    {
        this.animator = animator;
    }


    #endregion

    public virtual void MonsterAttackStart()    // 공격 시작함수
    {
        Debug.Log("monsterAttack parents Func");
    }

    public void TargetInputter(List<Transform> targets)
    {
        TargetList = targets;
    }


}
