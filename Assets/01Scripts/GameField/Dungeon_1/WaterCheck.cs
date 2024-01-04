using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterCheck : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // 워터에 캐릭터가 접촉한 경우, 게임 리셋
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            GameManager.Instance.CactusPool.AllReturnToPool();
            GameManager.Instance.MushroomAngryPool.AllReturnToPool();
            GameManager.Instance.MonsterHpBarPool.AllReturnToPool();
            GameManager.Instance.GolemBossPool.AllReturnToPool();

            int hp = GameManager.Instance.GetUserClass().GetUserCharacter().GetCurrentHp();
            int tmp = (int)(hp * 0.7f);
            GameManager.Instance.GetUserClass().GetUserCharacter().SetCurrentHp(tmp);
            SceneLoadManager.Instance.SceneLoadder("Dungeon_1");
        }
    }
}
