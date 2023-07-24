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

    public void NotifyAttackEvent(int num)
    {
        foreach(Observer tmp in _observers)
        {
            tmp.AttackEventNotify(num);
        }
    }

    public void NotifyAttackEventStart()
    {
        foreach(Observer tmp in _observers)
        {
            tmp.AttackEventStartNotify();
        }
    }

    public void NotifyAtkLevel(CharacterAttackMng.e_AttackLevel level)
    {
        foreach (Observer tmp in _observers)
        {
            tmp.AtkLevelNotify(level);
        }
    }

    public void NotifyBlinkValue(CharacterControlMng.e_BlinkPos value)
    {
        foreach (Observer tmp in _observers)
        {
            tmp.BlinkValueNotify(value);
        }
    }
    public void NotifyGetBlinkEnd()
    {
        foreach (Observer tmp in _observers)
        {
            tmp.GetBlinkEndNotify();
        }
    }

    public void NotifyGetBlinkStart()
    {
        foreach (Observer tmp in _observers)
        {
            tmp.GetBlinkStartNotify();
        }
    }
}
