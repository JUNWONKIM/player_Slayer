using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public PlayerHP player; // 플레이어의 체력을 관리하는 스크립트
    private CreatureHealth bossHealth; // 보스 체력을 관리하는 스크립트

    void Update()
    {
        CheckGameOver();
    }

    void CheckGameOver()
    {
        if (player != null && player.hp <= 0) // 플레이어의 체력이 0 이하일 경우
        {
            LoadWinScene(); // 승리 씬으로 전환
        }

        if (bossHealth != null && bossHealth.currentHealth <= 0) // 보스가 존재하고 체력이 0 이하일 경우
        {
            LoadLoseScene(); // 패배 씬으로 전환
        }
    }

   

    void LoadWinScene()
    {
        //SceneManager.LoadScene("WinScene"); // 승리 씬으로 전환
    }

    public void LoadLoseScene()
    {
        SceneManager.LoadScene("LoseScene"); // 패배 씬 이름에 맞게 변경
    }
}
