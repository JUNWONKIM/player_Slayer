using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_selectCreature : MonoBehaviour
{
    public Button myButton;
    public int buttonIndex; // 이 버튼의 인덱스 (1부터 4까지)
    private ColorBlock originalColors;
    private CreatureSpawner spawner;

    void Start()
    {
        if (myButton != null)
        {
            // 버튼의 원래 색상 저장
            originalColors = myButton.colors;
        }

        // CreatureSpawner 컴포넌트를 찾습니다.
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

        // 버튼을 실제로 눌렀을 때 실행되는 콜백 함수 호출 (선택사항)
        // myButton.onClick.Invoke();
    }

    void ReleaseButton()
    {
        myButton.colors = originalColors;
    }
}
