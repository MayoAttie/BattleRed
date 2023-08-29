using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HandlePauseTool;
public class EffectManager : Singleton<EffectManager>
{
    // 0번 : 무지개색 이팩트
    [SerializeField] GameObject[] effects;

    private void OnEnable()
    {
        // 게임매니저의 이벤트에 구독
        GameManager.OnPauseStateChanged += HandlePauseStateChanged;
    }

    private void OnDisable()
    {
        // 게임매니저의 이벤트 구독 해제
        GameManager.OnPauseStateChanged -= HandlePauseStateChanged;
    }

    // 오버로딩을 이용한, Voctor3와 float 인수 케이스 분리!
    public void EffectCreate(Transform pos, int index)
    {
        EffectCreate(pos, index, Vector3.zero, 1f);
    }
    public void EffectCreate(Transform pos, int index, Vector3 offSet)
    {
        EffectCreate(pos, index, offSet, 1f);
    }
    public void EffectCreate(Transform pos, int index, float size)
    {
        EffectCreate(pos, index, Vector3.zero, size);
    }

    
    public void EffectCreate(Transform pos, int index, Vector3 offset, float size)
    {
        GameObject obj = Instantiate(effects[index]);
        obj.transform.position = pos.position + offset; // 이펙트 위치 조정
        ParticleSystem particleSystem = obj.GetComponent<ParticleSystem>();
        particleSystem.transform.localScale *= size; // 이펙트 크기 조정
        var main = particleSystem.main;
        main.startSizeMultiplier *= size; // 이펙트 크기 조정
        float duration = main.duration + 0.5f;  // 이펙트 시간 설정
        Destroy(obj, duration); // 이펙트 파티클 파괴
    }
}
