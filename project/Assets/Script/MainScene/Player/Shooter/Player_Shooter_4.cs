using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Shooter_4 : MonoBehaviour
{
    public static Player_Shooter_4 instance;

    public GameObject bulletPrefab; // źȯ ������
    public Transform firePoint; // źȯ �߻� ��ġ
    public float fireInterval = 1f; // �߻� ���� (�� ����)

    public float bulletSpeed = 20f; // źȯ �ӵ�
    public int bulletCount = 0; // �Ѿ� ��

    public float damageAmount = 10; // ������ ��
    public float lifetime = 2f; // źȯ�� ���� (��)

    private Transform playerTransform; // �÷��̾� Ʈ������

    private bool isSlowed = false; // Slow ���� ����
    public float fireIntervalSlowMultiplier = 2f; // Slow ȿ�� �� �߻� ���� ���

    public AudioClip fireSound; // �߻� ���� Ŭ��
    private AudioSource audioSource; // AudioSource ���� �߰�

    // ���� �ӵ��� ���� ���� ����
    [Range(0.1f, 3f)] public float fireSoundPitch = 1f; // ������ �ӵ� (��ġ)
    [Range(0f, 1f)] public float fireSoundVolume = 1f; // ������ ����

    void Awake()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>(); // AudioSource ������Ʈ ��������
    }

    void Start()
    {
        // Player �±��� ������Ʈ�� ã��
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player �±׸� ���� ������Ʈ�� ã�� �� �����ϴ�.");
        }

        // ���� �ð����� Shoot �޼��� ȣ��
        InvokeRepeating("Shoot", 0f, fireInterval);
    }

    void Update()
    {
        CheckForSlowObjects();
    }

    void Shoot()
    {
        if (playerTransform == null || bulletCount <= 0)
        {
            return; // Player ������Ʈ�� ã�� ���ϰų� bulletCount�� 0 ������ ��� �߻����� ����
        }

        // �߻� ���� ���
        PlayFireSound();

        for (int i = 0; i < bulletCount; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            // źȯ Ȯ�� ����
            Quaternion spreadRotation = Quaternion.Euler(Random.Range(-30f, 30f), Random.Range(-30f, 30f), 0);
            Vector3 spreadDirection = spreadRotation * playerTransform.forward;

            // ���� ���Ϳ��� y �� ����
            spreadDirection.y = Mathf.Clamp(spreadDirection.y, 0.1f, 1f); // y ���� ������ �������� �ʵ��� Ŭ����

            Vector3 direction = spreadDirection.normalized;

            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.velocity = direction * bulletSpeed;

            // źȯ �ı��� ��ũ��Ʈ���� ���� ����
            Destroy(bullet, lifetime);

            // �浹 ó�� ������Ʈ�� �������� �߰�
            BulletCollisionHandler collisionHandler = bullet.AddComponent<BulletCollisionHandler>();
            collisionHandler.damageAmount = damageAmount;
        }
    }

    void PlayFireSound()
    {
        if (audioSource != null && fireSound != null)
        {
            audioSource.clip = fireSound;
            audioSource.pitch = fireSoundPitch;
            audioSource.volume = fireSoundVolume;
            audioSource.Play();
        }
    }

    private void CheckForSlowObjects()
    {
        GameObject[] slowObjects = GameObject.FindGameObjectsWithTag("Slow");

        if (slowObjects.Length > 0 && !isSlowed)
        {
            fireInterval *= fireIntervalSlowMultiplier; // �߻� ������ �� ��� �ø�
            isSlowed = true;
        }
        else if (slowObjects.Length == 0 && isSlowed)
        {
            fireInterval /= fireIntervalSlowMultiplier; // �߻� ������ ������� ����
            isSlowed = false;
        }
    }

    public void IncreaseBulletCount(int amount)
    {
        bulletCount += amount;
        Debug.Log("���� ���� : " + bulletCount);
    }

    public void IncreaseDamage(float amount)
    {
        damageAmount += amount;
        Debug.Log("���� ������ : " + damageAmount);
    }

    public void IncreaseFireRate(float amount)
    {
        fireInterval /= amount;
        if (fireInterval < 0.1f) fireInterval = 0.1f; // �ּ� �߻� ���� ����
        Debug.Log("���� �߻� �ӵ� :" + fireInterval);
    }

    public class BulletCollisionHandler : MonoBehaviour
    {
        public float damageAmount;

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Creature") || other.gameObject.CompareTag("Boss"))
            {
                // CreatureHp�� Mummy ������Ʈ ��� üũ
                CreatureHp enemyHealth = other.gameObject.GetComponent<CreatureHp>();
                BossHP BossHealth = other.gameObject.GetComponent<BossHP>();
                Mummy enemyHealth2 = other.gameObject.GetComponent<Mummy>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damageAmount);
                    Destroy(gameObject);
                }

                if (enemyHealth2 != null)
                {
                    enemyHealth2.TakeDamage(damageAmount);
                    Destroy(gameObject);
                }

                if (BossHealth != null)
                {
                    BossHealth.TakeDamage(damageAmount);
                    Destroy(gameObject);
                }
            }
        }
    }
}
