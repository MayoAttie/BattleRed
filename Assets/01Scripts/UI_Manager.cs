using UnityEngine;
public class UI_Manager : EnergyBarManager
{
    // 싱글턴 인스턴스
    private static UI_Manager instance;
    public static UI_Manager Instance { get { return instance; } }

    private void Awake()
    {
        // 싱글턴 인스턴스 설정
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 있다면 이 오브젝트는 파괴
        }
    }

    public void BagOpenButtonClick()
    {
        GameManager.Instance.PauseManager();
        Debug.Log("일시정지");
    }
}
