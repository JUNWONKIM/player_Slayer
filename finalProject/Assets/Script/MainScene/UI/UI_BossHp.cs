using UnityEngine;
using UnityEngine.UI;

public class UI_BossHp : MonoBehaviour
{
    private Image healthBar; // 체력을 표시할 UI 이미지

    void Start()
    {
        healthBar = GetComponent<Image>(); // 현재 GameObject에 부착된 이미지 컴포넌트를 가져옵니다.
    }

    public void SetBossHealth(CreatureHealth bossHealth)
    {
        if (bossHealth != null)
        {
            // 현재 체력을 최대 체력으로 나누어 비율을 계산
            float healthPercentage = bossHealth.currentHealth / bossHealth.maxHealth;
            healthBar.fillAmount = healthPercentage; // UI 이미지의 fillAmount를 업데이트
        }
    }
}
