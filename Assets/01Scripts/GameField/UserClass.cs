using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserClass
{
    CharacterClass userCharacter;               // 유저 캐릭터
    ItemClass userEquippedWeapon;               // 장비한 무기
    ItemClass[] userEquippedEquipment;          // 장비한 성유물(꽃,깃털,모래,성배,왕관)

    List<ItemClass> hadWeaponList;              // 보유한 무기 리스트
    List<ItemClass> hadEquipmentList;           // 보유한 성유물 리스트
    List<ItemClass> hadGemList;                 // 보유한 광물 리스트
    List<ItemClass> hadFoodList;                // 보유한 음식 리스트
    List<ItemClass> hadGrowMaterialList;        // 보유한 육성 재화 리스트
    List<ItemClass> hadEtcItemList;

    DateTime userLastConnectTime;

    int nMora;
    int nStarLight;

    public UserClass() 
    {
        hadEtcItemList = new List<ItemClass>();
        userEquippedWeapon = new ItemClass();
        userEquippedEquipment = new ItemClass[5];
        nMora = 0;
        nStarLight = 0;
    }

    public void SetUserCharacter(CharacterClass userCharacter) { this.userCharacter = userCharacter; }
    public void SetUserEquippedWeapon(ItemClass userEquippedWeapon) { this.userEquippedWeapon = userEquippedWeapon; }
    public void SetUserEquippedEquipment(ItemClass[] userEquippedEquipment)
    {
        if (userEquippedEquipment.Length != 5)
        {
            // 입력 배열의 크기가 5가 아닌 경우, 크기가 5인 새 배열을 생성하고 데이터를 복사
            this.userEquippedEquipment = new ItemClass[5];
            foreach(var item in userEquippedEquipment)
            {
                switch(item.GetTag())
                {
                    case "꽃":
                        this.userEquippedEquipment[0] = item;
                        break;
                    case "깃털":
                        this.userEquippedEquipment[1] = item;
                        break;
                    case "모래":
                        this.userEquippedEquipment[2] = item;
                        break;
                    case "성배":
                        this.userEquippedEquipment[3] = item;
                        break;
                    case "왕관":
                        this.userEquippedEquipment[4] = item;
                        break;
                }
            }
        }
        else
        {
            // 입력 배열의 크기가 이미 5인 경우 그대로 할당
            this.userEquippedEquipment = userEquippedEquipment;
        }
    }
    public void SetUserEquippedEquipment(ItemClass userEquippedEquipment, int index) { this.userEquippedEquipment[index] = userEquippedEquipment; }
    public void SetHadWeaponList(List<ItemClass> hadWeaponList) { this.hadWeaponList = hadWeaponList; }
    public void SetHadEquipmentList(List<ItemClass> hadEquipmentList) { this.hadEquipmentList = hadEquipmentList; }
    public void SetEquipedEquipmentList(ItemClass equipEquipment,int index) { this.userEquippedEquipment[index] = equipEquipment; }
    public void SetHadGemList(List<ItemClass> hadGemList) { this.hadGemList = hadGemList; }
    public void SetHadFoodList(List<ItemClass> hadFoodList) { this.hadFoodList= hadFoodList; }
    public void SetHadEtcItemList(List<ItemClass> hadEtcItemList) { this.hadEtcItemList = hadEtcItemList; }
    public void SetHadGrowMaterialList(List<ItemClass> hadGrowMaterialList) { this.hadGrowMaterialList= hadGrowMaterialList; }
    public void SetUserLastConnectTime(DateTime userLastConnectTime) { this.userLastConnectTime = userLastConnectTime; }
    public void SetMora(int nMora) { this.nMora = nMora; }

    public CharacterClass GetUserCharacter() { return this.userCharacter; }
    public ItemClass GetUserEquippedWeapon() { return this.userEquippedWeapon;}
    public ItemClass[] GetUserEquippedEquipment() { return this.userEquippedEquipment;}
    public List<ItemClass> GetHadWeaponList() { return this.hadWeaponList; }
    public List<ItemClass> GetHadEquipmentList() { return this.hadEquipmentList; }
    public List<ItemClass> GetHadGemList() { return this.hadGemList; }
    public List<ItemClass> GetHadFoodList() { return this.hadFoodList; }
    public List<ItemClass> GetHadGrowMaterialList() { return this.hadGrowMaterialList; }
    public List<ItemClass> GetHadEtcItemList() { return hadEtcItemList; } 
    public DateTime GetUserLastConnectTime() { return this.userLastConnectTime; }
    public int GetMora() { return this.nMora; }

}
