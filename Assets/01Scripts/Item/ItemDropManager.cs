using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemDropManager : Singleton<ItemDropManager>
{
    public void ItemDrop(Transform _position, string index)
    {
        Vector3 dropPosition = GetGroundPosition(_position);
        switch (index)
        {
            case "무기":
                {
                    // 무기 아이템을 오부잭투 풀로 반환
                    var item = GameManager.Instance.DropItem_1Pool.GetFromPool(dropPosition, Quaternion.identity);
                    item.SetItemCls(RandomItem(index));
                }
                break;
            case "성유물":
                {
                    // 성유물 아이템을 오브젝트 풀로 불출
                    var item = GameManager.Instance.DropItem_2Pool.GetFromPool(dropPosition, Quaternion.identity);
                    item.SetItemCls(RandomItem(index));
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

                    if (weapons.Count > 0)  // 무기 중에서 하나를 랜덤하게 꺼내어 저장
                    {
                        int randomIndex = Random.Range(0, weapons.Count);
                        data = weapons[randomIndex];
                    }
                }
                break;
            case "성유물":
                {
                    string[] tags = { "꽃", "성배", "깃털", "왕관", "모래" };
                    List<WeaponAndEquipCls> weaponAndEquipmentDataList = GameManager.Instance.GetWeaponAndEquipmentDataList();

                    List<WeaponAndEquipCls> artifacts = new List<WeaponAndEquipCls>();

                    // "성유물" 태그를 가진 아이템 중에서 해당 태그를 가진 아이템을 가져옴
                    foreach (string tag in tags)
                    {
                        List<WeaponAndEquipCls> itemsWithTag = weaponAndEquipmentDataList
                            .Where(item => item.GetTag() == tag)
                            .ToList();

                        artifacts.AddRange(itemsWithTag);
                    }

                    if (artifacts.Count > 0)  // "성유물" 중에서 하나를 랜덤하게 꺼내어 저장
                    {
                        int randomIndex = Random.Range(0, artifacts.Count);
                        data = artifacts[randomIndex];
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
}
