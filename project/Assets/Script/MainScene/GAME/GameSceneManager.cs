using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public PlayerHP player; 
    private CreatureHp bossHp; 

    void Update()
    {
        CheckGameOver();
    }

    void CheckGameOver()// ���ӿ��� üũ
    {
        if (player != null && player.hp <= 0) // ����� ü���� 0�� ���
        {
            LoadWinScene(); // �¸� ������ ��ȯ
        }

        if (bossHp != null && bossHp.currentHp <= 0) // ������ ü���� 0�� ��� & ������ ������ ���
        {
            LoadLoseScene(); // �й� ������ ��ȯ
        }
    }

   

    void LoadWinScene()
    {
        SceneManager.LoadScene("WinScene"); // �¸� �� ��ȯ
    }

    public void LoadLoseScene()
    {
        SceneManager.LoadScene("LoseScene"); // �й� �� ��ȯ
    }
}
