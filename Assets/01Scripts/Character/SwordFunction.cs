using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordFunction : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // 충돌한 객체의 레이어를 확인하여 몬스터 레이어인지 체크
        if (other.gameObject.layer == LayerMask.NameToLayer("Monster"))
        {
            // 몬스터와 충돌한 경우의 처리를 여기에 작성
            Debug.Log("Sword collided with a monster.");
        }
    }
}
