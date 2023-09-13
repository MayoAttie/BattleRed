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

    DateTime userLastConnectTime;

    int nGold;
    int nStarLight;

    public UserClass() 
    {
        userEquippedWeapon = new ItemClass();
        userEquippedEquipment = new ItemClass[5];
    }

    public void SetUserCharacter(CharacterClass userCharacter) { this.userCharacter = userCharacter; }
    public void SetUserEquippedWeapon(ItemClass userEquippedWeapon) { this.userEquippedWeapon = userEquippedWeapon; }
    public void SetUserEquippedEquipment(ItemClass[] userEquippedEquipment) { this.userEquippedEquipment= userEquippedEquipment; }
    public void SetUserEquippedEquipment(ItemClass userEquippedEquipment, int index) { this.userEquippedEquipment[index] = userEquippedEquipment; }
    public void SetHadWeaponList(List<ItemClass> hadWeaponList) { this.hadWeaponList = hadWeaponList; }
    public void SetHadEquipmentList(List<ItemClass> hadEquipmentList) { this.hadEquipmentList = hadEquipmentList; }
    public void SetHadGemList(List<ItemClass> hadGemList) { this.hadGemList = hadGemList; }
    public void SetHadFoodList(List<ItemClass> hadFoodList) { this.hadFoodList= hadFoodList; }
    public void SetUserLastConnectTime(DateTime userLastConnectTime) { this.userLastConnectTime = userLastConnectTime; }

    public CharacterClass GetUserCharacter() { return this.userCharacter; }
    public ItemClass GetUserEquippedWeapon() { return this.userEquippedWeapon;}
    public ItemClass[] GetUserEquippedEquipment() { return this.userEquippedEquipment;}
    public List<ItemClass> GetHadWeaponList() { return this.hadWeaponList; }
    public List<ItemClass> GetHadEquipmentList() { return this.hadEquipmentList; }
    public List<ItemClass> GetHadGemList() { return this.hadGemList; }
    public List<ItemClass> GetHadFoodList() { return this.hadFoodList; }
    public DateTime GetUserLastConnectTime() { return this.userLastConnectTime; }

}
