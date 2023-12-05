using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SwordFunction : CombatMediator
{
    CharacterClass character;
    // 칼 객체에 부착된 콜라이더 트리거 클래스
    private void OnTriggerEnter(Collider other)
    {
        
        // 충돌한 객체의 레이어를 확인하여 몬스터 레이어인지 체크
        if (other.gameObject.layer == LayerMask.NameToLayer("Monster"))
        {
            character = CharacterManager.Instance.GetCharacterClass();
            var characeterMng = CharacterManager.Instance;
            CharacterAttackMng.e_AttackLevel attackLevel = characeterMng.GetCharacterAtkLevel();

            // 평타 공격인지 체크
            if (attackLevel == CharacterAttackMng.e_AttackLevel.Attack1 ||
               attackLevel==CharacterAttackMng.e_AttackLevel.Attack2 ||
               attackLevel == CharacterAttackMng.e_AttackLevel.Attack3)
            {
                var mob = other.gameObject.GetComponent<MonsterManager>();
                Mediator_CharacterAttack(character, characeterMng, mob);                                   // 상속받은 중재자 패턴의 데미지 로직 함수 호출
            }
            else if(attackLevel == CharacterAttackMng.e_AttackLevel.AtkSkill)   // 스킬 공격 판정
            {
                character.GetCurrnetElement().SetIsActive(true);
                var mob = other.gameObject.GetComponent<MonsterManager>();
                Mediator_CharacterSkillAttack(character, characeterMng, mob);   // 상속받은 중재자 패턴의 데미지 로직 함수 호출
            }
            gameObject.SetActive(false);
        }
    }

}
