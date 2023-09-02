﻿using UnityEngine;

public class ItemSpritesSaver : Singleton<ItemSpritesSaver>
{
    /* 웨폰 목록
     * 0 - 천공의 검
     * 1 - 제례검
     */
    public Sprite[] WeaponSprites;

    /* 장비 목록
     * 
     */
    public Sprite[] EquipSprites;

    /* 보석 목록
     * 
     */
    public Sprite[] GemSprites;

    /* 음식 목록
     * 
     */
    public Sprite[] FoodSprites;


    /*  이미지 그라데이션 모음
     *  0 - 5성 그라데이션
     *  1 - 4성 그라데이션
     *  2 - 3성 그라데이션
     *  3 - 디폴트 그라데이션
     */
    public Sprite[] GradationSprite;


    // 색상 코드
    Color FiveStarColor = new Color(177f / 255f, 115f / 255f, 43f / 255f);
    Color FourStarColor = new Color(138f / 255f, 107f / 255f, 170f / 255f);
    Color ThreeStarColor = new Color(86f / 255f, 131f / 255f, 164f / 255f);
    Color OneStarColor = new Color(108f / 255f, 105f / 255f, 108f / 255f);


    public Color GetFiveStarColor() { return FiveStarColor; }
    public Color GetFourStarColor() { return FourStarColor; }
    public Color GetThreeStarColor() { return ThreeStarColor; }
    public Color GetOneStarColor() { return OneStarColor; }

}
