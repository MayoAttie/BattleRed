using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UI_UseToolClass;
using static UseTool;
public class GetResourcePrintUI : MonoBehaviour
{
    [SerializeField] Transform buttonSet_transform;
    [SerializeField] GameObject textLongButton;
    Button outButton;   // 객체 종료 버튼
    Image itemImage;
    TextMeshProUGUI item_text;
    ObjectPool<TextLongButton> longbuttonsPool;     // 스크롤뷰 콘텐트 객체 오브젝트풀
    List<TextLongButton> activedLongButtonList;     // 활성화된 버튼 객체
    private void Awake()
    {
        outButton = transform.GetChild(0).GetComponent<Button>();
        itemImage = transform.GetChild(1).GetChild(0).GetComponent<Image>();
        item_text = transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();
        outButton.onClick.AddListener(ObjectOutButtonEvent);
        activedLongButtonList = new List<TextLongButton>();
        longbuttonsPool = new ObjectPool<TextLongButton>(textLongButton, 3, buttonSet_transform);
    }

    void Start()
    {
        gameObject.SetActive(false);
    }

    public void Scroll_Start(ItemClass findTarget)
    {
        gameObject.SetActive(true);
        foreach (var tmp in activedLongButtonList)
        {
            longbuttonsPool.ReturnToPool(tmp);
        }
        activedLongButtonList.Clear();

        // 아이템 이름
        item_text.text = findTarget.GetName();

        // 아이템 이미지
        if (findTarget.GetTag() == "육성 아이템")
        {
            itemImage.sprite = WeaponAndEquipLimitBreak_UI_Dvider(findTarget);
        }
        else if(findTarget.GetTag() == "광물")
        {
            GemKindDivider(findTarget, itemImage);
        }
        else if(findTarget.GetTag() == "무기")
        {
            WeaponKindDivider(findTarget as WeaponAndEquipCls, itemImage);
        }
        else if (IsTagEquips(findTarget))
        {
            EquipmentKindDivider(findTarget as WeaponAndEquipCls, itemImage);
        }


        //아이템 획득처 세팅
        GetItemWithAttributes(findTarget);

    }

    #region 합성대 연결
    // 합성 가능함을 알림.
    void SynthesisTransformFind()
    {
        TextLongButton obj = longbuttonsPool.GetFromPool(Vector3.zero, Quaternion.identity);
        activedLongButtonList.Add(obj);
        obj.transform.localScale = Vector3.one;
        obj.MySymbolImage = ItemSpritesSaver.Instance.SpritesSet[4];                                // 합성 마크 스프라이트 초기화
        obj.MyText = "필드 위에 존재하는 합성대에서 합성 가능";
        
        obj.MyButton.onClick.RemoveAllListeners();      // 버튼 이벤트 리스너 초기화
        obj.MyButton.onClick.AddListener(SynthesisTransformFindButtonListener);

    }
    void SynthesisTransformFindButtonListener()
    {
        // 현재 씬이 필드일 경우에만,
        if(IsCurrentSceneNameCorrect("GameField"))
        {
            // 객체의 주소를 파인드할 수 있도록 세팅
            CharacterManager.Instance._PathFinder.FindPathStart(ObjectManager.Instance.objectArray[0].transform.position);
        }
        ObjectOutButtonEvent();
    }
    #endregion

    #region 던전오브젝트 연결
    void ToDungeonTransformFind(int index)
    {
        TextLongButton obj = longbuttonsPool.GetFromPool(Vector3.zero, Quaternion.identity);
        activedLongButtonList.Add(obj);
        obj.transform.localScale = Vector3.one;
        obj.MySymbolImage = ItemSpritesSaver.Instance.SpritesSet[5];                                // 보물상자 마크 스프라이트 초기화
        obj.MyText = "던전에 진입하여, 아이템 획득 가능";
        
            obj.MyButton.onClick.RemoveAllListeners();      // 버튼 이벤트 리스너 초기화
        obj.MyButton.onClick.AddListener(() => ToDungeonTransformFindButtonListener(index));
    }

