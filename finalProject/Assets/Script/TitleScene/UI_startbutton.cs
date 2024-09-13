using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_startbutton : MonoBehaviour
{
    public Text startText; // Text UI 요소에 대한 참조
    public UI_Setting uiSetting; // UI_Setting 스크립트에 대한 참조

    void Start()
    {
        startText.text = "키를 눌러 시작하세요";
    }

    void Update()
    {
        // 설정창이 활성화되지 않았을 때만 키보드 입력을 처리
        if (uiSetting != null && !uiSetting.IsSettingsPanelActive())
        {
            if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Escape))
            {
                LoadMainScene(); // 키보드 입력이 감지되면 메인 씬 로드
            }
        }
    }

    void LoadMainScene()
    {
        SceneManager.LoadScene("MainScene"); // MainScene이 빌드 설정에 포함되어 있는지 확인
    }
}
