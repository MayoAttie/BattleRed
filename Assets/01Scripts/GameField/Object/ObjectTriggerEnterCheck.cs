using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTriggerEnterCheck : MonoBehaviour
{
    IObjectTriggerCheckFunc parents;
    bool isActive;
    private void Awake()
    {
        isActive = false;
        parents = transform.parent.GetComponent<IObjectTriggerCheckFunc>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
            return;
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {

            if (!isActive)
            {
                isActive = true;
                // 부모 객체가 가진 인터페이스의 기능 함수를 호출.
                parents.EnterTriggerFunctionInit(this);
            }

        }
    }

    public bool IsActive
    {
        get { return isActive; }
        set { isActive = value; }
    }

}
