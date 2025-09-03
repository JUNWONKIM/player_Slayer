using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    public GameObject groundTilePrefab; // �� ������
    public float tileSize = 50f; //ũ��
    public float tileSpacing = 125f; // Ÿ�� ���� ����
    public int viewDistance = 5; // ��� �ֺ��� ������ Ÿ���� ����
    public Transform tilesParent; // Ÿ���� �ڽ����� ���� �θ� ������Ʈ

    private Dictionary<Vector2, GameObject> terrainTiles = new Dictionary<Vector2, GameObject>(); //��ǥ�� ������Ʈ ���� ��ųʸ�
    private Transform playerTransform;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        GenerateInitialTerrain(); //���� �� �� ����
    }

    void Update()
    {
        if (playerTransform == null) return; //��簡 �������� ���� �� ����

        GenerateTerrainAroundPlayer(); //��� �ֺ� �� ����
        RemoveDistantTiles();// �־��� Ÿ�� ����
    }

    void GenerateInitialTerrain() //�ʱ� �� ����
    {
        Vector2 playerPosition = GetCurrentPlayerTileCoord(); //��� ��ǥ Ȯ��

        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int z = -viewDistance; z <= viewDistance; z++)
            {
                Vector2 tileCoord = new Vector2(playerPosition.x + x, playerPosition.y + z);
                CreateTileAt(tileCoord);
            }
        }
    }

    void GenerateTerrainAroundPlayer() //��� �ֺ� Ÿ�� ����
    {
        Vector2 playerPosition = GetCurrentPlayerTileCoord();

        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int z = -viewDistance; z <= viewDistance; z++)
            {
                Vector2 tileCoord = new Vector2(playerPosition.x + x, playerPosition.y + z); //���ο� Ÿ�� ��ǥ ����

                if (!terrainTiles.ContainsKey(tileCoord)) //�ش� ��ǥ�� Ÿ���� ������
                {
                    CreateTileAt(tileCoord); //Ÿ�� ����
                }
            }
        }
    }

    void RemoveDistantTiles()// �־��� Ÿ�� ����
    {
        List<Vector2> tilesToRemove = new List<Vector2>();
        Vector2 playerPosition = GetCurrentPlayerTileCoord();

        foreach (var tile in terrainTiles)
        {
            if (Vector2.Distance(tile.Key, playerPosition) > viewDistance) //���� �־��� Ÿ�� Ȯ��
            {
                tilesToRemove.Add(tile.Key);//������ Ÿ�� �迭�� �߰�
            }
        }

        foreach (var tileCoord in tilesToRemove)
        {
            Destroy(terrainTiles[tileCoord]); //������ Ÿ�� �迭 ����
            terrainTiles.Remove(tileCoord);//��ųʸ����� ����
        }
    }

    Vector2 GetCurrentPlayerTileCoord() //���� ��簡 ��ġ�� Ÿ�� ��ǥ ���
    {
        //��ǥ�� Ÿ�� ũ�⿡ �°� ����
        float xCoord = Mathf.Floor(playerTransform.position.x / tileSpacing);
        float zCoord = Mathf.Floor(playerTransform.position.z / tileSpacing);

        return new Vector2(xCoord, zCoord);
    }

    void CreateTileAt(Vector2 tileCoord) //Ÿ�� ����
    {
        // Ÿ���� ��ġ�� ������ ��ġ�� ���
        Vector3 tilePosition = new Vector3(tileCoord.x * tileSpacing, 0, tileCoord.y * tileSpacing);

        // Ÿ�� ��ġ�� ȸ���� ����
        GameObject newTile = Instantiate(groundTilePrefab, tilePosition, Quaternion.identity);

        // Ÿ���� �θ� ����
        newTile.transform.parent = tilesParent;

        newTile.transform.localScale = new Vector3(tileSize, 1, tileSize);

        // Ÿ�� ��ųʸ��� �߰�
        terrainTiles.Add(tileCoord, newTile);
    }
}
