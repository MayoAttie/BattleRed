using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonRotateSetCls : MonoBehaviour
{
    Button[] buttons;
    Image[] images;
    Image[] insideImages;

    Animator animator;
    float currentAnimationTime = 0;   // 현재 애니메이션의 재생 시간

    private void Awake()
    {
        buttons = new Button[5];
        images = new Image[5];
        insideImages = new Image[5];
        for (int i = 0; i < 5; i++)
        { 
            images[i] = transform.GetChild(i).GetComponent<Image>();
            buttons[i] = images[i].transform.GetChild(0).GetComponent<Button>();
            insideImages[i] = images[i].transform.GetChild(0).GetComponent<Image>();
        }

        animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        animator.SetBool("ani", true);
        for (int i = 0; i < 5; i++)
            insideImages[i].enabled = false;
    }

    // Getter 메서드
    public Button[] GetButtons() { return buttons; }
    public Image[] GetImages() { return images; }
    public Image[] GetInsideImages() { return insideImages; }
    public void SetButtons(Button[] buttons){this.buttons = buttons; }
    public void SetImages(Image[] images) { this.images = images; }
    public void SetInsideImages(Image[] images) { insideImages = images;}

    // 애니메이션 제어 메서드
    public void AnimationActiveOff()
    {
        currentAnimationTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        animator.SetBool("ani", false);
    }
    public void AnimationActiveOn()
    {
        animator.Play("ButtonRotationAnimaiton", 0, currentAnimationTime);
        animator.SetBool("ani", true);

    }

    // 애니메이션 속도를 조절하는 메서드 (음수일 경우, 역재생)
    public void SetAnimationSpeed(float speedValue)
    {
        animator.SetFloat("Speed", speedValue);
    }

}
