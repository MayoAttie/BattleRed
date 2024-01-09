using System.Collections.Generic;
using UnityEngine;
public interface Observer
{
    void AtkLevelNotify(CharacterAttackMng.e_AttackLevel level);
    void BlinkValueNotify(CharacterControlMng.e_BlinkPos value);
    void GetEnemyFindNotify(List<Transform> findList);
    void AttackSkillStartNotify();
    void AttackSkillEndNotify();
    void CheckPoint_PlayerPassNotify(int num);
    void WorldMapOpenNotify();
    void WorldMapCloseNotify();
    void ConvertToTargetStateNotify(List<Vector3> listTarget);
}

