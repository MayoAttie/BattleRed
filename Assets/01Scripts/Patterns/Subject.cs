using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Subject : MonoBehaviour
{
    private readonly ArrayList _observers = new ArrayList();
    public void Attach(Observer observer)
    {
        _observers.Add(observer);
    }

    public void Detach(Observer observer)
    {
        _observers.Remove(observer);
    }
    // 공격 애니메이션 엔드 단계, 해당 공격레벨 알림
    public void NotifyAttackEvent(int num)
    {
        foreach(Observer tmp in _observers)
        {
            tmp.AttackEventNotify(num);
        }
    }
    // 공격 애니메이션 시작 알림
    public void NotifyAttackEventStart()
    {
        foreach(Observer tmp in _observers)
        {
            tmp.AttackEventStartNotify();
        }
    }

    //캐릭터 매니저에 공격 단계 방송
    public void NotifyAtkLevel(CharacterAttackMng.e_AttackLevel level)
    {
        foreach (Observer tmp in _observers)
        {
            tmp.AtkLevelNotify(level);
        }
    }

    //캐릭터 매니저에 블링크 Dirction 방송
    public void NotifyBlinkValue(CharacterControlMng.e_BlinkPos value)
    {
        foreach (Observer tmp in _observers)
        {
            tmp.BlinkValueNotify(value);
        }
    }
    // 회피기 끝
    public void NotifyGetBlinkEnd()
    {
        foreach (Observer tmp in _observers)
        {
            tmp.GetBlinkEndNotify();
        }
    }
    // 회피기 시작
    public void NotifyGetBlinkStart()
    {
        foreach (Observer tmp in _observers)
        {
            tmp.GetBlinkStartNotify();
        }
    }

    // 쉴드 종료 알림
    public void NotifyGetBrockEnd()
    {
        foreach (Observer tmp in _observers)
        {
            tmp.GetBrockEndNotify();
        }
    }

    public void NotifyGetEnemyFind(List<Transform> findList)
    {
        foreach(Observer tmp in _observers)
        {
            tmp.GetEnemyFindNotify(findList);
        }
    }
}
