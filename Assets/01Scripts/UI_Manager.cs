using UnityEngine;
public class UI_Manager : EnergyBarManager
{
    // 싱글턴 인스턴스
    private static UI_Manager instance;
    public static UI_Manager Instance { get { return instance; } }
    public GameObject Inventory;

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
        Inventory.SetActive(false);

    }

    public void BagOpenButtonClick()
    {
        GameManager.Instance.PauseManager();
        Inventory.SetActive(true);
        Debug.Log("일시정지");
    }
    public void BagCloseButtonClick()
    {
        GameManager.Instance.PauseManager();
        Inventory.SetActive(false);

    }
    public void UsingButtonClick()
    {
        Debug.Log("아이템 사용");
    }
}
