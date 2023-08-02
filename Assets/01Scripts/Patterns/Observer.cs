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

    void GetBrockEndNotify();   // 방어 애니메이션 종료 알림

    void GetEnemyFindNotify(List<Transform> findList);
}

