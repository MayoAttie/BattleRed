using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using UnityEngine;
using static EquipmentSetSynergyMng;
public class CharacterUpgradeManager
{
    // 돌파 상태 도달 시, true 반환
    public static bool WeaponExpUp_Upgrade(WeaponAndEquipCls weapon, int nWeaponUpgradeExp)
    {
        CharacterDataReviseWhenWeaponTakeOff(); // 기존 Add 데이터 제거
        var data = GameManager.Instance.GetUserClass().GetHadWeaponList().Find(tmp => tmp.Equals(weapon));
        var weaponData = data as WeaponAndEquipCls;

        int curExp = weapon.GetCurrentExp();
        int nextExp = curExp + nWeaponUpgradeExp;
        bool isLimit = false;
        weapon.SetCurrentExp(nextExp);
        weaponData.SetCurrentExp(nextExp);

        while(weapon.GetCurrentExp() >= weapon.GetMaxExp())
        {
            if(weapon.GetLimitLevel()>weapon.GetLevel())
            {
                int curLevel = weapon.GetLevel();

                weapon.SetLevel(curLevel+1);
                weaponData.SetLevel(curLevel+1);

                weapon.SetMaxExp((int)(weapon.GetMaxExp()*1.6f));
                weaponData.SetMaxExp(weapon.GetMaxExp());
            }
            else // 돌파단계 도달
            {
                weapon.SetCurrentExp(weapon.GetMaxExp());
                weaponData.SetCurrentExp(weapon.GetMaxExp());

                isLimit = true; ;
                break;
            }
        }
        // 장비의 메인 및 서브 스탯 데이터 수정
        GameManager.Instance.WeaponItemStatusSet(weapon);
        GameManager.Instance.WeaponItemStatusSet(weaponData);
        CharacterDataReviseToWeapon();  // 업그레이드 된 Add 데이터 반영
        return isLimit;
    }

    public static void EquipmentExpUp_Upgrade(WeaponAndEquipCls equip, int nWeaponUpgradeExp)
    {
        int index = 0;
        if (equip.GetTag() == "꽃") index = 0;
        else if (equip.GetTag() == "깃털") index = 1;
        else if (equip.GetTag() == "모래") index = 2;
        else if (equip.GetTag() == "성배") index = 3;
        else if (equip.GetTag() == "왕관") index = 4;

        CharacterDataReviseWhenEquipmentTakeOff(index); // 기존 Add 데이터 제거

        var data = GameManager.Instance.GetUserClass().GetHadEquipmentList().Find(tmp => tmp.Equals(equip));
        var equipData = data as WeaponAndEquipCls;

        int curExp = equip.GetCurrentExp();
        int nextExp = curExp + nWeaponUpgradeExp;
        equip.SetCurrentExp(nextExp);
        equipData.SetCurrentExp(nextExp);

        while (equip.GetCurrentExp() >= equip.GetMaxExp())
        {
            if (equip.GetLimitLevel() > equip.GetLevel())
            {
                int curLevel = equip.GetLevel();

                equip.SetLevel(curLevel + 1);
                equipData.SetLevel(curLevel + 1);

                equip.SetMaxExp((int)(equip.GetMaxExp() * 1.6f));
                equipData.SetMaxExp(equip.GetMaxExp());
            }
            else // 돌파단계 도달
            {
                equip.SetCurrentExp(equip.GetMaxExp());
                equipData.SetCurrentExp(equip.GetMaxExp());

                break;
            }
        }


        GameManager.Instance.EquipItemStatusSet(equip);
        GameManager.Instance.EquipItemStatusSet(equipData);
        CharacterDataReviseToEquipment(index);  // 업그레이드된 Add 데이터 반영
    }

    // 돌파 시, 다음 능력치 리턴(0-메인스탯, 1-서브스탯)
    public static float[] WeapomLimitBreakStateUp(WeaponAndEquipCls weapon)
    {
        float[] result = new float[2];

        int nextLevel = weapon.GetLimitLevel() + 1;
        var nextDataList = GameManager.Instance.GetList_WeaponAndEquipDataBase();
        var fullData =  nextDataList.Find(finder => finder.Item1.Equals(weapon.GetName()));
        var nextData = fullData.Item2.Find(tmp => tmp.LEVEL.Equals(nextLevel));
        result[0] = nextData.MAIN_STAT;
        result[1] = nextData.SUB_STAT;

        return result;
    }