    void ToDungeonTransformFindButtonListener(int index)
    {
        if (IsCurrentSceneNameCorrect("GameField"))
        {
            // 던전의 주소를 파인드할 수 있도록 세팅
            CharacterManager.Instance._PathFinder.FindPathStart(ObjectManager.Instance.objectArray[1].transform.position);
        }
        else if(IsCurrentSceneNameCorrect("Dungeon_1"))
        {
            // 각 인덱스에 해당하는 던전의 보물상자를 추적할 수 있도록 함
            if (index == 1)
            {   
                CharacterManager.Instance._PathFinder.FindPathStart(ObjectManager.Instance.objectArray[1].transform.position);

            }
            else if(index == 2)
            {
                CharacterManager.Instance._PathFinder.FindPathStart(ObjectManager.Instance.objectArray[2].transform.position);

            }
        }
        ObjectOutButtonEvent();
    }
    #endregion

    #region 월드 좌표 연결
    void ToEquipAndWeapon(int index)
    {
        TextLongButton obj = longbuttonsPool.GetFromPool(Vector3.zero, Quaternion.identity);
        activedLongButtonList.Add(obj);
        obj.transform.localScale = Vector3.one;
        if(index>=1 && index<4)
            obj.MySymbolImage = ItemSpritesSaver.Instance.SpritesSet[6];                                // 선인장 스프라이트 초기화
        else if(index>=3 && index <6)
            obj.MySymbolImage = ItemSpritesSaver.Instance.SpritesSet[7];                                // 버섯 스프라이트 초기화

        obj.MyText = "필드 몬스터를 사냥하여, 아이템 획득 가능";

        obj.MyButton.onClick.RemoveAllListeners();      // 버튼 이벤트 리스너 초기화
        obj.MyButton.onClick.AddListener(() => EquipAndWeapon_GameField_GetPositionPath(index));
    }
    void EquipAndWeapon_GameField_GetPositionPath(int index)
    {
        if (IsCurrentSceneNameCorrect("GameField") == false)
        {

        }
        else    // 게임 필드일 경우,
        {
            switch(index)
            {
                case 1:
                    {
                        Vector3 pos = GameManager.Instance.List_SpawnPoint[0];
                        CharacterManager.Instance._PathFinder.FindPathStart(pos);
                    }
                    break;
                case 2:
                    {
                        Vector3 pos = GameManager.Instance.List_SpawnPoint[1];
                        CharacterManager.Instance._PathFinder.FindPathStart(pos);
                    }
                    break;
                case 3:
                    {
                        Vector3 pos = GameManager.Instance.List_SpawnPoint[2];
                        CharacterManager.Instance._PathFinder.FindPathStart(pos);
                    }
                    break;
                case 4:
                    {
                        Vector3 pos = GameManager.Instance.List_SpawnPoint[3];
                        CharacterManager.Instance._PathFinder.FindPathStart(pos);
                    }
                    break;
                case 5:
                    {
                        Vector3 pos = GameManager.Instance.List_SpawnPoint[4];
                        CharacterManager.Instance._PathFinder.FindPathStart(pos);
                    }
                    break;
            }
        }
        ObjectOutButtonEvent();
    }
    
    #endregion



