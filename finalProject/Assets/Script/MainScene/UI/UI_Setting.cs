using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // EventSystem을 사용하기 위해 추가

public class UI_Setting : MonoBehaviour
{
    public GameObject settingsPanel; // 설정창 패널
    private bool isPaused = false;

    void Update()
    {
        // ESC 키를 누르면 설정창을 토글
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame(); // 설정창 닫기
            }
            else
            {
                PauseGame(); // 설정창 열기
            }
        }
    }

    void PauseGame()
    {
        settingsPanel.SetActive(true); // 설정창 활성화
        Time.timeScale = 0f; // 게임 일시 정지
        isPaused = true;

        // 모든 입력을 차단
        EventSystem.current.sendNavigationEvents = false;
    }

    public void ResumeGame()
    {
        settingsPanel.SetActive(false); // 설정창 비활성화
        Time.timeScale = 1f; // 게임 재개
        isPaused = false;

        // 입력 이벤트를 다시 활성화
        EventSystem.current.sendNavigationEvents = true;
    }

    public void QuitGame()
    {
        Application.Quit(); // 게임 종료
    }

    public bool IsSettingsPanelActive()
    {
        return settingsPanel.activeSelf; // 설정창이 활성화되어 있는지 확인
    }

}
