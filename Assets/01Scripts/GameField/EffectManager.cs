using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HandlePauseTool;
using Coffee.UIParticleExtensions;
using Coffee.UIExtensions;
public class EffectManager : Singleton<EffectManager>
{

    // 0번 : 청색 발광
    // 1번 : 무색 폭죽
    // 2번 : 붉은 발광
    // 3번 : 폭발
    [SerializeField] GameObject[] effects;
    [SerializeField] GameObject[] ui_effect;

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


    public void UI_ParticleCreate(Transform pos, int index)
    {
        UI_ParticleCreate(pos, index, 1);
    }
    public void UI_ParticleCreate(Transform pos, int index, float particleScale)
    {
        if (index < 0 || index >= ui_effect.Length)
        {
            Debug.LogError("Invalid index for ui_effect array");
            return;
        }

        // 프리팹을 생성
        GameObject particlePrefab = ui_effect[index];
        GameObject particleInstance = Instantiate(particlePrefab, pos.position, Quaternion.identity);
        // 생성된 파티클을 자식으로 설정
        particleInstance.transform.SetParent(pos);

        // UIParticle 컴포넌트 가져오기
        UIParticle uiParticleComponent = particleInstance.GetComponent<UIParticle>();
        // 플레이 메서드 호출
        uiParticleComponent.Play();

        // 파티클의 로컬 스케일 조절
        particleInstance.transform.localScale *= particleScale;

        ParticleSystem particleSystem = particleInstance.GetComponent<ParticleSystem>();
        var main = particleSystem.main;
        main.startSizeMultiplier *= particleScale; // 이펙트 크기 조정
        // 재생시간 이후에 자동으로 제거
        float duration = main.duration + 0.5f;  // 이펙트 시간 설정
        Destroy(particleInstance, duration); // 이펙트 파티클 파괴
    }
}
