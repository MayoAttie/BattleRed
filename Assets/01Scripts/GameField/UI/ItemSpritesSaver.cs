using UnityEngine;

public class ItemSpritesSaver : Singleton<ItemSpritesSaver>
{
    /* 웨폰 목록
     * 0 - 천공의 검
     * 1 - 제례검
     * 2 - 여명신검
     */
    [Header("무기 스프라이트")]
    public Sprite[] WeaponSprites;

    /* 장비 목록
     * <성배, 깃털, 왕관, 꽃, 모래 순서>
     * 0~4 - 행자의 마음
     * 5~9 - 전투광
     * 10~14 - 피에 물든 기사도
     */
    [Header("성유물 스프라이트")]
    public Sprite[] EquipSprites;

    /* 보석 목록
     * 0~2 - 철광석, 백철, 수정덩이
     * 3~5 - 정제용 하급 광물, 정제용 광물, 정제용 마법 광물
     * 6~8 - 야박석, 전기수정, 콜라피스
     */
    [Header("광물 스프라이트")]
    public Sprite[] GemSprites;

    /* 음식 목록
     * 0~2 - 어부 토스트, 버섯닭꼬치, 달콤달콤 닭고기 스튜
     * 3~4 - 냉채수육, 허니캐럿그릴
     * 5~7 - 스테이크, 무스프, 몬드 감자전
     * 8~10 - 방랑자의 경험, 모험자의 경험, 영웅의 경험(경험치 책)
     */
    [Header("음식 스프라이트")]
    public Sprite[] FoodSprites;

    /* 육성 소재 목록
     * 0~2 - 슬라임 액
     * 3~5 - 이능 두루마리
     * 6~8 - 지맥 가지
     * 9~11 - 혼돈 장치
     * 12~15 - 칼바람 이빨
     * 16~19 - 투사 사슬
     */
    [Header("육성 소재 스프라이트")]
    public Sprite[] GrowMaterialSprite;


    /*  이미지 그라데이션 모음
     *  0 - 5성 그라데이션
     *  1 - 4성 그라데이션
     *  2 - 3성 그라데이션
     *  3 - 디폴트 그라데이션
     */
    [Header("배경 그라데이션 스프라이트")]
    public Sprite[] GradationSprite;


    /*
     * 0 - 열쇠
     */
    [Header("기타 스프라이트")]
    public Sprite[] EtcSprite;

    /*  기타 스프라이트 모음
     *  0 - Back 스프라이트
     *  1 - Close 스프라이트
     *  2 - Plus 스프라이트
     *  3 - 모라
     *  4 - 합성마크
     *  5 - 보물상자
     *  6 - 선인장 몬스터
     *  7 - 버섯 몬스터
     */
    [Header("스프라이트 모음")]
    public Sprite[] SpritesSet;


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
    public Color GetColorAtGarade(int grade)
    {
        switch(grade)
        {
            case 5:
                return FiveStarColor;
            case 4:
                return FourStarColor;
            case 3: 
                return ThreeStarColor;
            default:
                return OneStarColor;
        }
    }


}
