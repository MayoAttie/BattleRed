using System.Collections.Generic;
using UnityEngine;
public interface Observer
{
    void AttackEventNotify(int num);
    void AttackEventStartNotify();

    void AtkLevelNotify(CharacterAttackMng.e_AttackLevel level);
    void BlinkValueNotify(CharacterControlMng.e_BlinkPos value);

    void GetBlinkEndNotify();

    void GetBlinkStartNotify();
}

