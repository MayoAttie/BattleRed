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
            buttons[i] = images[i].GetComponentInChildren<Button>();
            insideImages[i] = images[i].transform.GetChild(0).GetComponent<Image>();
        }

        animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        animator.SetInteger("controller", 0);
        for (int i = 0; i < 5; i++)
        {
            Color imageColor = insideImages[i].color; // 이미지의 현재 색상을 가져옴
            imageColor.a = 0f;                          // 알파값을 0으로 설정하여 투명하게 만듦
            insideImages[i].color = imageColor;         // 이미지의 색상을 변경
        }
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
        animator.SetInteger("controller", -1);
    }
    public void AnimationActiveOn()
    {
        animator.Play("ButtonRotationAnimaiton", 0, currentAnimationTime);
        animator.SetInteger("controller", 0);
    }

    // 애니메이션 속도를 조절하는 메서드 (음수일 경우, 역재생)
    public void SetAnimationSpeed(float speedValue)
    {
        animator.SetFloat("Speed", speedValue);
    }

    // 인덱스 값에 따라 애니메이션을 호출하는 함수
    public void SetAniControl_Play(int index)
    {
        currentAnimationTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        animator.SetFloat("Speed", 1);
        animator.SetInteger("controller", index+1);
    }

}
