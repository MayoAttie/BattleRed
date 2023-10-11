using System.Collections.Generic;
using UnityEngine;
public interface Observer
{
    void AtkLevelNotify(CharacterAttackMng.e_AttackLevel level);
    void BlinkValueNotify(CharacterControlMng.e_BlinkPos value);
    void GetEnemyFindNotify(List<Transform> findList);
    void AttackSkillStartNotify();
    void AttackSkillEndNotify();
}

