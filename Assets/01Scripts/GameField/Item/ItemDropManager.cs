using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UI_UseToolClass;
public class ItemDropManager : Singleton<ItemDropManager>
{
    public Camera mainCamera;
    public Transform dropItemCrollView;
    public LayerMask itemLayer;
    public LayerMask groundLayer;
    public float pickupRange = 3f;

    private bool isCreate;

    private List<DropItem_UI> dropUI_list;
    int id_index;

    private void Awake()
    {
        dropUI_list = new List<DropItem_UI>();
    }

    private void FixedUpdate()
    {
        if (mainCamera == null)
            return;

        // 플레이어 아이템 추적
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward); // 플레이어 카메라의 위치와 정면 방향으로 ray 발사
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange, groundLayer)) // Ground 레이어와 충돌한 경우
        {
            Vector3 center = hit.point; // 충돌 지점을 중심으로 한 OverlapSphere 사용
            float radius = 2f; // 반지름

            Collider[] colliders = Physics.OverlapSphere(center, radius, itemLayer);
            if (colliders.Length > 0)
            {
                foreach(var collider in colliders)  
                {
                    isCreate = true;
                    var itemComponent = collider.GetComponent<DropItem>();
                    foreach (var tmp in dropUI_list)
                    {
                        // 동일한 id값을 가지는 UI 객체가 없을 경우에는 생성을 true
                        if(tmp.Id== itemComponent.Id)
                        {
                            isCreate = false;
                            break;
                        }
                    }
                    if(isCreate)    // ui 객체를 생성한다.
                    {
                        var uiObj = GameManager.Instance.DropItemUI_Pool.GetFromPool(Vector3.zero, Quaternion.identity, dropItemCrollView);
                        string tag = itemComponent.GetItemCls().GetTag();
                        if (tag == "무기")
                            WeaponKindDivider(itemComponent.GetItemCls() as WeaponAndEquipCls, uiObj.ImgSymbol);
                        else if(tag == "꽃" || tag == "모래" || tag == "성배" || tag == "깃털" || tag == "왕관")
                            EquipmentKindDivider(itemComponent.GetItemCls() as WeaponAndEquipCls, uiObj.ImgSymbol);
                        uiObj.Text.text = itemComponent.GetItemCls().GetName();
                        uiObj.Id = itemComponent.Id;
                        uiObj.Button.onClick.RemoveAllListeners();  // 기존의 버튼 리스너 해제
                        uiObj.ItemCls = itemComponent.GetItemCls();
                        // 버튼 리스너 연결
                        uiObj.Button.onClick.AddListener(() => DropItemGet(uiObj,itemComponent));
                        dropUI_list.Add(uiObj);
                    }
                }
            }
            else
            {
                GameManager.Instance.DropItemUI_Pool.AllReturnToPool();
                dropUI_list.Clear();
            }
        }
        else
        {
            GameManager.Instance.DropItemUI_Pool.AllReturnToPool();
            dropUI_list.Clear();
        }
    }


    public void ItemDrop(Transform _position, string index)
    {
        Vector3 dropPosition = GetGroundPosition(_position);
        switch (index)
        {
            case "무기":
                {
                    // 무기 아이템을 오브젝트 풀로 반환
                    var item = GameManager.Instance.DropItem_1Pool.GetFromPool(dropPosition, Quaternion.identity);
                    item.SetItemCls(RandomItem(index));
                    item.Id = id_index;
                    id_index++;
                }
                break;
            case "성유물":
                {
                    // 성유물 아이템을 오브젝트 풀로 불출
                    var item = GameManager.Instance.DropItem_2Pool.GetFromPool(dropPosition, Quaternion.identity);
                    item.SetItemCls(RandomItem(index));
                    item.Id = id_index;
                    id_index++;
                }
                break;

        }
    }
    public ItemClass RandomItem(string index)
    {
        ItemClass data = new ItemClass();
        switch(index)
        {
            case "무기":
                {
                    List<WeaponAndEquipCls> weaponAndEquipmentDataList = GameManager.Instance.GetWeaponAndEquipmentDataList();

                    // "무기" 태그를 가진 아이템 중에서 랜덤하게 하나 선택
                    List<WeaponAndEquipCls> weapons = weaponAndEquipmentDataList
                        .Where(item => item.GetTag() == "무기")
                        .ToList();

                    if (weapons.Count > 0)
                    {
                        int randomIndex = Random.Range(0, weapons.Count);

                        // data를 새로운 인스턴스로 초기화하고, 선택한 무기의 정보를 복사함
                        data = new WeaponAndEquipCls();
                        data.CopyFrom(weapons[randomIndex]);
                    }
                }
                break;
            case "성유물":
                {
                    string[] tags = { "꽃", "성배", "깃털", "왕관", "모래" };
                    List<WeaponAndEquipCls> weaponAndEquipmentDataList = GameManager.Instance.GetWeaponAndEquipmentDataList();

                    List<WeaponAndEquipCls> artifacts = new List<WeaponAndEquipCls>();

                    foreach (string tag in tags)
                    {
                        List<WeaponAndEquipCls> itemsWithTag = weaponAndEquipmentDataList
                            .Where(item => item.GetTag() == tag)
                            .ToList();

                        artifacts.AddRange(itemsWithTag);
                    }

                    if (artifacts.Count > 0)
                    {
                        int randomIndex = Random.Range(0, artifacts.Count);

                        // data를 새로운 인스턴스로 초기화하고, 선택한 아이템의 정보를 복사함
                        data = new WeaponAndEquipCls();
                        data.CopyFrom(artifacts[randomIndex]);

                        GameManager.Instance.EquipStatusRandomSelector(data as WeaponAndEquipCls);
                        GameManager.Instance.EquipItemStatusSet(data);
                        GameManager.Instance.Item_Id_Generator(data);
                    }
                }
                break;
        }
        return data;
    }

    // 지면에 해당하는 좌표값을 리턴
    private Vector3 GetGroundPosition(Transform _position)
    {
        // 무기 아이템을 뿌릴 때, y 좌표를 Ground 레이어 마스크 위에 설정
        LayerMask groundLayerMask = LayerMask.GetMask("Ground"); // Ground 레이어 마스크
        float raycastDistance = 100f; // 적절한 거리 설정
        Vector3 dropPosition = _position.position;

        RaycastHit hit;
        if (Physics.Raycast(dropPosition, Vector3.down, out hit, raycastDistance, groundLayerMask))
        {
            dropPosition.y = hit.point.y; // Ground 레이어에 닿는 지점으로 y 좌표 설정
        }

        return dropPosition;
    }

    private void DropItemGet(DropItem_UI itemUi, DropItem dropItem)
    {
        string tag = itemUi.ItemCls.GetTag();
        // 무기를 주웠을 경우
        if (tag.Equals("무기"))
        {
            // 무기 추가
            var userHadWeapons = GameManager.Instance.GetUserClass().GetHadWeaponList();
            userHadWeapons.Add(itemUi.ItemCls);

            // 획득한 데이터 제거 및 객체 리턴
            dropUI_list.Remove(itemUi);
            GameManager.Instance.DropItem_1Pool.ReturnToPool(dropItem);
            GameManager.Instance.DropItemUI_Pool.ReturnToPool(itemUi);
        }
        // 성유물을 주웠을 경우
        else if(tag == "성배" || tag == "꽃" || tag == "깃털" || tag == "모래" || tag == "왕관")
        {
            var userHadEquips = GameManager.Instance.GetUserClass().GetHadEquipmentList();
            userHadEquips.Add(itemUi.ItemCls);

            dropUI_list.Remove(itemUi);
            GameManager.Instance.DropItem_2Pool.ReturnToPool(dropItem);
            GameManager.Instance.DropItemUI_Pool.ReturnToPool(itemUi);
        }
        else
        {

        }
        
    }
        
   public void SetCamera()
    {
        // MainCamera 태그를 가진 객체를 찾아서 mainCamera에 할당
        GameObject mainCameraObject = GameObject.FindGameObjectWithTag("MainCamera");

        if (mainCameraObject != null)
        {
            mainCamera = mainCameraObject.GetComponent<Camera>();

            if (mainCamera == null)
            {
                Debug.LogError("MainCamera 태그를 가진 객체에 Camera 컴포넌트가 없습니다.");
            }
        }
        else
        {
            Debug.LogError("MainCamera 태그를 가진 객체를 찾을 수 없습니다.");
        }
    }
}
