using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool instance; // �̱��� �ν��Ͻ�
    public GameObject[] bulletPrefabs; // ����ü ������
    public int poolSizePerBulletType = 10; 

    private Dictionary<GameObject, List<GameObject>> bulletPools; // ����ü Ǯ ��ųʸ�

    void Awake()
    {
        instance = this;
        InitializePools();
    }


    void InitializePools() //����ü Ǯ ����
    {
        bulletPools = new Dictionary<GameObject, List<GameObject>>();

        foreach (GameObject bulletPrefab in bulletPrefabs)
        {
            List<GameObject> pool = new List<GameObject>();

            for (int i = 0; i < poolSizePerBulletType; i++)
            {
                GameObject bullet = Instantiate(bulletPrefab);
                bullet.SetActive(false);
                pool.Add(bullet);
            }

            bulletPools.Add(bulletPrefab, pool);
        }
    }

  
    public GameObject GetBulletFromPool(GameObject bulletPrefab)  // ��� ������ ����ü ������Ʈ ��ȯ
    {
        if (!bulletPools.ContainsKey(bulletPrefab))
        {
            Debug.LogError("Bullet prefab is not in the pool.");
            return null;
        }

        List<GameObject> pool = bulletPools[bulletPrefab];
        foreach (GameObject bullet in pool)
        {
            if (!bullet.activeInHierarchy)
            {
                bullet.SetActive(true);
                return bullet;
            }
        }

        // ��� ������ ����ü�� ������ Ǯ�� �߰� �����Ͽ� ��ȯ
        GameObject newBullet = Instantiate(bulletPrefab);
        pool.Add(newBullet);
        return newBullet;
    }

    // ����ü ������Ʈ�� Ǯ�� ��ȯ
    public void ReturnBulletToPool(GameObject bullet)
    {
        bullet.SetActive(false);
    }
}
