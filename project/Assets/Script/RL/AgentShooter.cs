using UnityEngine;

public class AgentShooter : MonoBehaviour
{
    public GameObject bulletPrefab;         // 투사체 프리팹
    public float fireInterval = 1f;         // 발사 간격
    public float detectionRange = 50f;      // 크리처 탐지 범위
    public float bulletSpeed = 50f;         // 투사체 속도
    public float bulletDamage = 1f;         // 데미지

    private float lastFireTime = 0f;

    void Update()
    {
        if (Time.time - lastFireTime >= fireInterval)
        {
            TryShootNearestTarget();
            lastFireTime = Time.time;
        }
    }

    void TryShootNearestTarget()
    {
        GameObject[] creatures = GameObject.FindGameObjectsWithTag("Creature");

        GameObject closest = null;
        float closestDist = Mathf.Infinity;

        foreach (var c in creatures)
        {
            float dist = Vector3.Distance(transform.position, c.transform.position);
            if (dist < detectionRange && dist < closestDist)
            {
                closest = c;
                closestDist = dist;
            }
        }

        if (closest != null)
        {
            ShootAt(closest);
        }
    }

    void ShootAt(GameObject target)
    {
        Vector3 dir = (target.transform.position - transform.position).normalized;
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.LookRotation(dir));

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = dir * bulletSpeed;
        }

        BulletDamage bulletDamageScript = bullet.AddComponent<BulletDamage>();
        bulletDamageScript.damage = bulletDamage;
    }

    public class BulletDamage : MonoBehaviour
    {
        public float damage = 1f;
        public float lifeTime = 5f;

        private void Start()
        {
            Destroy(gameObject, lifeTime); // ✅ 7초 후 자동 제거
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Creature"))
            {
                CreatureHp hp = other.GetComponent<CreatureHp>();
                if (hp != null)
                {
                    hp.TakeDamage(damage); // ✅ 데미지 적용
                    Destroy(gameObject);   // ✅ 충돌 시 즉시 제거
                }
            }
        }
    }
}