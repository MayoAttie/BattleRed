using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointNotify : Subject
{
    public int num;
    private bool isPass = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if(!isPass)
            {
                isPass = true;
                NotifyCheckPoint_PlayerPass(num);
            }
        }
    }
}
