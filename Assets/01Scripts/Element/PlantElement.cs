using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using static HandlePauseTool;

public class PlantElement : CombatMediator
{
    Element myElement;
    float timeIndex = 0;
    float detectionRange = 15f;
    int idIndex;

    private void OnEnable()
    {
        // 게임매니저의 이벤트에 구독
        GameManager.OnPauseStateChanged += HandlePauseStateChanged;
    }

    private void OnDisable()
    {
        // 게임매니저의 이벤트 구독 해제
        GameManager.OnPauseStateChanged -= HandlePauseStateChanged;
    }

    // 객체 생명주기
    IEnumerator LifeCycle()
    {
        while(timeIndex <30f)
        {
            timeIndex += Time.deltaTime;
            yield return null;

            Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange, LayerMask.GetMask("Player"));

            foreach (Collider collider in colliders)
            {
                if(collider != null)
                {
                    if (CharacterManager.Instance.GetCharacterClass().GetCurrnetElement().GetIsActive()== true)
                        ElemnetInterective(CharacterManager.Instance);
                }
            }
        }


        QueueResettingAndDestroy();
    }

    void ElemnetInterective(CharacterManager characMng)
    {
        bool isActive = false;
        
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange, LayerMask.GetMask("Monster"));

        foreach (Collider collider in colliders)
        {

            var mobMng = collider.GetComponent<MonsterManager>();
            isActive = Mediator_PlantObj(characMng.GetCharacterClass(), mobMng);

            if (isActive)
                break;
        }


        if (isActive == false)
            return;



        QueueResettingAndDestroy();
    }

    public PlantElement(){}

    public void QueueResettingAndDestroy()
    {
        // 제거하려는 게임 오브젝트를 큐에서 제거
        Queue<GameObject> plantQue = Element_Interaction.Instance.plantQue;
        GameObject gameObjectToRemove = this.gameObject; // 현재 게임 오브젝트를 제거하려는 대상으로 설정

        Queue<GameObject> newQueue = new Queue<GameObject>();
        foreach (GameObject obj in plantQue)
        {
            if (obj != gameObjectToRemove)
            {
                newQueue.Enqueue(obj);
            }
        }
        Element_Interaction.Instance.plantQue = newQueue;
        Element_Interaction.plantElementNum--;
        // 게임 오브젝트 제거
        Destroy(gameObjectToRemove);
    }

    public void SetElement(Element element)
    { 
        myElement = element;
        StartCoroutine(LifeCycle());
    }
    public Element GetElement() { return myElement; }
    public float GetTimeIndex() { return timeIndex; }
    public int GetIdIndex() { return idIndex; }
    public void SetIdIndex(int idIndex) { this.idIndex = idIndex; }
    public float GetDetectionRange() { return detectionRange; }
}