    public static void WeaponLimitBreakFunction(WeaponAndEquipCls weapon)
    {
        var data = GameManager.Instance.GetUserClass().GetHadWeaponList().Find(tmp => tmp.Equals(weapon));
        var weaponData = data as WeaponAndEquipCls;

        int curLevel = weapon.GetLimitLevel();
        int nextLimitLevel = 0;
        switch(curLevel)
        {
            case 20:
                nextLimitLevel = 40;
                break;
            case 40:
                nextLimitLevel = 50;
                break;
            case 50:
                nextLimitLevel = 60;
                break;
            case 60:
                break;
        }
        // 0이 아닐 경우, 돌파
        if (nextLimitLevel != 0)
        {
            weapon.SetLimitLevel(nextLimitLevel);
            weaponData.SetLimitLevel(nextLimitLevel);
        }
    }

    // 재련 후, 데이터 수정
    public static void WeaponReforgeFunction(WeaponAndEquipCls weapon)
    {
        var data = GameManager.Instance.GetUserClass().GetHadWeaponList().Find(tmp => tmp.Equals(weapon));
        var weaponData = data as WeaponAndEquipCls;

        int curLevel = weapon.GetEffectLevel();
        weapon.SetEffectLevel(curLevel+1);
        weaponData.SetEffectLevel(curLevel+1);

        string nextStr = GameManager.Instance.NextReforgeEffectData(weapon);
        weapon.SetEffectText(nextStr);
        weaponData.SetEffectText(nextStr);
    }

