using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Transform tilePrefab;
    public Transform obstaclePrefab;
    public Transform navmeshFloor;
    public Transform navmeshMaskPrefab;
    public Vector2 mapSize;
    public Vector2 maxMapSize;

    [Range(0,1)]
    public float oulinePercent;
    [Range(0,1)]
    public float obstaclePercent;

    public float tileSize;

    List<Coordinate> allTileCoordinate;
    Queue<Coordinate> shuffledTileCoordinate;

    public int seed = 10;
    Coordinate mapCenter;

    private void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        allTileCoordinate = new List<Coordinate>();
        for (int i = 0; i < mapSize.x; i++)
        {
            for (int j = 0; j < mapSize.y; j++)
            {
                allTileCoordinate.Add(new Coordinate(i, j));
            }
        }
        shuffledTileCoordinate = new Queue<Coordinate>(Utility.ShuffleArray(allTileCoordinate.ToArray(), seed));
        mapCenter = new Coordinate((int)mapSize.x/2, (int)mapSize.y/2);

        string holderName = "Generate Map";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        for (int i = 0; i < mapSize.x; i++)
        {
            for (int j = 0; j < mapSize.y; j++)
            {
                /*
                 mapSize.x = 10; mapSize.y = 10;
                 第一次循环的点坐标(-4.5, 0, -4.5)； 第二次循环点坐标(-3.5, 0, -3.5)；第三次循环点坐标(-2.5, 0, -2.5)；...
                 */
                Vector3 tilePosition = CoordToPosition(i, j);
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;

                newTile.localScale = Vector3.one * (1 - oulinePercent) * tileSize;
                newTile.parent = mapHolder;
            }
        }

        bool[,] obstacleMap = new bool[(int)mapSize.x,(int)mapSize.y];
        
        int obstacleCount = (int)(mapSize.x * mapSize.y * obstaclePercent);
        int currentObstacleCount = 0;
        for (int i = 0;i < obstacleCount; i++)
        {
            Coordinate randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            //Debug.Log("x: " + randomCoord.x +" y: "+ randomCoord.y);
            currentObstacleCount++;
            //Debug.Log("("+mapCenter.x+","+ mapCenter.y + ") (" + randomCoord.x + "," + randomCoord.y + ")");

            if(randomCoord != mapCenter && MapIsFullyAccessiable(obstacleMap, currentObstacleCount))
            {
                Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);
                Transform newObstacle = Instantiate(obstaclePrefab,obstaclePosition + Vector3.up * .5f,Quaternion.identity) as Transform;

                newObstacle.localScale = Vector3.one * (1 - oulinePercent) * tileSize;
                newObstacle.parent = mapHolder;
            }
            else
            {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObstacleCount--;
            }
        }

        navmeshFloor.localScale = new Vector3(maxMapSize.x, maxMapSize.y) * tileSize;
    }

    bool MapIsFullyAccessiable(bool[,] obstacleMap, int currentObstacleCount)
    {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Coordinate> queue = new Queue<Coordinate>();

        queue.Enqueue(mapCenter);
        mapFlags[mapCenter.x, mapCenter.y] = true;

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

        int targetAccessiableTileCount = (int)(mapSize.x * mapSize.y - currentObstacleCount);

        return targetAccessiableTileCount == accessiableTileCount;
    }
    Vector3 CoordToPosition(int i, int j)
    {
        return new Vector3(-mapSize.x / 2 + i + 0.5f, 0, -mapSize.y / 2 + j + 0.5f) * tileSize;
    }

    public Coordinate GetRandomCoord()
    {
        Coordinate randomCoord = shuffledTileCoordinate.Dequeue();
        shuffledTileCoordinate.Enqueue(randomCoord);
        return randomCoord;
    }

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
}
