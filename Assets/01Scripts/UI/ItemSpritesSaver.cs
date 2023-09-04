using UnityEngine;

public class ItemSpritesSaver : Singleton<ItemSpritesSaver>
{
    /* 웨폰 목록
     * 0 - 천공의 검
     * 1 - 제례검
     */
    public Sprite[] WeaponSprites;

    /* 장비 목록
     * <성배, 깃털, 왕관, 꽃, 모래 순서>
     * 0~4 - 행자의 마음
     * 5~9 - 전투광
     * 10~14 - 피에 물든 기사도
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


    // 성급 색상 코드
    Color FiveStarColor = new Color(177f / 255f, 115f / 255f, 43f / 255f);
    Color FourStarColor = new Color(138f / 255f, 107f / 255f, 170f / 255f);
    Color ThreeStarColor = new Color(86f / 255f, 131f / 255f, 164f / 255f);
    Color OneStarColor = new Color(108f / 255f, 105f / 255f, 108f / 255f);

    // UI 색상 코드
    Color DarkColor = new Color(72f / 255f, 80f / 255f, 99f / 255f);
    Color BeigeColor = new Color(232f / 255f, 225f / 255f, 214f / 255f);


    public Color GetFiveStarColor() { return FiveStarColor; }
    public Color GetFourStarColor() { return FourStarColor; }
    public Color GetThreeStarColor() { return ThreeStarColor; }
    public Color GetOneStarColor() { return OneStarColor; }
    public Color GetDarkColor() {  return DarkColor; }
    public Color GetBeigeColor() { return BeigeColor; }


}
