using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantElement : CombatMediator
{
    Element myElement;
    float timeIndex = 0;
    float detectionRange = 3f;
    int idIndex;

    // 객체 생명주기
    IEnumerator LifeCycle()
    {
        detectionRange = 4f;
        while(timeIndex <30f)
        {
            timeIndex += Time.deltaTime;
            yield return null;

            Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange, LayerMask.GetMask("Player"));

            foreach (Collider collider in colliders)
            {
                if(collider != null)
                {
                    Debug.Log("____GetActive =="+ CharacterManager.Instance.GetCharacterClass().GetCurrnetElement().GetIsActive());
                    if (CharacterManager.Instance.GetCharacterClass().GetCurrnetElement().GetIsActive()== true)
                        ElemnetInterective(CharacterManager.Instance);
                }
            }
        }


        Destroy(gameObject);
    }

    void ElemnetInterective(CharacterManager characMng)
    {
        detectionRange = 4f;
        bool isActive = false;
        
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange, LayerMask.GetMask("Monster"));

        foreach (Collider collider in colliders)
        {
            if (collider == null)
                Debug.Log("몬스터 널");
            else
                Debug.Log("몬스터 널 X" + collider.gameObject.name);

            var mobMng = collider.GetComponent<MonsterManager>();
            bool checker = Mediator_PlantObj(characMng.GetCharacterClass(), mobMng);

            if (!isActive) isActive = checker;
        }


        if (isActive == false)
            return;

        Destroy(gameObject);
    }

    public PlantElement(){}

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
