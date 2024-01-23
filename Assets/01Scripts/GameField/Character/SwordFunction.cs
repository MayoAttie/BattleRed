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
        else if(other.gameObject.layer == LayerMask.NameToLayer("AttackInteractionObj"))
        {
            Transform parents = other.transform.parent;
            if (parents == null)
                return;

            // 상호작용 오브젝트의 부모가 퍼즐 오브젝트일 경우,
            PuzzleObject puzzleCls = parents.GetComponent<PuzzleObject>();
            if(puzzleCls != null)
            {
                puzzleCls.ObjectRotation(other.transform); // 퍼즐 회전
                gameObject.SetActive(false);
                return;
            }

            // 자기자신이 상호작용 객체일 경우.
            InteractionObject attackInteraction = other.GetComponent<InteractionObject>();
            if(attackInteraction != null)
            {
                string Checker = attackInteraction.Name;
                Debug.Log("Checker : " + Checker);

                if (Checker.Contains("원소비석_"))   // 상호작용 객체에 저장된 Name에 원소비석_가 존재할 경우.
                {
                    int index = Checker.IndexOf("_");
                    string afterUnderscore = Checker.Substring(index + 1);      // 글자 잘라서 확인
                    Debug.Log("afterUnderscore : " + afterUnderscore);
                    switch (afterUnderscore)    // 원소비석을 체크하여 올바른 속성이 아니라면 리턴
                    {
                        case "불":
                            if (CharacterManager.Instance.GetCharacterClass().GetCurrnetElement().GetElement() != Element.e_Element.Fire)
                                return;
                            break;
                        case "물":
                            if (CharacterManager.Instance.GetCharacterClass().GetCurrnetElement().GetElement() != Element.e_Element.Water)
                                return;
                            break;
                        case "바람":
                            if (CharacterManager.Instance.GetCharacterClass().GetCurrnetElement().GetElement() != Element.e_Element.Wind)
                                return;
                            break;
                        case "번개":
                            if (CharacterManager.Instance.GetCharacterClass().GetCurrnetElement().GetElement() != Element.e_Element.Lightning)
                                return;
                            break;
                        case "풀":
                            if (CharacterManager.Instance.GetCharacterClass().GetCurrnetElement().GetElement() != Element.e_Element.Plant)
                                return;
                            break;
                        default:
                            break;
                    }
                }
                ObjectManager.Instance.FunctionConnecter(attackInteraction);    // 오브젝트 매니저의 분류 함수에 연결.
            }
        }
    }

}
