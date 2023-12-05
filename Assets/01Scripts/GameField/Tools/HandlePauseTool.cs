using UnityEngine;

public class HandlePauseTool : MonoBehaviour
{
    public static void HandlePauseStateChanged(bool isPaused)
    {
        // 일시정지 상태 변경에 따른 동작 구현
        if (isPaused)
        {
            // 일시정지 상태일 때 실행할 작업
            Time.timeScale = 0;
        }
        else
        {
            // 일시정지 해제 상태일 때 실행할 작업
            Time.timeScale = 1;
        }
    }
}
