using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_startbutton : MonoBehaviour
{
    public UI_Setting uiSetting; // UI_Setting ��ũ��Ʈ�� ���� ���� (�ʿ�� ����)

    void Update()
    {
        // ����â�� Ȱ��ȭ���� �ʾ��� ���� Ű���� �Է��� ó��
        if (uiSetting != null && !uiSetting.IsSettingsPanelActive())
        {
            // ���콺 Ŭ���� ������ Ű���� �Է¸� ����
            if (IsKeyboardInput() && !Input.GetKeyDown(KeyCode.Escape))
            {
                LoadMainScene(); // Ű���� �Է��� �����Ǹ� ���� �� �ε�
            }
        }
    }

    // Ű���� �Է� ���� �Լ�
    bool IsKeyboardInput()
    {
        // Ű���� Ű�� ���ȴ��� üũ (���콺 Ŭ���� ����)
        return Input.anyKeyDown && !(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2));
    }

    void LoadMainScene()
    {
        SceneManager.LoadScene("MainScene"); // MainScene�� ���� ������ ���ԵǾ� �ִ��� Ȯ��
    }
}
