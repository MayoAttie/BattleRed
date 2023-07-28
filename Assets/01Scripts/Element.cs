using System.Collections;
using System.Collections.Generic;

public class Element
{
    #region 변수

    e_Element element;
    bool isActive;

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

}
