using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Setting : MonoBehaviour
{
    public GameObject settingsPanel;  // 설정창 패널
    public GameObject keyInfoPanel;   // 추가된 키 정보 패널
    private bool isPaused = false;
    private bool isKeyInfoPanelActive = false; // 키 정보 패널의 상태

    void Update()
    {
        // ESC 키 입력 처리
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isKeyInfoPanelActive)
            {
                // 키 정보 패널이 열려 있을 때 ESC를 누르면 설정창으로 돌아감
                CloseKeyInfoPanelAndOpenSettings();
            }
            else if (isPaused)
            {
                // 설정창이 열려 있을 때 ESC를 누르면 게임을 재개
                ResumeGame();
            }
            else
            {
                // 게임이 일시정지되지 않은 상태에서는 ESC를 누르면 설정창을 연다
                PauseGame();
            }
        }
    }

    public void ToggleSettingsPanel()
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

    void PauseGame()
    {
        settingsPanel.SetActive(true);  // 설정창 활성화
        keyInfoPanel.SetActive(false);  // 키 정보 패널 비활성화
        Time.timeScale = 0f;  // 게임 일시 정지
        isPaused = true;
        isKeyInfoPanelActive = false;

        // 모든 입력을 차단
        EventSystem.current.sendNavigationEvents = false;
    }

    public void ResumeGame()
    {
        settingsPanel.SetActive(false);  // 설정창 비활성화
        Time.timeScale = 1f;  // 게임 재개
        isPaused = false;
        isKeyInfoPanelActive = false;

        // 입력 이벤트를 다시 활성화
        EventSystem.current.sendNavigationEvents = true;
    }

    public void QuitGame()
    {
        Application.Quit();  // 게임 종료
    }

    public bool IsSettingsPanelActive()
    {
        return settingsPanel.activeSelf;  // 설정창이 활성화되어 있는지 확인
    }

    // 설정 패널 토글 버튼
    public void OnSettingsButtonClick()
    {
        ToggleSettingsPanel();  // 버튼으로 설정창 토글
    }

    // 키 정보 패널을 여는 버튼
    public void OnKeyInfoPanelButtonClick()
    {
        OpenKeyInfoPanel();
    }

    // 키 정보 패널 열기
    public void OpenKeyInfoPanel()
    {
        keyInfoPanel.SetActive(true);  // 키 정보 패널 활성화
        settingsPanel.SetActive(false);  // 설정창 비활성화
        isKeyInfoPanelActive = true;
    }

    // 키 정보 패널 닫고 설정창으로 돌아가기
    public void CloseKeyInfoPanelAndOpenSettings()
    {
        keyInfoPanel.SetActive(false);  // 키 정보 패널 비활성화
        settingsPanel.SetActive(true);  // 설정창 다시 활성화
        isKeyInfoPanelActive = false;
    }

    // 게임 종료 버튼 함수
    public void OnQuitButtonClick()
    {
        QuitGame();  // 게임 종료
    }
}
