using System.Collections;
using UnityEngine;

public class PlayerBurn : MonoBehaviour
{
    public float burnDamage = 5.0f; // 지속 데미지
    public float burnDuration = 10.0f; // 지속 시간
    public GameObject burnEffectPrefab; // 화상 프리팹 프리팹

    private float burnEndTime = -1.0f; // 화상 상태 종료 시간
    private PlayerHP playerHP; 
    private GameObject currentBurnEffect; // 현재 생성된 화상 프리팹

    void Start()
    {
        playerHP = GetComponent<PlayerHP>();
    }

    void Update()
    {
        if (burnEndTime > Time.time) //지속 시간 동안
        {
            playerHP.TakeDamage(burnDamage * Time.deltaTime); //용사에게 피해 가함

          
            if (currentBurnEffect != null)
            {
                currentBurnEffect.transform.position = transform.position; //화상 프리팹 위치를 용사에게 고정
            }
        }
        else if (currentBurnEffect != null)
        {
            // 화상 상태가 끝나면 프리팹을 삭제
            Destroy(currentBurnEffect);
        }
    }

    public void ApplyBurn() //화상 업데이트
    {
        // 화상 시간 업데이트
        burnEndTime = Mathf.Max(burnEndTime, Time.time + burnDuration);

        // 기존의 화상 프리팹가 있는 경우 삭제
        if (currentBurnEffect != null)
        {
            Destroy(currentBurnEffect);
        }

        // 새로운 화상 프리팹 생성
        currentBurnEffect = Instantiate(burnEffectPrefab, transform.position, Quaternion.identity);
    }
}
