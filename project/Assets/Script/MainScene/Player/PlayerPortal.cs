using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPortal : MonoBehaviour
{
    public GameObject portalPrefab; // ��Ż ������
    public float distanceToSpawnPortal = 40.0f; // ��Ż ���� �Ÿ�
    public float portalOffset = 20.0f; // ��Ż ���� ��ġ
    public float portalHeight = 15.0f; // ��Ż�� y ��ġ

    private GameObject[] portals; // ������ ��Ż�� ����
    private bool portalSpawned = false; // ��Ż ���� ����
    private GameObject boss; 

    void Start()
    {
        portals = new GameObject[8]; //��Ż 8�� �ʱ�ȭ

        boss = GameObject.FindGameObjectWithTag("Boss");
    }

    void Update()
    {
        // ���� ������Ʈ�� ���������� ã��
        if (boss == null)
        {
            boss = GameObject.FindGameObjectWithTag("Boss");
            return;
        }

        // �������� �Ÿ� Ȯ��
        float distanceToBoss = Vector3.Distance(transform.position, boss.transform.position);
              
        if (distanceToBoss <= distanceToSpawnPortal && !portalSpawned)  // �������� �Ÿ��� distanceToSpawnPortal ������ ��
        {
            SpawnPortals(); //��Ż ��ȯ
        }
      
        else if (distanceToBoss > distanceToSpawnPortal && portalSpawned)   // ������ �Ÿ� ������ ������ ��Ż�� ����
        {
            DestroyPortals(); //��Ż ����
        }
        else if (portalSpawned)
        {
            UpdatePortalPositions(); //��Ż ��ġ ������Ʈ
        }
    }

    void SpawnPortals() //��Ż ����
    {
        //��ġ
        Vector3[] directions = new Vector3[] {
            transform.forward,                // ����
            -transform.forward,               // ����
            transform.right,                  // ����
            -transform.right,                 // ����
            (transform.forward + transform.right).normalized,    // �ϵ���
            (transform.forward - transform.right).normalized,    // �ϼ���
            (-transform.forward + transform.right).normalized,   // ������
            (-transform.forward - transform.right).normalized    // ������
        };
        //�ٶ󺸴� ����
        Quaternion[] rotations = new Quaternion[] {
            Quaternion.LookRotation(transform.forward),                // ����
            Quaternion.LookRotation(-transform.forward),               // ����
            Quaternion.LookRotation(transform.right),                  // ����
            Quaternion.LookRotation(-transform.right),                 // ����
            Quaternion.LookRotation((transform.forward + transform.right).normalized),    // �ϵ���
            Quaternion.LookRotation((transform.forward - transform.right).normalized),    // �ϼ���
            Quaternion.LookRotation((-transform.forward + transform.right).normalized),   // ������
            Quaternion.LookRotation((-transform.forward - transform.right).normalized)    // ������
        };

        for (int i = 0; i < directions.Length; i++)
        {
            // ��Ż�� ��ġ�� ���
            Vector3 portalPosition = transform.position + directions[i] * portalOffset;
            portalPosition.y = portalHeight;

            // ȸ��
            portals[i] = Instantiate(portalPrefab, portalPosition, rotations[i]);
        }

        // ��Ż ���� ���� üũ
        portalSpawned = true;
    }

    void DestroyPortals() //��Ż ����
    {
        // ������ ��Ż�� ����
        for (int i = 0; i < portals.Length; i++)
        {
            if (portals[i] != null)
            {
                Destroy(portals[i]);
            }
        }

        // ��Ż ���� ���� üũ
        portalSpawned = false;
    }

    void UpdatePortalPositions() //��Ż ��ġ ������Ʈ
    {
        Vector3[] directions = new Vector3[] {
            transform.forward,                // ����
            -transform.forward,               // ����
            transform.right,                  // ����
            -transform.right,                 // ����
            (transform.forward + transform.right).normalized,    // �ϵ���
            (transform.forward - transform.right).normalized,    // �ϼ���
            (-transform.forward + transform.right).normalized,   // ������
            (-transform.forward - transform.right).normalized    // ������
        };

        for (int i = 0; i < portals.Length; i++)
        {
            if (portals[i] != null)
            {
                // ��Ż�� ���ο� ��ġ�� ���
                Vector3 portalPosition = transform.position + directions[i] * portalOffset;
                portalPosition.y = portalHeight;

                // ��Ż�� ��ġ�� ������Ʈ
                portals[i].transform.position = portalPosition;
                portals[i].transform.rotation = Quaternion.LookRotation(directions[i]);
            }
        }
    }
}
