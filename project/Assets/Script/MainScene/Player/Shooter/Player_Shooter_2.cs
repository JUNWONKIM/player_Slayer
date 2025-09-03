using System.Collections;
using UnityEngine;

public class Player_Shooter_2 : MonoBehaviour
{
    public static Player_Shooter_2 instance;

    public GameObject spawnPrefab; // �Ѿ� ������
    public float spawnInterval = 5f; // �Ѿ� ��ȯ ����
    public float spawnRadius = 20f; // �Ѿ� ��ȯ ���� ������
    public float fixedYPosition = 2.5f; // ������ Y �� ��ġ
    public int projectilesPerFire = 0; // �� ���� �߻��� �߻�ü ��
    public float bulletLifetime = 1.5f; // �Ѿ��� ����
    public float damageAmount = 1f; // �Ѿ��� ������

    private float lastSpawnTime; // ������ ��ȯ �ð�

    private bool isSlowed = false; // Slow ���� ����
    public float spawnIntervalSlowMultiplier = 2f; // Slow ȿ�� �� �߻� ���� ���

    public AudioClip chargeSound; // ù ��° ���� Ŭ��
    public AudioClip explodeSound; // �� ��° ���� Ŭ��
    private AudioSource audioSource; // AudioSource ���� �߰�

    // ���� �ӵ��� ���� ���� ����
    [Range(0.1f, 3f)] public float chargeSoundPitch = 1f; // ù ��° ������ �ӵ� (��ġ)
    [Range(0f, 1f)] public float chargeSoundVolume = 1f; // ù ��° ������ ����
    [Range(0.1f, 3f)] public float explodeSoundPitch = 1f; // �� ��° ������ �ӵ� (��ġ)
    [Range(0f, 1f)] public float explodeSoundVolume = 1f; // �� ��° ������ ����

    void Awake()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>(); // AudioSource ������Ʈ ��������
    }

    void Start()
    {
        InvokeRepeating("SpawnBulletWithExplosion", 0f, spawnInterval);
    }

    void Update()
    {
        CheckForSlowObjects();
    }

    void SpawnBulletWithExplosion()
    {
        for (int i = 0; i < projectilesPerFire; i++)
        {
            Vector3 spawnPosition = transform.position + Random.insideUnitSphere * spawnRadius;
            spawnPosition.y = fixedYPosition;

            GameObject bullet = Instantiate(spawnPrefab, spawnPosition, Quaternion.identity);
            Destroy(bullet, bulletLifetime);

            BulletCollisionHandler collisionHandler = bullet.AddComponent<BulletCollisionHandler>();
            collisionHandler.damageAmount = damageAmount;

            Collider bulletCollider = bullet.GetComponent<Collider>();
            bulletCollider.enabled = false;

            StartCoroutine(EnableColliderAfterDelay(bulletCollider, 1.0f));

            StartCoroutine(PlaySoundsSequentially());
        }
    }

    IEnumerator PlaySoundsSequentially()
    {
        if (audioSource != null)
        {
            // ù ��° ���� ���
            audioSource.clip = chargeSound;
            audioSource.pitch = chargeSoundPitch;
            audioSource.volume = chargeSoundVolume;
            audioSource.Play();
            yield return new WaitForSeconds(audioSource.clip.length / audioSource.pitch);

            // �� ��° ���� ���
            audioSource.clip = explodeSound;
            audioSource.pitch = explodeSoundPitch;
            audioSource.volume = explodeSoundVolume;
            audioSource.Play();
        }
    }

    IEnumerator EnableColliderAfterDelay(Collider collider, float delay)
    {
        yield return new WaitForSeconds(delay);
        collider.enabled = true;
    }

    private void CheckForSlowObjects()
    {
        GameObject[] slowObjects = GameObject.FindGameObjectsWithTag("Slow");

        if (slowObjects.Length > 0 && !isSlowed)
        {
            spawnInterval *= spawnIntervalSlowMultiplier; // �߻� ������ �� ��� �ø�
            isSlowed = true;
        }
        else if (slowObjects.Length == 0 && isSlowed)
        {
            spawnInterval /= spawnIntervalSlowMultiplier; // �߻� ������ ������� ����
            isSlowed = false;
        }
    }

    public void IncreaseProjectileCount(int amount)
    {
        projectilesPerFire += amount;
        Debug.Log("��ź ���� " + projectilesPerFire);
    }

    public void IncreaseDamage(float amount)
    {
        damageAmount += amount;
        Debug.Log("��ź ������ : " + damageAmount);
    }

    public void IncreaseFireRate(float amount)
    {
        spawnInterval /= amount;
        if (spawnInterval < 0.1f) spawnInterval = 0.1f;
        Debug.Log("��ź �߻� �ӵ� :" + spawnInterval);
    }

    public class BulletCollisionHandler : MonoBehaviour
    {
        public float damageAmount;
        private Player_Shooter_2 shooterInstance;

        public BulletCollisionHandler(Player_Shooter_2 shooterInstance)
        {
            this.shooterInstance = shooterInstance;
        }

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
