﻿using System.Collections;
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

    // 적 탐지 알림
    public void NotifyGetEnemyFind(List<Transform> findList)
    {
        foreach(Observer tmp in _observers)
        {
            tmp.GetEnemyFindNotify(findList);
        }
    }

}
