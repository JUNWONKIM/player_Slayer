using System.Collections;
using UnityEngine;

public class PlayerBurn : MonoBehaviour
{
    public float burnDamage = 5.0f; // 화상 상태에서 주는 데미지
    public float burnDuration = 10.0f; // 화상 상태 지속 시간
    public GameObject burnEffectPrefab; // 화상 이펙트 프리팹

    private float burnEndTime = -1.0f; // 화상 상태 종료 시간
    private PlayerHP playerHP; // 플레이어의 체력 스크립트
    private GameObject currentBurnEffect; // 현재 생성된 화상 이펙트

    void Start()
    {
        playerHP = GetComponent<PlayerHP>();
    }

    void Update()
    {
        if (burnEndTime > Time.time)
        {
            // 화상 상태인 동안
            playerHP.TakeDamage(burnDamage * Time.deltaTime);

            // 화상 이펙트가 현재 플레이어의 위치와 일치하도록 업데이트
            if (currentBurnEffect != null)
            {
                currentBurnEffect.transform.position = transform.position;
            }
        }
        else if (currentBurnEffect != null)
        {
            // 화상 상태가 끝나면 이펙트를 삭제
            Destroy(currentBurnEffect);
        }
    }

    public void ApplyBurn()
    {
        // 화상 상태 업데이트
        burnEndTime = Mathf.Max(burnEndTime, Time.time + burnDuration);

        // 기존의 화상 이펙트가 있는 경우 삭제
        if (currentBurnEffect != null)
        {
            Destroy(currentBurnEffect);
        }

        // 새로운 화상 이펙트 생성
        currentBurnEffect = Instantiate(burnEffectPrefab, transform.position, Quaternion.identity);
    }
}