    // 아이템 획득처 출력 함수
    void GetItemWithAttributes(ItemClass targetItem)
    {
        if (targetItem.GetTag() == "육성 아이템")
        {
            switch (targetItem.GetName())
            {
                case "칼바람 울프의 젖니":               // 1티어 *

                    break;
                case "칼바람 울프의 이빨":               // 2티어 **
                    SynthesisTransformFind();
                    break;
                case "칼바람 울프의 부서진 이빨":        // 3티어 ***
                    SynthesisTransformFind();
                    break;
                case "지맥의 낡은 가지":                 // 1티어 *
                    ToDungeonTransformFind(2);
                    break;
                case "지맥의 마른 잎":                   // 2티어 **
                    SynthesisTransformFind();
                    break;
                case "지맥의 새싹":                      // 3티어 ***
                    SynthesisTransformFind();
                    break;  
                case "슬라임 응축액":                    // 1티어 *
                    ToDungeonTransformFind(1);
                    break;
                case "슬라임청":                         // 2티어 **
                    SynthesisTransformFind();
                    break;
                case "슬라임 원액":                      // 3티어 ***
                    SynthesisTransformFind();
                    break;
                case "라이언 투사의 족쇄":               // 1티어 *

                    break;
                case "라이언 투사의 쇠사슬":             // 2티어 **
                    SynthesisTransformFind();
                    break;
                case "라이언 투사의 수갑":               // 3티어 ***
                    SynthesisTransformFind();
                    break;
                case "혼돈의 장치":                      // 1티어 *
                    ToDungeonTransformFind(1);
                    break;
                case "혼돈의 회로":                      // 2티어 **
                    SynthesisTransformFind();
                    break;
                case "혼돈의 노심":                      // 3티어 ***
                    SynthesisTransformFind();
                    break;
                case "이능 두루마리":                    // 1티어 *
                    ToDungeonTransformFind(1);
                    break;
                case "봉마의 두루마리":                  // 2티어 ** 
                    SynthesisTransformFind();
                    break;
                case "금주의 두루마리":                  // 3티어 ***
                    SynthesisTransformFind();
                    break;
            }
        }
        else if(targetItem.GetTag() == "무기")
        {
            switch(targetItem.GetName())
            {
                case "천공의 검":
                    ToEquipAndWeapon(1);
                    ToEquipAndWeapon(2);
                    ToEquipAndWeapon(3);
                    SynthesisTransformFind();
                    break;
                case "제례검":
                    ToEquipAndWeapon(1);
                    ToEquipAndWeapon(2);
                    ToEquipAndWeapon(3);
                    SynthesisTransformFind();
                    break;
                case "여명신검":
                    ToEquipAndWeapon(1);
                    ToEquipAndWeapon(2);
                    ToEquipAndWeapon(3);
                    break;
            }
        }
        else if(IsTagEquips(targetItem))    // 성유물일 경우
        {
            switch(targetItem.GetName())
            {
                case "이국의 술잔":
                    ToEquipAndWeapon(4);
                    ToEquipAndWeapon(5);
                    break;
                case "귀향의 깃털":
                    ToEquipAndWeapon(4);
                    ToEquipAndWeapon(5);
                    break;
                case "이별의 모자":
                    ToEquipAndWeapon(4);
                    ToEquipAndWeapon(5);
                    break;
                case "옛 벗의 마음":
                    ToEquipAndWeapon(4);
                    ToEquipAndWeapon(5);
                    break;
                case "빛을 좆는 돌":
                    ToEquipAndWeapon(4);
                    ToEquipAndWeapon(5);
                    break;
                case "전투광의 해골잔":
                    SynthesisTransformFind();
                    ToEquipAndWeapon(4);
                    ToEquipAndWeapon(5);
                    break;
                case "전투광의 깃털":
                    SynthesisTransformFind();
                    ToEquipAndWeapon(4);
                    ToEquipAndWeapon(5);
                    break;
                case "전투광의 귀면":
                    SynthesisTransformFind();
                    ToEquipAndWeapon(4);
                    ToEquipAndWeapon(5);
                    break;
                case "전투광의 장미":
                    SynthesisTransformFind();
                    ToEquipAndWeapon(4);
                    ToEquipAndWeapon(5);
                    break;
                case "전투광의 시계":
                    SynthesisTransformFind();
                    ToEquipAndWeapon(4);
                    ToEquipAndWeapon(5);
                    break;
                case "피에 물든 기사의 술잔":
                    SynthesisTransformFind();
                    ToEquipAndWeapon(4);
                    ToEquipAndWeapon(5);
                    break;
                case "피에 물든 검은 깃털":
                    SynthesisTransformFind();
                    ToEquipAndWeapon(4);
                    ToEquipAndWeapon(5);
                    break;
                case "피에 물든 철가면":
                    SynthesisTransformFind();
                    ToEquipAndWeapon(4);
                    ToEquipAndWeapon(5);
                    break;
                case "피에 물든 강철 심장":
                    SynthesisTransformFind();
                    ToEquipAndWeapon(4);
                    ToEquipAndWeapon(5);
                    break;
                case "피에 물든 기사의 시계":
                    SynthesisTransformFind();
                    ToEquipAndWeapon(4);
                    ToEquipAndWeapon(5);
                    break;
                default: break;
            }
        }
    }





    void ObjectOutButtonEvent()
    {
        foreach(var tmp in activedLongButtonList)
        {
            longbuttonsPool.ReturnToPool(tmp);
        }
        gameObject.SetActive(false);
    }
}
