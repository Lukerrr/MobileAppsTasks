using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class WorldGenerator : MonoBehaviour
{

    class WorldArea
    {
        public Vector2Int pos1;
        public Vector2Int pos2;

        public WorldArea(int x, int y)
        {
            // Make sure x <= y
            if (x > y)
            {
                int t = y;
                y = x;
                x = t;
            }

            pos1 = new Vector2Int(x, x);
            pos2 = new Vector2Int(y, y);
        }

        public WorldArea(Vector2Int v)
        {
            // Make sure x <= y
            if (v.x > v.y)
            {
                int t = v.y;
                v.y = v.x;
                v.x = t;
            }

            pos1 = new Vector2Int(v.x, v.x);
            pos2 = new Vector2Int(v.y, v.y);
        }

        public WorldArea(Vector2Int v1, Vector2Int v2)
        {
            // Make sure v1 <= v2
            if (v1.x > v2.x)
            {
                int t = v2.x;
                v2.x = v1.x;
                v1.x = t;
            }

            if (v1.y > v2.y)
            {
                int t = v2.y;
                v2.y = v1.y;
                v1.y = t;
            }

            pos1 = v1;
            pos2 = v2;
        }
    }

    public RestartUi restartUi;

    public GameObject groundObject;
    public GameObject towerGroundObject;
    public GameObject towerAreaObject;
    public GameObject borderWallObject;

    public WallGenerator wallGenStrong;
    public WallGenerator wallGenWeak;

    public GameObject playerObject;
    public GameObject enemyObject;

    public List<GameObject> forestObjects;

    private List<WorldArea> enemySpawnAreas;
    private WorldArea playerSpawnArea;

    private NavMeshSurface navMeshSurface;

    public int gameAreaSize = 10;
    public int forestWidth = 2;

    public int wallChance = 50;
    public int weakWallChance = 75;

    public int numEnemies = 5;
    public float enemySpawnDelay = 30f;
    public float playerSpawnDelay = 5f;

    private int enemiesKilled = 0;

    private int towerAreaSize = 8;
    private int spawnAreaSize = 3;

    private int wallMinLen = 3;
    private int wallMaxLenMin = 5;
    private int wallMaxLenMax = 10;

    private float cellSize = 2f;

    private System.Random rnd = new System.Random();

    private GameObject lastSpawnedPlayer;

    private List<WallGenerator> pendingWallGenerators = new List<WallGenerator>();

    private bool bGameActive = true;

    void Start()
    {
        navMeshSurface = GetComponent<NavMeshSurface>();

        int worldSize = GetWorldSize();

        WorldArea borderedArea = new WorldArea(1, worldSize - 1);
        WorldArea gameArea = new WorldArea(forestWidth + 1, worldSize - (forestWidth + 1));
        WorldArea towerArea = new WorldArea((worldSize - towerAreaSize) / 2, (worldSize + towerAreaSize) / 2);

        // Generate enemy spawns at the map corners
        enemySpawnAreas = new List<WorldArea>
        {
            new WorldArea(new Vector2Int(gameArea.pos1.x, gameArea.pos1.y), new Vector2Int(gameArea.pos1.x + spawnAreaSize, gameArea.pos1.y + spawnAreaSize)),
            new WorldArea(new Vector2Int(gameArea.pos2.x, gameArea.pos2.y), new Vector2Int(gameArea.pos2.x - spawnAreaSize, gameArea.pos2.y - spawnAreaSize)),
            new WorldArea(new Vector2Int(gameArea.pos1.x, gameArea.pos2.y), new Vector2Int(gameArea.pos1.x + spawnAreaSize, gameArea.pos2.y - spawnAreaSize)),
            new WorldArea(new Vector2Int(gameArea.pos2.x, gameArea.pos1.y), new Vector2Int(gameArea.pos2.x - spawnAreaSize, gameArea.pos1.y + spawnAreaSize)),
        };

        // Generate player spawn variants near the tower
        int towerSpawnOffset = (towerAreaSize - spawnAreaSize) / 2;
        List<WorldArea> playerSpawnAreas = new List<WorldArea>
        {
            new WorldArea(new Vector2Int(towerArea.pos1.x, towerArea.pos1.y + towerSpawnOffset), new Vector2Int(towerArea.pos1.x - spawnAreaSize, towerArea.pos1.y + spawnAreaSize + towerSpawnOffset)),
            new WorldArea(new Vector2Int(towerArea.pos2.x, towerArea.pos1.y + towerSpawnOffset), new Vector2Int(towerArea.pos2.x + spawnAreaSize, towerArea.pos1.y + spawnAreaSize + towerSpawnOffset)),
            new WorldArea(new Vector2Int(towerArea.pos1.x + towerSpawnOffset, towerArea.pos2.y), new Vector2Int(towerArea.pos1.x + spawnAreaSize + towerSpawnOffset, towerArea.pos2.y + spawnAreaSize)),
            new WorldArea(new Vector2Int(towerArea.pos1.x + towerSpawnOffset, towerArea.pos1.y), new Vector2Int(towerArea.pos1.x + spawnAreaSize + towerSpawnOffset, towerArea.pos1.y - spawnAreaSize)),
        };

        playerSpawnArea = playerSpawnAreas[rnd.Next(playerSpawnAreas.Count)];

        Instantiate(towerAreaObject, GetAreaCenter(towerArea), Quaternion.identity);

        navMeshSurface.size = new Vector3(gameAreaSize * cellSize, 10f, gameAreaSize * cellSize);
        navMeshSurface.center = GetAreaCenter(towerArea);

        int[,] cellsMatrix = new int[worldSize, worldSize];

        for (var x = 0; x < worldSize; ++x)
        {
            for (var z = 0; z < worldSize; ++z)
            {
                Vector2Int cell = new Vector2Int(x, z);
                Vector3 cellPos = GetCellWorldPos(cell);

                // Cell is occupied by default
                cellsMatrix[cell.x, cell.y] = -1;

                bool bIsGameArea = IsInArea(cell, gameArea);
                bool bIsTowerArea = IsInArea(cell, towerArea);
                bool bIsBorderArea = !IsInArea(cell, borderedArea);

                if (bIsTowerArea)
                {
                    Instantiate(towerGroundObject, cellPos, Quaternion.identity);
                    continue;
                }

                Instantiate(groundObject, cellPos, Quaternion.identity);

                if (IsInAnyArea(cell, enemySpawnAreas.Concat(new List<WorldArea> { playerSpawnArea }).ToList()))
                {
                    continue;
                }

                if (!bIsGameArea && !bIsBorderArea)
                {
                    GameObject forestObject = forestObjects[rnd.Next(forestObjects.Count)];
                    Instantiate(forestObject, cellPos, Quaternion.identity);
                }

                if (bIsBorderArea)
                {
                    SpawnWorldBorder(cell);
                }

                if (bIsGameArea)
                {
                    // Cell is vacant
                    cellsMatrix[cell.x, cell.y] = 0;
                }
            }
        }

        WorldArea wallsArea = new WorldArea(new Vector2Int(gameArea.pos1.x + 1, gameArea.pos1.y + 1), new Vector2Int(gameArea.pos2.x - 1, gameArea.pos2.y - 1));

        int wallId = 1;
        for (var x = wallsArea.pos1.x; x < wallsArea.pos2.x; ++x)
        {
            for (var z = wallsArea.pos1.y; z < wallsArea.pos2.y; ++z)
            {
                if (!GetRandomBool(wallChance))
                {
                    continue;
                }

                Vector2Int cell = new Vector2Int(x, z);
                int wallMaxLen = rnd.Next(wallMaxLenMin, wallMaxLenMax);
                WallGenerator wallClass = GetRandomBool(weakWallChance) ? wallGenWeak : wallGenStrong;
                WallGenerator result = SpawnWall(wallId, wallClass, cell, wallsArea, cellsMatrix, wallMaxLen);
                if (result is not null)
                {
                    ++wallId;
                }
            }
        }

        foreach (var wallGen in pendingWallGenerators)
        {
            wallGen.GenerateWall();
        }

        pendingWallGenerators.Clear();

        navMeshSurface.BuildNavMesh();

        SpawnPlayer();
        StartCoroutine(EnemySpawnRoutine());
    }

    public void OnPlayerKilled()
    {
        if (bGameActive)
        {
            StartCoroutine(PlayerSpawnRoutine());
        }
    }

    public void OnEnemyKilled()
    {
        ++enemiesKilled;
        if (enemiesKilled >= numEnemies)
        {
            OnGameEnd(true);
        }
    }

    public void OnTowerDestroyed()
    {
        OnGameEnd(false);
    }

    private void OnGameEnd(bool bVictory)
    {
        bGameActive = false;
        lastSpawnedPlayer.GetComponent<PlayerController>().DisableInput();
        restartUi.ShowRestartScreen(bVictory);
    }

    public void SpawnPlayer()
    {
        if (lastSpawnedPlayer is not null)
        {
            Destroy(lastSpawnedPlayer);
        }

        lastSpawnedPlayer = InstantiateObjectInArea(playerSpawnArea, playerObject, Quaternion.identity);
    }
    private IEnumerator PlayerSpawnRoutine()
    {
        WaitForSeconds waitTime = new WaitForSeconds(playerSpawnDelay);
        yield return waitTime;
        SpawnPlayer();
    }

    private IEnumerator EnemySpawnRoutine()
    {
        WaitForSeconds waitTime = new WaitForSeconds(enemySpawnDelay);

        for (int i = 0; i < numEnemies; ++i)
        {
            yield return waitTime;
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        InstantiateObjectInArea(enemySpawnAreas[rnd.Next(enemySpawnAreas.Count)], enemyObject, Quaternion.identity);
    }

    private GameObject InstantiateObjectInArea(WorldArea area, GameObject obj, Quaternion quat)
    {
        return Instantiate(obj, GetAreaCenter(area), quat);
    }

    private WallGenerator SpawnWall(int wallId, WallGenerator wallClass, Vector2Int cell, WorldArea area, int[,] cellsMatrix, int wallMaxLen, int wallCurLen = 0)
    {
        if (wallCurLen >= wallMaxLen)
        {
            return null;
        }

        if (cellsMatrix[cell.x, cell.y] != 0)
        {
            return null;
        }

        List<Vector2Int> neighborCells = GetCellNeighbors(cell);
        List<Vector2Int> availCells = new List<Vector2Int>();

        int numThisWallsInNeighbors = 0;
        int numOtherWallsInNeighbors = 0;
        foreach (var n in neighborCells)
        {
            if (!IsInArea(n, area))
            {
                continue;
            }

            int cellId = cellsMatrix[n.x, n.y];

            if (cellId != 0)
            {
                if (cellId == wallId)
                {
                    ++numThisWallsInNeighbors;
                }
                else if (cellId != -1)
                {
                    ++numOtherWallsInNeighbors;
                }
            }
            else
            {
                if (n.x == cell.x || n.y == cell.y)
                {
                    availCells.Add(n);
                }
            }
        }

        if (numOtherWallsInNeighbors > 0)
        {
            return null;
        }

        if (numThisWallsInNeighbors > 2)
        {
            return null;
        }

        cellsMatrix[cell.x, cell.y] = wallId;

        WallGenerator nextWall = SpawnWall(wallId, wallClass, availCells[rnd.Next(availCells.Count)], area, cellsMatrix, wallMaxLen, wallCurLen + 1);
        WallGenerator curWall = null;

        if (nextWall is not null || wallCurLen >= wallMinLen)
        {
            curWall = Instantiate(wallClass.gameObject, GetCellWorldPos(cell), Quaternion.identity).GetComponent<WallGenerator>();

            if (nextWall is not null)
            {
                nextWall.neightborWalls.Add(curWall);
                curWall.neightborWalls.Add(nextWall);
            }

            pendingWallGenerators.Add(curWall);
        }
        else
        {
            cellsMatrix[cell.x, cell.y] = 0;
        }

        return curWall;
    }

    private List<Vector2Int> GetCellNeighbors(Vector2Int cell)
    {
        return new List<Vector2Int> { 
            new Vector2Int(cell.x+1, cell.y),
            new Vector2Int(cell.x, cell.y+1),
            new Vector2Int(cell.x-1, cell.y),
            new Vector2Int(cell.x, cell.y-1),

            new Vector2Int(cell.x+1, cell.y+1),
            new Vector2Int(cell.x-1, cell.y-1),
            new Vector2Int(cell.x+1, cell.y-1),
            new Vector2Int(cell.x-1, cell.y+1),
        };
    }

    private void SpawnWorldBorder(Vector2Int cell)
    {
        GameObject borderObject = Instantiate(borderWallObject, GetCellWorldPos(cell), Quaternion.identity);

        bool bIsXAxis = cell.x == 0 || cell.x == GetWorldSize() - 1;
        bool bIsZAxis = cell.y == 0 || cell.y == GetWorldSize() - 1;

        bool bIsCorner = bIsXAxis && bIsZAxis;

        Vector3 borderScale = new Vector3(bIsZAxis ? 1.0f : .5f, bIsCorner ? 1.5f : 1.0f, bIsXAxis ? 1.0f : .5f);

        borderObject.transform.localScale = borderScale;
    }

    private Vector3 GetAreaCenter(WorldArea area)
    {
        Vector3 startPt = GetCellWorldPos(area.pos1);
        Vector3 endPt = GetCellWorldPos(area.pos2);
        return (endPt - startPt - new Vector3(cellSize, 0f, cellSize)) / 2f + startPt;
    }

    private bool IsInArea(Vector2Int pt, WorldArea area)
    {
        return pt.x >= area.pos1.x && pt.x < area.pos2.x && pt.y >= area.pos1.y && pt.y < area.pos2.y;
    }

    private bool IsInAnyArea(Vector2Int pt, List<WorldArea> areaList)
    {
        foreach (var area in areaList)
        {
            if (IsInArea(pt, area))
            {
                return true;
            }
        }
        return false;
    }

    private Vector3 GetCellWorldPos(Vector2Int cell)
    {
        return new Vector3(cell.x * cellSize, 0, cell.y * cellSize);
    }

    private int GetWorldSize()
    {
        return gameAreaSize + 2 * forestWidth + 2;
    }

    private bool GetRandomBool(int chance)
    {
        return rnd.Next(100) + 1 < chance;
    }

}
