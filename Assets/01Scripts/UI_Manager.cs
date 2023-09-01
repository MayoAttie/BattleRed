using UnityEngine;
public class UI_Manager : EnergyBarManager
{
    // 싱글턴 인스턴스
    private static UI_Manager instance;
    public static UI_Manager Instance { get { return instance; } }

    #region 변수
    // 인벤토리 관련
    public GameObject Inventory;

    [SerializeField]    // 인벤토리 버튼들
    InventoryButton[] invenButtons;

    #endregion


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

    #region 인벤토리 UI 관리
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


    #endregion

}
