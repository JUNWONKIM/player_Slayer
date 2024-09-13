using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    public GameObject groundTilePrefab; // 땅 타일 프리팹
    public float tileSize = 50f; // 땅 타일의 크기
    public float tileSpacing = 125f; // 타일 간의 간격
    public int viewDistance = 5; // 플레이어 주변에 생성될 타일의 범위
    public Transform tilesParent; // 타일을 자식으로 넣을 부모 오브젝트

    private Dictionary<Vector2, GameObject> terrainTiles = new Dictionary<Vector2, GameObject>();
    private Transform playerTransform;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        GenerateInitialTerrain();
    }

    void Update()
    {
        if (playerTransform == null) return;

        GenerateTerrainAroundPlayer();
        RemoveDistantTiles();
    }

    void GenerateInitialTerrain()
    {
        Vector2 playerPosition = GetCurrentPlayerTileCoord();

        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int z = -viewDistance; z <= viewDistance; z++)
            {
                Vector2 tileCoord = new Vector2(playerPosition.x + x, playerPosition.y + z);
                CreateTileAt(tileCoord);
            }
        }
    }

    void GenerateTerrainAroundPlayer()
    {
        Vector2 playerPosition = GetCurrentPlayerTileCoord();

        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int z = -viewDistance; z <= viewDistance; z++)
            {
                Vector2 tileCoord = new Vector2(playerPosition.x + x, playerPosition.y + z);

                if (!terrainTiles.ContainsKey(tileCoord))
                {
                    CreateTileAt(tileCoord);
                }
            }
        }
    }

    void RemoveDistantTiles()
    {
        List<Vector2> tilesToRemove = new List<Vector2>();
        Vector2 playerPosition = GetCurrentPlayerTileCoord();

        foreach (var tile in terrainTiles)
        {
            if (Vector2.Distance(tile.Key, playerPosition) > viewDistance)
            {
                tilesToRemove.Add(tile.Key);
            }
        }

        foreach (var tileCoord in tilesToRemove)
        {
            Destroy(terrainTiles[tileCoord]);
            terrainTiles.Remove(tileCoord);
        }
    }

    Vector2 GetCurrentPlayerTileCoord()
    {
        // 플레이어의 위치를 기준으로 현재 타일 좌표 계산
        float xCoord = Mathf.Floor(playerTransform.position.x / tileSpacing);
        float zCoord = Mathf.Floor(playerTransform.position.z / tileSpacing);

        return new Vector2(xCoord, zCoord);
    }

    void CreateTileAt(Vector2 tileCoord)
    {
        // 타일의 위치를 고정된 위치로 계산
        Vector3 tilePosition = new Vector3(tileCoord.x * tileSpacing, 0, tileCoord.y * tileSpacing);

        // 타일의 위치와 회전을 고정된 값으로 설정
        GameObject newTile = Instantiate(groundTilePrefab, tilePosition, Quaternion.identity);

        // 타일의 부모를 설정
        newTile.transform.parent = tilesParent;

        newTile.transform.localScale = new Vector3(tileSize, 1, tileSize);

        // 타일 딕셔너리에 추가
        terrainTiles.Add(tileCoord, newTile);
    }
}
