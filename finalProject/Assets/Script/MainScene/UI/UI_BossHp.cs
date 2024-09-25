using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_BossHp : MonoBehaviour
{
    public Slider healthSlider; // 체력을 표시할 슬라이더

    // 보스의 체력을 설정하고 UI 업데이트
    public void SetBossHealth(BossHP bossHealth)
    {
        if (bossHealth != null && healthSlider != null)
        {
            healthSlider.maxValue = bossHealth.maxHealth;
            healthSlider.value = bossHealth.currentHealth;

            // 지속적으로 보스의 체력 UI를 업데이트하기 위해 코루틴을 시작
            StartCoroutine(UpdateHealthBar(bossHealth));
        }
    }

    private IEnumerator UpdateHealthBar(BossHP bossHealth)
    {
        // 보스가 살아있는 동안 체력바를 업데이트
        while (bossHealth != null && bossHealth.currentHealth > 0)
        {
            healthSlider.value = bossHealth.currentHealth;
            yield return null; // 매 프레임마다 업데이트
        }

        // 보스가 죽으면 체력바를 숨김
        healthSlider.gameObject.SetActive(false);
    }
}
