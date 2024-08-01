using UnityEngine;
using UnityEngine.UI;

public class UI_selectCreature : MonoBehaviour
{
    public Button myButton;
    public int buttonIndex; // 이 버튼의 인덱스 (1부터 4까지)
    public Image cooldownImage; // 쿨타임 표시용 이미지
    public float cooldownTime = 5f; // 버튼의 쿨타임 시간
    public Color activeColor = Color.white; // 버튼 활성화 색상
    public Color cooldownColor = Color.red; // 버튼 쿨타임 색상

    private ColorBlock originalColors;
    private bool isOnCooldown = false;
    private float cooldownEndTime;
    private CreatureSpawner spawner;

    void Start()
    {
        if (myButton != null)
        {
            // 버튼의 원래 색상 저장
            originalColors = myButton.colors;
            // 버튼 클릭 이벤트 설정
            myButton.onClick.AddListener(OnButtonClick);
        }

        // 쿨타임 이미지 설정
        if (cooldownImage != null)
        {
            cooldownImage.fillAmount = 0f;
        }
        spawner = FindObjectOfType<CreatureSpawner>();
        // 시작할 때 버튼 상태를 설정합니다.
        UpdateButtonState();
    }

    void Update()
    {
        if (spawner != null && myButton != null)
        {
            // selectedCreature 값에 따라 버튼 상태를 업데이트합니다.
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

    void PressButton()
    {
        var colors = myButton.colors;
        colors.normalColor = colors.pressedColor;
        myButton.colors = colors;
    }

    void ReleaseButton()
    {
       
            myButton.colors = originalColors;
        
    }

    void OnButtonClick()
    {
        if (!isOnCooldown)
        {
            StartCooldown();
        }
    }

    public void StartCooldown()
    {
        isOnCooldown = true;
        cooldownEndTime = Time.time + cooldownTime;

        // 버튼 색상 변경
        if (myButton.image != null)
        {
            myButton.image.color = cooldownColor;
        }

        // 쿨타임 이미지 초기화
        if (cooldownImage != null)
        {
            cooldownImage.fillAmount = 1f;
        }
    }

    void UpdateCooldown()
    {
        float remainingTime = cooldownEndTime - Time.time;

        if (remainingTime <= 0)
        {
            EndCooldown();
        }
        else
        {
            // 쿨타임 이미지 업데이트
            if (cooldownImage != null)
            {
                cooldownImage.fillAmount = remainingTime / cooldownTime;
            }

            // 버튼 색상 밝게 변경
            if (myButton.image != null)
            {
                myButton.image.color = Color.Lerp(cooldownColor, activeColor, 1 - (remainingTime / cooldownTime));
            }
        }
    }

    void EndCooldown()
    {
        isOnCooldown = false;

        // 버튼 색상 원래대로
        if (myButton.image != null)
        {
            myButton.image.color = activeColor;
        }

        // 쿨타임 이미지 초기화
        if (cooldownImage != null)
        {
            cooldownImage.fillAmount = 0f;
        }
    }

    public bool IsOnCooldown()
    {
        return isOnCooldown;
    }
}
