using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class UI_BossHp : MonoBehaviour
{
    public Slider hpSlider; // 체력을 표시할 슬라이더

   
    public void SetBossHp(BossHP bossHP) //보스 hp 설정
    {
        if (bossHP != null && hpSlider != null)
        {
            hpSlider.maxValue = bossHP.maxHP;
            hpSlider.value = bossHP.currentHP;

          
            StartCoroutine(UpdateHpBar(bossHP)); //보스 hp 업데이트
        }
    }

    private IEnumerator UpdateHpBar(BossHP bossHP) //보스 hp 업데이트
    {
        
        while (bossHP != null && bossHP.currentHP > 0) //boss가 살아 있을 시
        {
            hpSlider.value = bossHP.currentHP;
            yield return null; 
        }

       
        hpSlider.gameObject.SetActive(false); //보스가 죽으면 비활성화
    }
}