    // 무기 데이터를 캐릭터의 능력치 데이터에 반영하는 함수
    public static void CharacterDataReviseToWeapon()
    {
        CharacterClass characterCls = GameManager.Instance.GetUserClass().GetUserCharacter();
        WeaponAndEquipCls weaponCls = GameManager.Instance.GetUserClass().GetUserEquippedWeapon() as WeaponAndEquipCls;

        int characAtk = characterCls.GetAttack();

        float mainStat = weaponCls.GetMainStat();
        float subStat = weaponCls.GetSubStat();

        // 무기 공격력 캐릭터 공격력에 반영
        characterCls.SetAttack(characAtk + (int)mainStat);
        float currentValue = 0;
        switch (weaponCls.GetName())
        {
            case "천공의 검":
                {   //원충
                    currentValue = characterCls.GetElementCharge();
                    characterCls.SetElementCharge(currentValue+ subStat);
                }
                break;
            case "제례검":
                {   //원충
                    currentValue = characterCls.GetElementCharge();
                    characterCls.SetElementCharge(currentValue + subStat);
                }
                break;
            case "여명신검":
                {   // 치피
                    currentValue = characterCls.GetCriticalDamage();
                    characterCls.SetCriticalDamage(currentValue+ subStat);
                }
                break;
        }
    }
    // 기존의 장비를 해제했을 때, 캐릭터 데이터를 수정하는 함수.
    public static void CharacterDataReviseWhenWeaponTakeOff()
    {
        CharacterClass characterCls = GameManager.Instance.GetUserClass().GetUserCharacter();
        WeaponAndEquipCls weaponCls = GameManager.Instance.GetUserClass().GetUserEquippedWeapon() as WeaponAndEquipCls;

        if (weaponCls == null) return;

        int characAtk = characterCls.GetAttack();

        float mainStat = weaponCls.GetMainStat();
        float subStat = weaponCls.GetSubStat();

        // 무기 공격력 캐릭터 공격력에서 제외
        characterCls.SetAttack(characAtk - (int)mainStat);
        float currentValue = 0;

        switch (weaponCls.GetName())
        {
            case "천공의 검":
                {   //원충
                    currentValue = characterCls.GetElementCharge();
                    characterCls.SetElementCharge(currentValue - subStat);
                }
                break;
            case "제례검":
                {   //원충
                    currentValue = characterCls.GetElementCharge();
                    characterCls.SetElementCharge(currentValue - subStat);
                }
                break;
            case "여명신검":
                {   // 치피
                    currentValue = characterCls.GetCriticalDamage();
                    characterCls.SetCriticalDamage(currentValue - subStat);
                }
                break;
        }
    }
    // 성유물 데이터의 능력치를 캐릭터 능력치 데이터에 반영하는 함수
    public static void CharacterDataReviseToEquipment()
    {
        CharacterClass characterCls = GameManager.Instance.GetUserClass().GetUserCharacter();
        var equipDatas = GameManager.Instance.GetUserClass().GetUserEquippedEquipment();
        
        float stat = 0;

        for(int i =0; i<equipDatas.Length; i++)
        {
            if (equipDatas[i] == null) continue;

            var reItem = equipDatas[i] as WeaponAndEquipCls;
            stat = reItem.GetMainStat();
            if(reItem.GetTag() == "꽃")      // 체력 업
            {
                int characterMaxHp = characterCls.GetMaxHp();
                int characterCurrentHp = characterCls.GetCurrentHp();
                characterCls.SetMaxHp(characterMaxHp + (int)stat);
                characterCls.SetCurrentHp(characterCurrentHp + (int)stat);
            }
            else if(reItem.GetTag() == "깃털")    // 공격력 업
            {
                int characterAttack = characterCls.GetAttack();
                characterCls.SetAttack(characterAttack + (int)stat);
            }
            else                                // 피봇에 따라서 분기 후, 능력치 업그레이드
            {
                string reDataText = reItem.GetEffectText();
                string pivot = ""; // pivot 문자열 초기화
                if (reDataText.Length > 7) // 8번째 값부터 끝까지의 부분을 pivot에 저장
                    pivot = reDataText.Substring(7);

                switch(pivot)
                {
                    case "공격력": // 공격력은 기존 데이터에 %로 증가
                        {
                            int characterAttack = characterCls.GetAttack();
                            float tmp = (float)(characterAttack * (stat * 0.01));
                            GameManager.Instance.GetUserClass().GetUserCharacter().AddItemEffect(i,0,(int)tmp,0);
                            characterCls.SetAttack(characterAttack + (int)tmp);
                        }
                        break;
                    case "방어력": // 방어력은 기존 데이터의 %로 증가
                        {
                            int characterDeffense = characterCls.GetDeffense();
                            float tmp = (float)(characterDeffense * (stat * 0.01));
                            GameManager.Instance.GetUserClass().GetUserCharacter().AddItemEffect(i, 0, 0, (int)tmp);
                            characterCls.SetDeffense(characterDeffense + (int)tmp);
                        }
                        break;
                    case "체력":  // 방어력은 기존 데이터의 %로 증가
                        {
                            int characterMaxHp = characterCls.GetMaxHp();
                            int characterCurrentHp = characterCls.GetCurrentHp();
                            float tmp1 = (float)(characterMaxHp * (stat * 0.01));
                            float tmp2 = (float)(characterCurrentHp * (stat * 0.01));
                            GameManager.Instance.GetUserClass().GetUserCharacter().AddItemEffect(i, (int)tmp1, 0, 0);
                            characterCls.SetMaxHp(characterMaxHp + (int)tmp1);
                            characterCls.SetCurrentHp(characterCurrentHp + (int)tmp2);
                        }
                        break;
                    case "원소 마스터리": // 원소 마스터리는 기존 데이터에 정량 증가
                            int characterElement = characterCls.GetElementNum();
                            characterCls.SetElementNum(characterElement+ (int)stat);
                        break;
                    case "치명타 확률":  // 치확은 기존 데이터에 정량 증가
                        float characterCriticalPercentage = characterCls.GetCriticalPercentage();
                        characterCls.SetCriticalPersentage(characterCriticalPercentage + stat);
                        break;
                    case "치명타 피해":  // 치피는 기존 데이터에 정량 증가
                        float characterCriticalDamage = characterCls.GetCriticalDamage();   
                        characterCls.SetCriticalDamage(characterCriticalDamage + stat);
                        break;
                    default: break;
                }
            }
        }
    }
    //착용한 성유물의 스탯을 캐릭터에 반영
    public static void CharacterDataReviseToEquipment(int index)
    {
        CharacterClass characterCls = GameManager.Instance.GetUserClass().GetUserCharacter();
        var equipDatas = GameManager.Instance.GetUserClass().GetUserEquippedEquipment()[index];

        var reItem = equipDatas as WeaponAndEquipCls;
        if (reItem == null) return;
        
        float stat = reItem.GetMainStat();

        if (reItem.GetTag() == "꽃") // 체력 업
        {
            int characterMaxHp = characterCls.GetMaxHp();
            int characterCurrentHp = characterCls.GetCurrentHp();
            characterCls.SetMaxHp(characterMaxHp + (int)stat);
            characterCls.SetCurrentHp(characterCurrentHp + (int)stat);
        }
        else if (reItem.GetTag() == "깃털") // 공격력 업
        {
            int characterAttack = characterCls.GetAttack();
            characterCls.SetAttack(characterAttack + (int)stat);
        }
        else // 피봇에 따라서 분기 후, 능력치 업그레이드
        {
            string reDataText = reItem.GetEffectText();
            string pivot = ""; // pivot 문자열 초기화
            if (reDataText.Length > 7) // 8번째 값부터 끝까지의 부분을 pivot에 저장
                pivot = reDataText.Substring(7);

            switch (pivot)
            {
                case "공격력": // 공격력은 기존 데이터에 %로 증가
                    {
                        int characterAttack = characterCls.GetAttack();
                        float tmp = (float)(characterAttack * (stat * 0.01));
                        tmp = tmp <= 0 ? 1 : tmp;
                        GameManager.Instance.GetUserClass().GetUserCharacter().AddItemEffect(index, 0, (int)tmp, 0);
                        characterCls.SetAttack(characterAttack + (int)tmp);
                    }
                    break;
                case "방어력": // 방어력은 기존 데이터의 %로 증가
                    {
                        int characterDefense = characterCls.GetDeffense();
                        float tmp = (float)(characterDefense * (stat * 0.01));
                        tmp = tmp <= 0 ? 1 : tmp;
                        GameManager.Instance.GetUserClass().GetUserCharacter().AddItemEffect(index, 0, 0, (int)tmp);
                        characterCls.SetDeffense(characterDefense + (int)tmp);
                    }
                    break;
                case "체력": // 체력은 기존 데이터의 %로 증가
                    {
                        int characterMaxHp = characterCls.GetMaxHp();
                        int characterCurrentHp = characterCls.GetCurrentHp();
                        float tmp1 = (float)(characterMaxHp * (stat * 0.01));
                        float tmp2 = (float)(characterCurrentHp * (stat * 0.01));
                        tmp1 = tmp1 <= 0 ? 1 : tmp1;
                        tmp2 = tmp2 <= 0 ? 1 : tmp2;
                        GameManager.Instance.GetUserClass().GetUserCharacter().AddItemEffect(index, (int)tmp1, 0, 0);
                        characterCls.SetMaxHp(characterMaxHp + (int)tmp1);
                        characterCls.SetCurrentHp(characterCurrentHp + (int)tmp2);
                    }
                    break;
                case "원소 마스터리": // 원소 마스터리는 기존 데이터에 정량 증가
                    int characterElement = characterCls.GetElementNum();
                    characterCls.SetElementNum(characterElement + (int)stat);
                    break;
                case "치명타 확률": // 치확은 기존 데이터에 정량 증가
                    float characterCriticalPercentage = characterCls.GetCriticalPercentage();
                    characterCls.SetCriticalPersentage(characterCriticalPercentage + stat);
                    break;
                case "치명타 피해": // 치피는 기존 데이터에 정량 증가
                    float characterCriticalDamage = characterCls.GetCriticalDamage();
                    characterCls.SetCriticalDamage(characterCriticalDamage + stat);
                    break;
                default: break;
            }
        }
        UI_Manager.Instance.HpBarFill_Init(characterCls.GetCurrentHp());
        UI_Manager.Instance.HpBarFill_End(characterCls.GetMaxHp(), characterCls.GetCurrentHp(), true);

    }
    // 장착 해제한 성유물의 데이터를 캐릭터에서 해제
    public static void CharacterDataReviseWhenEquipmentTakeOff(int index)
    {
        CharacterClass characterCls = GameManager.Instance.GetUserClass().GetUserCharacter();
        var equipData = GameManager.Instance.GetUserClass().GetUserEquippedEquipment()[index] as WeaponAndEquipCls;

        if (equipData == null) return;
        
        float stat = 0;

        stat = equipData.GetMainStat();
        if (equipData.GetTag() == "꽃")      // 체력 업
        {
            int characterMaxHp = characterCls.GetMaxHp();
            int characterCurrentHp = characterCls.GetCurrentHp();
            characterCls.SetMaxHp(characterMaxHp - (int)stat);
            characterCls.SetCurrentHp(characterCurrentHp - (int)stat);
        }
        else if (equipData.GetTag() == "깃털")    // 공격력 업
        {
            int characterAttack = characterCls.GetAttack();
            characterCls.SetAttack(characterAttack - (int)stat);
        }
        else                          // 피봇에 따라서 분기 후, 능력치 제거
        {
            string reDataText = equipData.GetEffectText();
            string pivot = ""; // pivot 문자열 초기화
            if (reDataText.Length > 7) // 8번째 값부터 끝까지의 부분을 pivot에 저장
                pivot = reDataText.Substring(7);

            switch (pivot)
            {
                case "공격력": // 증가분을 가져와 감산
                    {
                        int characterAttack = characterCls.GetAttack();
                        int tmp = GameManager.Instance.GetUserClass().GetUserCharacter().GetItemAddAttack(index);
                        GameManager.Instance.GetUserClass().GetUserCharacter().RemoveItemEffect(index);
                        characterCls.SetAttack(characterAttack - (int)tmp);
                    }
                    break;
                case "방어력": // 증가분을 가져와서 감산
                    {
                        int characterDeffense = characterCls.GetDeffense();
                        int tmp = GameManager.Instance.GetUserClass().GetUserCharacter().GetItemAddDefense(index);
                        GameManager.Instance.GetUserClass().GetUserCharacter().RemoveItemEffect(index);
                        characterCls.SetDeffense(characterDeffense - (int)tmp);
                    }
                    break;
                case "체력":  // 증가분을 가져와서 감산
                    {
                        int characterMaxHp = characterCls.GetMaxHp();
                        int characterCurrentHp = characterCls.GetCurrentHp();
                        int tmp = GameManager.Instance.GetUserClass().GetUserCharacter().GetItemAddHp(index);
                        GameManager.Instance.GetUserClass().GetUserCharacter().RemoveItemEffect(index);
                        if (characterMaxHp > 0 && characterCurrentHp > 0)
                        {
                            float hpRatio = (float)characterCurrentHp / characterMaxHp; // 현재 체력 비율
                            int adjustedMaxHp = characterMaxHp - (int)tmp;
                            int adjustedCurrentHp = (int)(adjustedMaxHp * hpRatio);

                            characterCls.SetMaxHp(adjustedMaxHp);
                            characterCls.SetCurrentHp(adjustedCurrentHp);
                        }
                    }
                    break;
                case "원소 마스터리": // 원소 마스터리는 기존 데이터에 정량 증가
                    int characterElement = characterCls.GetElementNum();
                    characterCls.SetElementNum(characterElement - (int)stat);
                    break;
                case "치명타 확률":  // 치확은 기존 데이터에 정량 증가
                    float characterCriticalPercentage = characterCls.GetCriticalPercentage();
                    characterCls.SetCriticalPersentage(characterCriticalPercentage - stat);
                    break;
                case "치명타 피해":  // 치피는 기존 데이터에 정량 증가
                    float characterCriticalDamage = characterCls.GetCriticalDamage();
                    characterCls.SetCriticalDamage(characterCriticalDamage - stat);
                    break;
                default: break;
            }
        }
        UI_Manager.Instance.HpBarFill_Init(characterCls.GetCurrentHp());
        UI_Manager.Instance.HpBarFill_End(characterCls.GetMaxHp(), characterCls.GetCurrentHp(), true);
    }

