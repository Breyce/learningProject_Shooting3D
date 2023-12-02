using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Map[] maps;
    public int mapIndex;

    public Transform tilePrefab;
    public Transform obstaclePrefab;
    public Transform navmeshFloor;
    public Transform navmeshMaskPrefab;
    public Vector2 maxMapSize;

    [Range(0,1)]
    public float oulinePercent;

    public float tileSize;

    List<Coordinate> allTileCoordinate;
    Queue<Coordinate> shuffledTileCoordinate;
    Queue<Coordinate> shuffledOpenTileCoordinate;

    Map currentMap;
    Transform[,] tileMap;

    private void Start()
    {
        FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
    }

    void OnNewWave(int waveNumber)
    {
        mapIndex = waveNumber - 1;
        GenerateMap();
    }

    public void GenerateMap()
    {
        currentMap = maps[mapIndex];
        tileMap = new Transform[currentMap.mapSize.x, currentMap.mapSize.y];
        System.Random prng = new System.Random(currentMap.seed);
        GetComponent<BoxCollider>().size = new Vector3 (currentMap.mapSize.x * tileSize, .05f, currentMap.mapSize.y * tileSize);

        //生成坐标
        allTileCoordinate = new List<Coordinate>();
        for (int i = 0; i < currentMap.mapSize.x; i++)
        {
            for (int j = 0; j < currentMap.mapSize.y; j++)
            {
                allTileCoordinate.Add(new Coordinate(i, j));
            }
        }
        shuffledTileCoordinate = new Queue<Coordinate>(Utility.ShuffleArray(allTileCoordinate.ToArray(), currentMap.seed));

        //生成Generate Map
        string holderName = "Generate Map";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        //生成地图
        for (int i = 0; i < currentMap.mapSize.x; i++)
        {
            for (int j = 0; j < currentMap.mapSize.y; j++)
            {
                /*
                 mapSize.x = 10; mapSize.y = 10;
                 第一次循环的点坐标(-4.5, 0, -4.5)； 第二次循环点坐标(-3.5, 0, -3.5)；第三次循环点坐标(-2.5, 0, -2.5)；...
                 */
                Vector3 tilePosition = CoordToPosition(i, j);
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;

                newTile.localScale = Vector3.one * (1 - oulinePercent) * tileSize;
                newTile.parent = mapHolder;

                //存储tile
                tileMap[i,j] = newTile;
            }
        }

        //生成障碍
        bool[,] obstacleMap = new bool[(int)currentMap.mapSize.x,(int)currentMap.mapSize.y];
        
        int obstacleCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y * currentMap.obstaclePercent);
        int currentObstacleCount = 0;

        List<Coordinate> allOpenCoordinate = new List<Coordinate>(allTileCoordinate);
        for (int i = 0;i < obstacleCount; i++)
        {
            Coordinate randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            //Debug.Log("x: " + randomCoord.x +" y: "+ randomCoord.y);
            currentObstacleCount++;
            //Debug.Log("("+mapCenter.x+","+ mapCenter.y + ") (" + randomCoord.x + "," + randomCoord.y + ")");

            if(randomCoord != currentMap.mapCenter && MapIsFullyAccessiable(obstacleMap, currentObstacleCount))
            {
                //随机障碍高度
                float obstacleHeight = Mathf.Lerp(currentMap.minObstacleHeight,currentMap.maxObstacleHeight,(float)prng.NextDouble());

                Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);
                Transform newObstacle = Instantiate(obstaclePrefab,obstaclePosition + Vector3.up * obstacleHeight / 2f,Quaternion.identity) as Transform;

                newObstacle.localScale = new Vector3((1 - oulinePercent) * tileSize, obstacleHeight, (1 - oulinePercent) * tileSize);
                newObstacle.parent = mapHolder;

                //给物体上色
                Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();
                Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
                float colorPercent = randomCoord.y / (float)currentMap.mapSize.y;
                obstacleMaterial.color = Color.Lerp(currentMap.foregroundColor, currentMap.backgroundColor, colorPercent);
                obstacleRenderer.sharedMaterial = obstacleMaterial;

                //移除生成敌人的位置
                allOpenCoordinate.Remove(randomCoord);
            }
            else
            {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObstacleCount--;
            }
        }

        shuffledOpenTileCoordinate = new Queue<Coordinate>(Utility.ShuffleArray(allOpenCoordinate.ToArray(), currentMap.seed));

        //生成导航遮罩
        //左
        Transform maskLeft = Instantiate(navmeshMaskPrefab, Vector3.left * (currentMap.mapSize.x + maxMapSize.x)/4f * tileSize, Quaternion.identity) as Transform;
        maskLeft.parent = mapHolder;
        maskLeft.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;

        //右
        Transform maskRight = Instantiate(navmeshMaskPrefab, Vector3.right * (currentMap.mapSize.x + maxMapSize.x)/4f * tileSize, Quaternion.identity) as Transform;
        maskRight.parent = mapHolder;
        maskRight.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;

        //上
        Transform maskTop = Instantiate(navmeshMaskPrefab, Vector3.forward * (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity) as Transform;
        maskTop.parent = mapHolder;
        maskTop.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;

        //下
        Transform maskBottom = Instantiate(navmeshMaskPrefab, Vector3.back * (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity) as Transform;
        maskBottom.parent = mapHolder;
        maskBottom.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;

        navmeshFloor.localScale = new Vector3(maxMapSize.x, maxMapSize.y) * tileSize;
    }

    bool MapIsFullyAccessiable(bool[,] obstacleMap, int currentObstacleCount)
    {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Coordinate> queue = new Queue<Coordinate>();

        queue.Enqueue(currentMap.mapCenter);
        mapFlags[currentMap.mapCenter.x, currentMap.mapCenter.y] = true;

        int accessiableTileCount = 1;

        while (queue.Count > 0)
        {
            Coordinate tile = queue.Dequeue();

            for(int x = -1; x <= 1; x++)
            {
                for(int y = -1; y <= 1; y++)
                {
                    int neighbourX = tile.x + x;
                    int neighbourY = tile.y + y;
                    if(x == 0 || y == 0)
                    {
                        if(neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) &&
                            neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1))
                        {
                            if (!mapFlags[neighbourX,neighbourY] && !obstacleMap[neighbourX, neighbourY])
                            {
                                mapFlags[neighbourX,neighbourY] = true;
                                queue.Enqueue(new Coordinate(neighbourX,neighbourY));
                                accessiableTileCount++;
                            }
                        }
                    }
                }
            }
        }

        int targetAccessiableTileCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y - currentObstacleCount);

        return targetAccessiableTileCount == accessiableTileCount;
    }
    Vector3 CoordToPosition(int i, int j)
    {
        return new Vector3(-currentMap.mapSize.x / 2f + i + 0.5f, 0, -currentMap.mapSize.y / 2f + j + 0.5f) * tileSize;
    }

    public Transform GetTileFromPosition(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x / tileSize + (currentMap.mapSize.x - 1) / 2f);
        int y = Mathf.RoundToInt(position.z / tileSize + (currentMap.mapSize.y - 1) / 2f);

        x = Mathf.Clamp(x, 0, tileMap.GetLength(0) - 1);
        y = Mathf.Clamp(y, 0, tileMap.GetLength(1) - 1);

        return tileMap[x, y];
    }

    public Coordinate GetRandomCoord()
    {
        Coordinate randomCoord = shuffledTileCoordinate.Dequeue();
        shuffledTileCoordinate.Enqueue(randomCoord);
        return randomCoord;
    }

    public Transform GetRandomOpenTile()
    {
        Coordinate randomCoord = shuffledOpenTileCoordinate.Dequeue();
        shuffledOpenTileCoordinate.Enqueue(randomCoord);
        return tileMap[randomCoord.x,randomCoord.y];
    }

    [System.Serializable]
    public struct Coordinate
    {
        public int x;
        public int y;

        public Coordinate(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public static bool operator == (Coordinate left, Coordinate right)
        {
            return left.x == right.x && left.y == right.y;
        }

        public static bool operator !=(Coordinate left, Coordinate right)
        {
            return !(left == right);
        }
    }

    [System.Serializable]
    public class Map
    {

        public Coordinate mapSize;
        [Range(0,1)]
        public float obstaclePercent;
        public int seed;
        public float minObstacleHeight;
        public float maxObstacleHeight;
        public Color foregroundColor;
        public Color backgroundColor;

        public Coordinate mapCenter
        {
            get
            {
                return new Coordinate(mapSize.x/2, mapSize.y/2);
            }
        }
    }
}
