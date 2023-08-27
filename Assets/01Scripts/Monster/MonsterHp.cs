using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterHp : EnergyBarManager
{

    Canvas canvas;
    Camera hpCamera;
    RectTransform rectParent;
    RectTransform rectHp;
    Vector3 monsterPos;

    private void OnEnable()
    {
        Hpbar = gameObject.transform.GetChild(0).GetComponent<Image>();

    }


    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        hpCamera = canvas.worldCamera;
        rectParent = canvas.GetComponent<RectTransform>();
        rectHp = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        var screenPos = Camera.main.WorldToScreenPoint(monsterPos); // 몬스터의 월드 3d좌표를 스크린좌표로 변환

        if (screenPos.z < 0.0f)
        {
            screenPos *= -1.0f;
        }

        var localPos = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectParent, screenPos, hpCamera, out localPos); // 스크린 좌표를 다시 체력바 UI 캔버스 좌표로 변환

        rectHp.localPosition = localPos; // 체력바 위치조정
        Debug.Log(nameof(localPos) + ":" + localPos);
        // 스케일을 항상 (1, 1, 1)로 설정
        rectHp.localScale = Vector3.one;
    }

    public void SetMonsterPos(Vector3 pos) { monsterPos = pos; }




}
