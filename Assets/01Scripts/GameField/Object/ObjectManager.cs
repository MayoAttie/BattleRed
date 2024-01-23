using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UI_Manager;
using static UseTool;
public class ObjectManager : Singleton<ObjectManager>
{
    /** 0번 - SynthesisObject
     * 
     */
    public List<InteractionObject> objectArray;
    //public DataPrintScreenScrollManager dpsScrollManager;
    //private List<bool> isOpenIng;
    [SerializeField]
    GameObject treasureBox;
    
    private Dictionary<InteractionObject, bool> isOpenChecker;

    private void Awake()
    {
        isOpenChecker = new Dictionary<InteractionObject, bool>();
        objectArray = new List<InteractionObject>();
        //isOpenIng = new List<bool>();
        ObjectFindSetter();
    }


    public void FunctionConnecter(InteractionObject obj)
    {
        // 합성대 오브젝트 함수 연결
        switch(obj.Name)
        {
            case "합성대":
                obj.ObjectClickEventSet().onClick.AddListener(() => SynthesisObjectFunction());
                break;
            case "던전1":
                obj.ObjectClickEventSet().onClick.AddListener(() => DungeonEntrence(1));
                break;
            case "던전중간통로":
                obj.ObjectClickEventSet().onClick.AddListener(() => DungeonMiddleDoor(obj));
                break;
            case "보물상자1":
                obj.ObjectClickEventSet().onClick.AddListener(() => TreasureBox_1(obj));
                break;
            case "보물상자2":
                obj.ObjectClickEventSet().onClick.AddListener(() => TreasureBox_2(obj));
                break;
            case "퍼즐 보물상자1":
                obj.ObjectClickEventSet().onClick.AddListener(() => Dynamic_TreasureBox_1(obj));
                break;
            case "원소비석_불":
            case "원소비석_물":
            case "원소비석_바람":
            case "원소비석_번개":
            case "원소비석_풀":
                ElementRockTrigger(obj);
                break;

        }
    }
    #region 합성대
    //합성대
    void SynthesisObjectFunction()
    {
        Synthesis synthesisInstance = UI_Manager.Instance.synthesis; // Synthesis 클래스의 인스턴스 생성
        synthesisInstance.SynthesisObjectFunc_UI_Print();           // Synthesis 클래스의 메서드 호출
    }

    #endregion

    #region 던전 연결
    //던전
    void DungeonEntrence(int number)
    {
        switch (number)
        {
            case 1:
                SceneLoadManager.Instance.SceneLoadder("Dungeon_1");
                break;
        }
    }
    #endregion

    #region 던전 중간통로
    // 던전 중간통로
    public void DungeonMiddleDoor(InteractionObject obj)
    {
        var key = GameManager.Instance.GetUserClass().GetHadEtcItemList().Find(item => item.GetName().Equals("열쇠"));
        if (key == null)
        {
            DataPrintScreenScrollManager.Instance.JustTextPrint("열쇠가 필요합니다!!");
            // 키가 없을 경우
            return;
        }

        isOpenChecker[obj] = true;

        Transform objectTransform = obj.transform; // objectArray[0]의 트랜스폼 가져오기

        // 캐릭터 이동 좌표 초기화
        Vector3 targetPosition = objectTransform.GetChild(5).position;

        CharacterManager.Instance.gameObject.transform.position = targetPosition;

        // 캐릭터를 오브젝트 방향으로 회전
        CharacterManager.Instance.gameObject.transform.LookAt(objectTransform);


        GameObject obj1 = objectTransform.GetChild(1).gameObject;
        GameObject obj2 = objectTransform.GetChild(2).gameObject;

        // EffectManager를 이용하여 이펙트 생성
        EffectManager.Instance.EffectCreate(obj1.transform, 2, new Vector3(1.2f, 2.4f, -1.2f));
        EffectManager.Instance.EffectCreate(obj2.transform, 2, new Vector3(-1.2f, 2.4f, -1.2f));

        StartCoroutine(FalseDoor(obj1, obj2));
    }
    IEnumerator FalseDoor(GameObject obj1, GameObject obj2)
    {
        yield return new WaitForSeconds(1.2f);
        obj1.SetActive(false);
        obj2.SetActive(false);
    }

    #endregion


