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

    void CheckGameOver()// 게임오버 체크
    {
        if (player != null && player.hp <= 0) // 용사의 체력이 0일 경우
        {
            LoadWinScene(); // 승리 씬으로 전환
        }

        if (bossHp != null && bossHp.currentHp <= 0) // 보스의 체력이 0일 경우 & 보스가 존재할 경우
        {
            LoadLoseScene(); // 패배 씬으로 전환
        }
    }

   

    void LoadWinScene()
    {
        SceneManager.LoadScene("WinScene"); // 승리 씬 전환
    }

    public void LoadLoseScene()
    {
        SceneManager.LoadScene("LoseScene"); // 패배 씬 전환
    }
}
