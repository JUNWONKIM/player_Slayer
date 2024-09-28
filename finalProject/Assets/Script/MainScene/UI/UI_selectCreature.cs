using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI_selectCreature : MonoBehaviour
{
    public Button myButton;
    public int buttonIndex; // 버튼 인덱스
    public float cooldownTime = 0f; // 버튼의 쿨타임 시간
    public Color activeColor = Color.white; // 버튼 활성화 색상
    public Color cooldownColor = Color.red; // 버튼 쿨타임 색상

    private ColorBlock originalColors; // 버튼의 원래 색상 저장
    private bool isOnCooldown = false; // 쿨다운 확인
    private float cooldownEndTime; // 쿨타임이 끝나는 시간을 저장
    private CreatureSpawner spawner; // CreatureSpawner 스크립트 참조

    void Start()
    {
        if (myButton != null)
        {
            originalColors = myButton.colors; // 버튼의 원래 색상 저장
            myButton.onClick.AddListener(OnButtonClick); // 버튼 클릭 이벤트 설정
        }

        spawner = FindObjectOfType<CreatureSpawner>();
        UpdateButtonState(); // 버튼 상태 설정
    }

    void Update()
    {
        if (spawner != null && myButton != null)
        {
            UpdateButtonState();
        }

        if (isOnCooldown)
        {
            UpdateCooldown();
        }
    }

    void UpdateButtonState()
    {
        if (spawner.selectedCreature == buttonIndex)
        {
            PressButton();
        }
        else
        {
            ReleaseButton();
        }
    }

    void PressButton() // 버튼이 눌릴 경우
    {
        var colors = myButton.colors;
        colors.normalColor = colors.pressedColor;
        myButton.colors = colors;
    }

    void ReleaseButton() // 버튼이 해제될 경우
    {
        myButton.colors = originalColors;
    }

    void OnButtonClick() // 버튼이 눌릴 경우 쿨타임 시작
    {
        if (!isOnCooldown)
        {
            StartCooldown();
        }
    }

    public void StartCooldown() // 쿨타임 시작
    {
        isOnCooldown = true;
        cooldownEndTime = Time.time + cooldownTime;

        if (myButton.image != null)
        {
            myButton.image.color = cooldownColor; // 버튼 색상 변경
        }
    }

    void UpdateCooldown() //쿨타임 진행
    {
        float remainingTime = cooldownEndTime - Time.time; //남은 시간

        if (remainingTime <= 0) //쿨타임 종료
        {
            EndCooldown(); 
        }
        else
        {
            // 버튼 색상 점점 밝게 변경
            if (myButton.image != null)
            {
                myButton.image.color = Color.Lerp(cooldownColor, activeColor, 1 - (remainingTime / cooldownTime));
            }
        }
    }

    void EndCooldown() //쿨타임 종료
    {
        isOnCooldown = false;

        // 버튼 색상을 원래 색상으로 복원
        if (myButton.image != null)
        {
            myButton.image.color = activeColor;
        }
    }

    public bool IsOnCooldown()
    {
        return isOnCooldown;
    }
}
