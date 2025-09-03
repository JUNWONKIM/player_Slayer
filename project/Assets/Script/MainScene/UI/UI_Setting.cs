using UnityEngine;
using UnityEngine.UI;

public class UI_Setting : MonoBehaviour
{
    public GameObject settingsPanel;  // ����â �г�
    public GameObject keyInfoPanel;   // Ű ���� �г�
    private bool isPaused = false;
    private bool isKeyInfoPanelActive = false; 

    void Update()
    {
       
        if (Input.GetKeyDown(KeyCode.Escape))//ESC �Է� ��
        {
            if (isKeyInfoPanelActive)
            {
                // Ű ���� �г��� ���� ���� �� ESC�� ������ ����â���� ���ư�
                CloseKeyInfoPanelAndOpenSettings();
            }
            else if (isPaused)
            {
                // ����â�� ���� ���� �� ESC�� ������ ������ �簳
                ResumeGame();
            }
            else
            {
                // ������ �Ͻ��������� ���� ���¿����� ESC�� ������ ����â�� ��
                PauseGame();
            }
        }
    }

    public void ToggleSettingsPanel() //���� ��ư ��� ����
    {
        if (isPaused)
        {
            ResumeGame(); // ����â �ݱ�
        }
        else
        {
            PauseGame(); // ����â ����
        }
    }

    void PauseGame() //����â ����
    {
        settingsPanel.SetActive(true);  // ����â Ȱ��ȭ
        keyInfoPanel.SetActive(false);  // Ű ���� �г� ��Ȱ��ȭ
        Time.timeScale = 0f;  // ���� �Ͻ� ����
        isPaused = true;
        isKeyInfoPanelActive = false;
    }

    public void ResumeGame() //���� �簳
    {
        settingsPanel.SetActive(false);  // ����â ��Ȱ��ȭ
        Time.timeScale = 1f;  // ���� �簳
        isPaused = false;
        isKeyInfoPanelActive = false;
    }

    public bool IsSettingsPanelActive()  // ����â�� Ȱ��ȭ�Ǿ� �ִ��� Ȯ��
    {
        return settingsPanel.activeSelf; 
    }

    public void OnSettingsButtonClick() //���� ��ư
    {
        ToggleSettingsPanel();  
    }

    public void OpenKeyInfoPanel() //Ű ���� �г� ����
    {
        keyInfoPanel.SetActive(true);  // Ű ���� �г� Ȱ��ȭ
        settingsPanel.SetActive(false);  // ����â ��Ȱ��ȭ
        isKeyInfoPanelActive = true;
    }

    public void CloseKeyInfoPanelAndOpenSettings() //Ű ���� �г� ��Ȱ��ȭ, ����â Ȱ��ȭ
    {
        keyInfoPanel.SetActive(false);  // Ű ���� �г� ��Ȱ��ȭ
        settingsPanel.SetActive(true);  // ����â �ٽ� Ȱ��ȭ
        isKeyInfoPanelActive = false;
    }

    public void OnQuitButtonClick() //���� ���� ��ư
    {
        Application.Quit();  // ���� ����
    }
}
