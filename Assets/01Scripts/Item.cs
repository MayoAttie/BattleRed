using UnityEngine;

public class Item : Objects
{
    #region 구조체
    public enum e_ItemType
    {
        None = 0,
        Consum,
        EquipMent,
        Quest,
        Etc,
        Max
    }

    #endregion

    e_ItemType itemType;
    int nItemCount;

    public Item(string sTag, string sName, int nGrade, bool isActive, e_ItemType itemType, int nItemCount)
        : base(sTag, sName, nGrade, isActive)
    {
        this.itemType = itemType;
        this.nItemCount = nItemCount;
    }

    public Item()
    {
    }

    #region 게터세터
    public void SetItemType(e_ItemType itemType) { this.itemType = itemType; }
    public e_ItemType GetItemType() { return this.itemType; }
    public void SetItemCount(int nItemCount) { this.nItemCount = nItemCount; }
    public int GetItemCount() { return this.nItemCount;}
    #endregion

}