    // 기본 스테이터스를 변경하는 세트 효과 적용 함수
    public static void EquipmentSetSynergyApplyer()
    {
        var equips = GameManager.Instance.GetUserClass().GetUserEquippedEquipment();
        Dictionary<string, int> sets = new Dictionary<string, int>();
        // 착용한 장비를 순회하며, 세트 개수를 저장
        foreach (var equip in equips) 
        {
            if(equip == null)
                continue;

            // 딕셔너리에 세트의 개수를 저장
            if (!sets.ContainsKey(equip.GetSet()))
                sets[equip.GetSet()] = 0;
            sets[equip.GetSet()]++;
        }
        //  딕셔너리를 순회하며, 세트에 따라, 유저 캐릭터 데이터 수정
        foreach(var set in sets)
        {
            string setName = set.Key;
            var userClass = GameManager.Instance.GetUserClass().GetUserCharacter();
            if(set.Value>=4)    // 세트의 숫자가 4 이상일 경우
            {
                switch (setName)
                {
                    case "행자의 마음":
                        if (userClass.GetEquipSetApplied(setName, 4) == 0)
                            Hangja_Heart_4(userClass, null, 0, false);
                        if (userClass.GetEquipSetApplied(setName,2) ==0)
                            Hangja_Heart_2(userClass,true);
                        break;
                    case "전투광":
                        if (userClass.GetEquipSetApplied(setName, 4) == 0)
                            JeonjaengGwang_4(userClass, false,false);
                        if (userClass.GetEquipSetApplied(setName, 2) == 0)
                            JeonjaengGwang_2(userClass,true);
                        break;
                    case "피에 물든 기사도":
                        if (userClass.GetEquipSetApplied(setName, 4) == 0)
                            BloodKnightChivalry_4(userClass, true);
                        if (userClass.GetEquipSetApplied(setName, 2) == 0)
                            BloodKnightChivalry_2(userClass, true);
                        break;
                }

            }
            else if(set.Value>=2)   // 세트의 숫자가 2 이상일 경우
            {
                switch (setName)
                {
                    case "행자의 마음":
                        if (userClass.GetEquipSetApplied(setName, 2) == 0)
                            Hangja_Heart_2(userClass, true);
                        break;
                    case "전투광":
                        if (userClass.GetEquipSetApplied(setName, 2) == 0)
                            JeonjaengGwang_2(userClass, true);
                        break;
                    case "피에 물든 기사도":
                        if (userClass.GetEquipSetApplied(setName, 2) == 0)
                            BloodKnightChivalry_2(userClass, true);
                        break;
                }
            }
        }
    }