    #region 보물상자
    IEnumerator AniPlayWaitAfterFuncStart(float length, List<ItemClass> item_list, GameObject box)
    {
        // 아이템 저장
        var userCls = GameManager.Instance.GetUserClass();
        foreach (var tmp in item_list)
        {
            if (tmp.GetTag() == "무기")
                userCls.GetHadWeaponList().Add(tmp);
            else if (IsTagEquips(tmp))
                userCls.GetHadEquipmentList().Add(tmp);
            else if (tmp.GetTag() == "광물")
            {
                var isHave = userCls.GetHadGemList().Find(item => item.GetName().Equals(tmp.GetName()));
                if (isHave == null)
                    userCls.GetHadGemList().Add(tmp);
                else
                    isHave.SetNumber(isHave.GetNumber() + tmp.GetNumber());
            }
            else if (tmp.GetTag() == "음식")
            {
                var isHave = userCls.GetHadFoodList().Find(item => item.GetName().Equals(tmp.GetName()));
                if (isHave == null)
                    userCls.GetHadFoodList().Add(tmp);
                else
                    isHave.SetNumber(isHave.GetNumber() + tmp.GetNumber());
            }
            else if (tmp.GetTag() == "육성 아이템")
            {
                var isHave = userCls.GetHadGrowMaterialList().Find(item => item.GetName().Equals(tmp.GetName()));
                if (isHave == null)
                    userCls.GetHadGrowMaterialList().Add(tmp);
                else
                    isHave.SetNumber(isHave.GetNumber() + tmp.GetNumber());
            }
            else if(tmp.GetTag() == "기타")
            {
                var isHave = userCls.GetHadEtcItemList().Find(item => item.GetName().Equals(tmp.GetName()));
                if (isHave == null)
                    userCls.GetHadEtcItemList().Add(tmp);
            }
        }

        EffectManager.Instance.EffectCreate(box.transform, 0,new Vector3(0,0.3f,0), 3);


        yield return new WaitForSeconds(length);
        Destroy(box);
        yield return new WaitForSeconds(0.5f);

        DataPrintScreenScrollManager.Instance.GetItemPrint(item_list);
    }
    // 보물상자1
    void TreasureBox_1(InteractionObject obj)
    {
        isOpenChecker[obj] = true;

        List<ItemClass> getItem_list = new List<ItemClass>();

        AddItemToList_CopyData("철광석", getItem_list);
        AddItemToList_CopyData("슬라임 응축액", getItem_list);
        AddItemToList_CopyData("혼돈의 장치", getItem_list);
        AddItemToList_CopyData("이능 두루마리", getItem_list);

        var ani = obj.GetComponent<Animator>();
        ani.SetBool("isOpen", true);
        float length = GetAnimationLength(ani, "treasure_chest_open");

        StartCoroutine(AniPlayWaitAfterFuncStart(length, getItem_list, obj.gameObject));
    }
    // 보물상자2
    void TreasureBox_2(InteractionObject obj)
    {
        isOpenChecker[obj] = true;

        List<ItemClass> getItem_list = new List<ItemClass>();

        AddItemToList_CopyData("철광석", getItem_list);
        getItem_list[0].SetNumber(5);
        AddItemToList_CopyData("무스프", getItem_list);
        AddItemToList_CopyData("지맥의 낡은 가지", getItem_list);

        var ani = obj.GetComponent<Animator>();
        ani.SetBool("isOpen", true);
        float length = GetAnimationLength(ani, "treasure_chest_open");

        StartCoroutine(AniPlayWaitAfterFuncStart(length, getItem_list, obj.gameObject));
    }
    #endregion


    #region 동적 보물상자

    void Dynamic_TreasureBox_1(InteractionObject obj)
    {
        IsOpenChecker[obj] = true;

        List<ItemClass> getItem_list = new List<ItemClass>();

        AddItemToList_CopyData("백철", getItem_list);
        AddItemToList_CopyData("지맥의 낡은 가지", getItem_list);
        AddItemToList_CopyData("이능 두루마리", getItem_list);
        AddItemToList_CopyData("열쇠", getItem_list);


        var ani = obj.GetComponent<Animator>();
        ani.SetBool("isOpen", true);
        float length = GetAnimationLength(ani, "treasure_chest_open");

        StartCoroutine(AniPlayWaitAfterFuncStart(length, getItem_list, obj.gameObject));
    }


    #endregion

    #region 원소비석

    void ElementRockTrigger(InteractionObject obj)
    {
        obj.isActive = true;
        Transform parents = obj.transform.parent;

        // 부모에 ElementRocksObject가 존재한다면 함수 호출
        ElementRocksObject parentsCls = parents.GetComponent<ElementRocksObject>();
        if(parentsCls != null)
            parentsCls.ElementRockOn(obj);

        // 자식 객체중에 파티클 시스템이 있다면, ActiveOn
        ParticleSystem effect = obj.transform.GetComponentInChildren<ParticleSystem>(true);
        if(effect != null)
            effect.gameObject.SetActive(true);
    }

    #endregion



    public void ObjectFindSetter()
    {
        objectArray.Clear();
        GameObject parents = GameObject.Find("Objects");
        if (parents == null)
            return;

        IsOpenChecker.Clear();
        InteractionObject[] childObjs = parents.GetComponentsInChildren<InteractionObject>();

        // isOpenIng 리스트를 childObjs 배열의 크기만큼 false로 초기화
        foreach (var i in childObjs)
        {
            IsOpenChecker[i] = false;
        }

        foreach (var tmp in childObjs)
            objectArray.Add(tmp);
    }


    public Dictionary<InteractionObject, bool> IsOpenChecker
    {
        get { return isOpenChecker; }
        set { isOpenChecker = value; }
    }
    public GameObject TreasureBox
    {
        get { return treasureBox; }
    }
}
