using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Linq;
using UnityEngine.Playables;
using UnityEngine.Timeline;
public class TimelineManager : MonoBehaviour
{
    public PlayableDirector playableDirector;
    private void Start()
    {
        // 이벤트 등록: 타임라인 재생이 끝났을 때 호출되는 메서드를 등록
        playableDirector.stopped += OnPlayableDirectorStopped;
    }

    public void PlayTimeline_getIndex(int index)
    {
        StartCoroutine(PlayTimelineCoroutine(index));
    }

    private IEnumerator PlayTimelineCoroutine(int index)
    {
        // 인덱스에 따라 프레임 범위 계산
        float startTime = 0;
        float endTime = 0;
        //Debug.Log(index - 1);

        // 프레임 범위에 해당하는 초 계산
        IndexFunction(ref startTime, ref endTime, index);


        // 타임라인 재생 범위 설정
        playableDirector.time = startTime;

        // 타임라인 재생
        playableDirector.Play();

        // 특정 초까지 대기
        yield return new WaitForSeconds(endTime - startTime);

        // 타임라인 중지
        StopTimeline();
    }

    // 시간 계산 함수
    void IndexFunction(ref float startTime, ref float endTime, int index)
    {
        switch(index)
        {
            case 1:
                startTime = 0;
                endTime = 0.2f;
                break;
            case 2:
                startTime = 1;
                endTime = 1.46f;
                break;
            case 3:
                startTime = 2;
                endTime = 3;
                break;
            case 10:
                startTime = 4;
                endTime = 5.43f;
                break;
        }
    }

    private void StopTimeline()
    {
        // 타임라인 중지
        playableDirector.Stop();
    }

    private void OnPlayableDirectorStopped(PlayableDirector director)
    {
        // 타임라인이 종료되면 호출되는 메서드
        //Debug.Log("Timeline Stopped");

        // 여기에 타임라인 종료 시 수행할 작업을 추가할 수 있음
    }
}
