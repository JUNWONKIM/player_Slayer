using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_startbutton : MonoBehaviour
{
    public UI_Setting uiSetting; // UI_Setting 스크립트에 대한 참조 (필요시 연결)

    void Update()
    {
        // 설정창이 활성화되지 않았을 때만 키보드 입력을 처리
        if (uiSetting != null && !uiSetting.IsSettingsPanelActive())
        {
            // 마우스 클릭을 제외한 키보드 입력만 감지
            if (IsKeyboardInput() && !Input.GetKeyDown(KeyCode.Escape))
            {
                LoadMainScene(); // 키보드 입력이 감지되면 메인 씬 로드
            }
        }
    }

    // 키보드 입력 감지 함수
    bool IsKeyboardInput()
    {
        // 키보드 키가 눌렸는지 체크 (마우스 클릭은 제외)
        return Input.anyKeyDown && !(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2));
    }

    void LoadMainScene()
    {
        SceneManager.LoadScene("MainScene"); // MainScene이 빌드 설정에 포함되어 있는지 확인
    }
}
