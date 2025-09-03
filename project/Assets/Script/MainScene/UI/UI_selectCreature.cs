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
    public Color selectedColor = Color.green; // 버튼 선택 색상

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
            PressButton(); // 선택된 버튼은 초록색으로 변경
        }
        else
        {
            if (!isOnCooldown)
            {
                ReleaseButton(); // 선택되지 않았고 쿨타임이 없을 때 원래 상태로
            }
        }
    }

    void PressButton() // 버튼이 선택되었을 때
    {
        if (myButton.image != null)
        {
            myButton.image.color = selectedColor; // 선택된 상태에서 초록색으로 변경
        }
    }

    void ReleaseButton() // 버튼이 해제되었을 때
    {
        if (myButton.image != null && !isOnCooldown)
        {
            myButton.image.color = activeColor; // 기본 활성화 상태로 복원
        }
    }

    void OnButtonClick() // 버튼 클릭 시 쿨타임 시작
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
            myButton.image.color = cooldownColor; // 쿨타임 동안 빨간색으로 변경
        }
    }

    void UpdateCooldown() // 쿨타임 진행 중
    {
        float remainingTime = cooldownEndTime - Time.time; // 남은 시간 계산

        if (remainingTime <= 0) // 쿨타임이 끝났을 때
        {
            EndCooldown();
        }
        else
        {
            // 쿨타임 동안 색상이 점차 바뀌도록 설정
            if (myButton.image != null)
            {
                myButton.image.color = Color.Lerp(cooldownColor, activeColor, 1 - (remainingTime / cooldownTime));
            }
        }
    }

    void EndCooldown() // 쿨타임 종료 시
    {
        isOnCooldown = false;

        // 쿨타임이 끝나면 원래 상태로 복원
        if (myButton.image != null)
        {
            myButton.image.color = activeColor;
        }

        // 만약 버튼이 선택된 상태라면, 선택 색상으로 돌아가게 처리
        if (spawner.selectedCreature == buttonIndex)
        {
            PressButton();
        }
    }

    public bool IsOnCooldown()
    {
        return isOnCooldown;
    }
}
