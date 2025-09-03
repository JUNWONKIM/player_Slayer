using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems; 

public class CreatureSpawner : MonoBehaviour
{
    public GameObject playerPrefab; // ��� ������
    public GameObject[] creaturePrefabs; // ũ���� ������
    public float spawnRange = 30f; //�ּ� ��ȯ ����
    public int selectedCreature = 1; // ���õ� ũ����, ó���� 1������ ����
    public UI_selectCreature[] uiButtons; // UI ��ư 
    public GraphicRaycaster graphicRaycaster; //ĵ���� ����ĳ����
    public EventSystem eventSystem; 
    public UI_Setting uiSetting;  // UI_Setting 

    private const string GroundTag = "ground";
    private const int LeftMouseButton = 0;
    private LineRenderer lineRenderer;

    private PlayerHP playerHP; // �÷��̾� ü�� ��ũ��Ʈ

    void Start()
    {
        // ��ȯ ���� ���� ǥ��
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 360;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.useWorldSpace = false;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;

        // �÷��̾��� ü�� ��ũ��Ʈ ����
        playerHP = playerPrefab.GetComponent<PlayerHP>();

        // ���õ� ũ��ó�� 1������ ���� ����
        selectedCreature = 1;
    }

    void Update()
    {
        HandleCreatureSelection();

        // ����â�� Ȱ��ȭ�� ��� ��ȯ ����
        if (uiSetting != null && !uiSetting.IsSettingsPanelActive())
        {
            // UI Ŭ���� �ƴ϶�� ���콺 Ŭ�� ó��
            if (Input.GetMouseButtonDown(LeftMouseButton) && !IsPointerOverUIElement())
            {
                if (selectedCreature == 4)
                {
                    SpawnRandomCreature();
                }
                else
                {
                    SpawnSelectedCreature(selectedCreature - 1);
                }
            }
        }

        DrawSpawnRange();
    }

    public bool IsWithinSpawnRange(Vector3 position) //���� ���� ���� Ȯ��
    {
        Vector3 playerPosition = playerPrefab.transform.position;
        return Vector3.Distance(playerPosition, position) <= spawnRange;
    }

    void HandleCreatureSelection() // ��ȯ�� ũ���� ����
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedCreature = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedCreature = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            // �÷��̾� ü���� 70% �̸��� ���� 3�� ũ���� ���� ����
            if (playerHP.hp <= playerHP.max_hp * 0.7f)
            {
                selectedCreature = 3;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            // �÷��̾� ü���� 50% �̸��� ���� 4�� ũ���� ���� ����
            if (playerHP.hp <= playerHP.max_hp * 0.5f)
            {
                selectedCreature = 4;
            }
        }
    }

    void DrawSpawnRange() //���� ���� ǥ��
    {
        Vector3 playerPosition = playerPrefab.transform.position;
        Vector3 center = new Vector3(playerPosition.x, 1f, playerPosition.z);

        for (int i = 0; i < 360; i++) //LinRenderer ���
        {
            float rad = Mathf.Deg2Rad * i;
            float x = center.x + Mathf.Cos(rad) * spawnRange;
            float z = center.z + Mathf.Sin(rad) * spawnRange;
            lineRenderer.SetPosition(i, new Vector3(x, center.y, z));
        }
    }

    void SpawnSelectedCreature(int index) //���õ� ũ���� ��ȯ
    {
        if (index < creaturePrefabs.Length)
        {
            if (!uiButtons[index].IsOnCooldown())
            {
                GameObject creatureToSpawn = creaturePrefabs[index];
                SpawnCreature(creatureToSpawn);
                TriggerButtonCooldown(index); //��ư ��Ÿ�� ����
            }
        }
    }

    void SpawnRandomCreature() //���� ���� ��ȯ
    {
        int randomIndex = Random.Range(3, creaturePrefabs.Length);
        if (!uiButtons[randomIndex].IsOnCooldown())
        {
            GameObject creatureToSpawn = creaturePrefabs[randomIndex];
            SpawnCreature(creatureToSpawn);
            TriggerButtonCooldown(randomIndex);//��ư ��Ÿ�� ����
        }
    }

    void SpawnCreature(GameObject creaturePrefab) //ũ���� ��ȯ
    {
        Vector3 playerPosition = playerPrefab.transform.position;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.CompareTag(GroundTag)) //Ŭ���� ������Ʈ�� Ground�� ���
        {
            Vector3 spawnPosition = hit.point;

            if (Vector3.Distance(new Vector3(playerPosition.x, 0f, playerPosition.z), new Vector3(spawnPosition.x, 0f, spawnPosition.z)) > spawnRange) //���� ������ ���
            {
                if (creaturePrefab != null)
                {
                    Instantiate(creaturePrefab, spawnPosition, Quaternion.identity);
                }
            }
        }
    }

    void TriggerButtonCooldown(int index) //UI ��ٿ� ����
    {
        if (index < uiButtons.Length)
        {
            uiButtons[index].StartCooldown();
        }
    }

    bool IsPointerOverUIElement() //����â�� Ŭ���� ��� ��ȯ x
    {
        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerData, results);

        return results.Count > 0;
    }
}
