using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public CharacterClass characterCls;
    private void Awake()
    {
        characterCls = new CharacterClass(1000, 100, 0, 100, 50, 15, 1, 3.0f, CharacterClass.eCharactgerState.e_NONE, eElement.e_NONE);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
