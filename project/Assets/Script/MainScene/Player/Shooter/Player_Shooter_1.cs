using System.Collections;
using UnityEngine;

public class Player_Shooter_1 : MonoBehaviour
{
    public static Player_Shooter_1 instance;

    public GameObject bulletPrefab; // �߻�ü ������

    public float fireInterval = 1f; // �߻� ����
    public float detectionRange = 100f; // Ž�� ����
    public float projectileSpeed = 100f; //����ü �ӵ�
    public int projectilesPerFire = 1; // �� ���� �߻��� ����ü ��
    public float burstInterval = 0.1f; // ���� �߻� ����
    public float damageAmount = 1; // ������ ��
    public float volume = 1f;
    public float fireIntervalSlowMultiplier = 2f; // Slow ȿ�� �� �߻� ���� ���

    public AudioClip shootSound; // �߻� ���� Ŭ�� �߰�
    private AudioSource audioSource; 
    [Range(0f, 1f)]
    private float lastFireTime; // ������ �߻� �ð�
    private bool isSlowed = false; // Slow ���� ����


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            audioSource = GetComponent<AudioSource>(); 
            audioSource.volume = volume; 
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    void Update()
    {
        // �ν����Ϳ��� ������ ����Ǿ��� �� AudioSource�� ����
        audioSource.volume = volume;

        if (Time.time - lastFireTime > fireInterval)
        {
            StartCoroutine(FireProjectileBurst());
            lastFireTime = Time.time;
        }

        CheckForSlowObjects(); // Slow �±� üũ
    }

    IEnumerator FireProjectileBurst()
    {
        for (int i = 0; i < projectilesPerFire; i++)
        {
            Shoot();
            if (i < projectilesPerFire - 1)
            {
                yield return new WaitForSeconds(burstInterval);
            }
        }
    }

    void Shoot()
    {
        GameObject closestTarget = null;
        float closestDistance = Mathf.Infinity;

        // Creature�� Boss �±׸� ���� ��ü�� ��� ã��
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Creature");
        foreach (GameObject target in GameObject.FindGameObjectsWithTag("Boss"))
        {
            var newTargets = new GameObject[targets.Length + 1];
            targets.CopyTo(newTargets, 0);
            newTargets[targets.Length] = target;
            targets = newTargets;
        }

        // ���� ����� ��ǥ�� ã��
        foreach (GameObject target in targets)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance < closestDistance && distance <= detectionRange)
            {
                closestTarget = target;
                closestDistance = distance;
            }
        }

        if (closestTarget != null)
        {
            Vector3 targetDirection = closestTarget.transform.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(targetDirection);
            GameObject bullet = Instantiate(bulletPrefab, transform.position, rotation);

            Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
            if (bulletRigidbody != null)
            {
                bulletRigidbody.velocity = targetDirection.normalized * projectileSpeed;
            }

            // �߻� ���� ���
            if (audioSource != null && shootSound != null)
            {
                audioSource.PlayOneShot(shootSound);
            }

            // �浹 ó�� ������Ʈ�� �������� �߰�
            BulletCollisionHandler collisionHandler = bullet.AddComponent<BulletCollisionHandler>();
            collisionHandler.damageAmount = damageAmount;
        }
    }

    private void CheckForSlowObjects()
    {
        GameObject[] slowObjects = GameObject.FindGameObjectsWithTag("Slow");

        if (slowObjects.Length > 0 && !isSlowed)
        {
            fireInterval *= fireIntervalSlowMultiplier; // �߻� ������ ������ ��
            isSlowed = true;
        }
        else if (slowObjects.Length == 0 && isSlowed)
        {
            fireInterval /= fireIntervalSlowMultiplier; // �߻� ������ ������� ����
            isSlowed = false;
        }
    }

    public void IncreaseFireRate(float amount)
    {
        fireInterval /= amount;
        if (fireInterval < 0.1f) fireInterval = 0.1f; // �ּ� �߻� ���� ����
        Debug.Log("����ü �߻� �ӵ� :" + fireInterval);
    }

    public void IncreaseProjectileCount(int amount)
    {
        projectilesPerFire += amount;
        Debug.Log("����ü ���� : " + projectilesPerFire);
    }

    public void IncreaseDamage(float amount)
    {
        damageAmount += amount;
        Debug.Log("����ü ������ : " + damageAmount);
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
