using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAniEventFinder : Subject
{
    public void GetAttackEvent(int num)
    {
        NotifyAttackEvent(num);
    }

    public void GetAttackEventStart()
    {
        NotifyAttackEventStart();
    }

    public void GetBlinkEnd()
    {
        NotifyGetBlinkEnd();
    }

    public void GetBlinkStart()
    {
        NotifyGetBlinkStart();
    }


}