    public static void EquipmentSetSynergyUnApplyer()
    {
        var equips = GameManager.Instance.GetUserClass().GetUserEquippedEquipment();
        Dictionary<string, int> sets = new Dictionary<string, int>();
        var userClass = GameManager.Instance.GetUserClass().GetUserCharacter();

        // 착용한 장비를 순회하며, 세트 개수를 저장
        foreach (var equip in equips)
        {
            if(equip == null)
                continue;

            // 딕셔너리에 세트의 개수를 저장
            if (!sets.ContainsKey(equip.GetSet()))
                sets[equip.GetSet()] = 0;
            sets[equip.GetSet()]++;
        }
        var curAppliedSet = userClass.GetEquipSetAppliedDictionary();
        var itemsToRemove = new List<KeyValuePair<Tuple<string, int>, float>>();
        Dictionary<Tuple<string, int>, float> copiedSynergyData = new Dictionary<Tuple<string, int>, float>(curAppliedSet);

        // 기존의 시너지 효과 중에서 해제된 것을 데이터에서 제거
        foreach (var item in copiedSynergyData)   // 기존 시너지 효과 딕셔너리를 순회
        {
            bool isHave = false;
            foreach (var set in sets)   // 착용한 장비를 순회
            {
                int offset = 0;
                if (set.Value == 4 || set.Value == 5)
                    offset = 4;
                if (set.Value == 3 || set.Value == 2)
                    offset = 2;
                if(userClass.GetEquipSetApplied(set.Key,offset)!=0 && set.Key==item.Key.Item1 && offset == item.Key.Item2)
                {
                    isHave = true;
                    break;
                }
            }

            // 착용 중인 시너지 효과가 아닐 경우에는, 관리 딕셔너리에서 데이터 제거 및 캐릭터 데이터 수정
            if(isHave==false)
            {
                // 캐릭터의 강화 데이터 제거
                switch(item.Key.Item1)
                {
                    case "행자의 마음":
                        if (item.Key.Item2 == 2)
                            Hangja_Heart_2(userClass, false);
                        break;
                    case "전투광":
                        if (item.Key.Item2 == 4)
                            JeonjaengGwang_4(userClass, false, true);
                        if (item.Key.Item2 == 2)
                            JeonjaengGwang_2(userClass, false);
                        break;
                    case "피에 물든 기사도":
                        if (item.Key.Item2 == 4)
                            BloodKnightChivalry_4(userClass, false);
                        if (item.Key.Item2 == 2)
                            BloodKnightChivalry_2(userClass, false);
                        break;
                }
                curAppliedSet.Remove(item.Key);
            }
        }
    }
}
