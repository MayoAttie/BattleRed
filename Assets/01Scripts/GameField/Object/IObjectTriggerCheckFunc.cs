using UnityEngine;
public interface IObjectTriggerCheckFunc 
{
    // 상호작용을 위해, 플레이어가 특정 객체에 충돌 시 호출되는 가상 메서드
    void EnterTriggerFunctionInit(ObjectTriggerEnterCheck other);
}
