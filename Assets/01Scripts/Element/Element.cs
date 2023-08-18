using System.Collections;
using System.Collections.Generic;

public class Element
{
    #region 변수

    e_Element element;
    bool isActive;
    bool isChild;

    #endregion

    #region 구조체
    public enum e_Element
    {
        None,
        Fire,
        Water,
        Plant,
        Lightning,
        Wind,
        Max
    }
    #endregion

    #region 세터게터

    public void SetElement(e_Element element)
    {
        this.element = element;
    }
    public void SetIsActive(bool isActive)
    {
        this.isActive = isActive;
    }
    public void SetIsChild(bool isChild)
    {
        this.isChild = isChild;
    }

    public e_Element GetElement() { return element; }
    public bool GetIsActive() { return isActive; }
    public bool GetIsChild() { return isChild;}

    #endregion

    public Element() { }
    public Element(e_Element element, bool isActive, bool isChild)
    {
        this.element = element;
        this.isActive = isActive;
        this.isChild = isChild;
    }



}
