using System.Collections;
using UnityEngine;

public class PlayerBurn : MonoBehaviour
{
    public float burnDamage = 5.0f; // ���� ������
    public float burnDuration = 10.0f; // ���� �ð�
    public GameObject burnEffectPrefab; // ȭ�� ������ ������

    private float burnEndTime = -1.0f; // ȭ�� ���� ���� �ð�
    private PlayerHP playerHP; 
    private GameObject currentBurnEffect; // ���� ������ ȭ�� ������

    void Start()
    {
        playerHP = GetComponent<PlayerHP>();
    }

    void Update()
    {
        if (burnEndTime > Time.time) //���� �ð� ����
        {
            playerHP.TakeDamage(burnDamage * Time.deltaTime); //��翡�� ���� ����

          
            if (currentBurnEffect != null)
            {
                currentBurnEffect.transform.position = transform.position; //ȭ�� ������ ��ġ�� ��翡�� ����
            }
        }
        else if (currentBurnEffect != null)
        {
            // ȭ�� ���°� ������ �������� ����
            Destroy(currentBurnEffect);
        }
    }

    public void ApplyBurn() //ȭ�� ������Ʈ
    {
        // ȭ�� �ð� ������Ʈ
        burnEndTime = Mathf.Max(burnEndTime, Time.time + burnDuration);

        // ������ ȭ�� �����հ� �ִ� ��� ����
        if (currentBurnEffect != null)
        {
            Destroy(currentBurnEffect);
        }

        // ���ο� ȭ�� ������ ����
        currentBurnEffect = Instantiate(burnEffectPrefab, transform.position, Quaternion.identity);
    }
}
