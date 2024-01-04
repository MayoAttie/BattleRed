using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class UseTool
{
    // 애니메이션의 길이를 얻는 함수
    public static float GetAnimationLength(Animator animator, string clipName)
    {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;

        foreach (AnimationClip clip in clips)
        {
            if (clip.name == clipName)
            {
                return clip.length;
            }
        }

        return 0f;
    }

    public static void AddItemToList_CopyData(string itemName, List<ItemClass> itemList)
    {
        ItemClass item = GameManager.Instance.GetItemDataList().Find(tmp => tmp.GetName().Equals(itemName));
        if (item != null)
        {
            ItemClass copy = new ItemClass();
            copy.CopyFrom(item);
            itemList.Add(copy);
        }
    }

    public static bool IsCurrentSceneNameCorrect(string name)
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName.Equals(name))
            return true;
        else
            return false;
    }

    public static bool IsTagEquips(ItemClass tmp)
    {
        if (tmp.GetTag() == "꽃" || tmp.GetTag() == "성배" || tmp.GetTag() == "왕관" || tmp.GetTag() == "모래" || tmp.GetTag() == "깃털")
            return true;
        else
            return false;
    }
}
