using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    // The 2D array from specs
    private int[,] levelMap =
    {
        {1,2,2,2,2,2,2,2,2,2,2,2,2,7},
        {2,5,5,5,5,5,5,5,5,5,5,5,5,4},
        {2,5,3,4,4,3,5,3,4,4,4,3,5,4},
        {2,6,4,0,0,4,5,4,0,0,0,4,5,4},
        {2,5,3,4,4,3,5,3,4,4,4,3,5,3},
        {2,5,5,5,5,5,5,5,5,5,5,5,5,5},
        {2,5,3,4,4,3,5,3,3,5,3,4,4,4},
        {2,5,3,4,4,3,5,4,4,5,3,4,4,3},
        {2,5,5,5,5,5,5,4,4,5,5,5,5,4},
        {1,2,2,2,2,1,5,4,3,4,4,3,0,4},
        {0,0,0,0,0,2,5,4,3,4,4,3,0,3},
        {0,0,0,0,0,2,5,4,4,0,0,0,0,0},
        {0,0,0,0,0,2,5,4,4,0,3,4,4,8},
        {2,2,2,2,2,1,5,3,3,0,4,0,0,0},
        {0,0,0,0,0,0,5,0,0,0,4,0,0,0},
    };

    // Test map for testing different layouts
    private int[,] testMap =
    {
        {1,2,2,7},
        {2,5,5,4},
        {2,5,5,4},
        {1,2,2,3}
    };

    // Prefabs for each tile type
    public GameObject[] tilePrefabs = new GameObject[9]; // Index matches tile type

    // Level management
    public GameObject manualLevelParent;
    public Transform proceduralLevelParent;
    public Camera gameCamera;

    // Control flags
    public bool generateNewMap = false;
    public bool useTestMap = false;

    void Start()
    {
        // Only generate procedural level if flag is enabled
        if (generateNewMap)
        {
            // Disable manual level but don't destroy it (needed for grading)
            if (manualLevelParent != null)
            {
                manualLevelParent.SetActive(false);
            }

            // Use test map if flag is enabled
            if (useTestMap)
            {
                levelMap = testMap;
            }

            // Generate the procedural level
            GenerateProceduralLevel();

            // Adjust camera to show the whole level
            AdjustCameraToLevel();
        }
        // If generateNewMap is false, manual level stays active and nothing happens
    }

    void GenerateProceduralLevel()
    {
        // Clear any existing procedural level
        ClearProceduralLevel();

        // Generate all four quadrants
        GenerateTopLeftQuadrant();
        GenerateTopRightQuadrant();
        GenerateBottomLeftQuadrant();
        GenerateBottomRightQuadrant();
    }

    void ClearProceduralLevel()
    {
        // Remove any existing generated tiles
        foreach (Transform child in proceduralLevelParent)
        {
            Destroy(child.gameObject);
        }
    }

    void GenerateTopLeftQuadrant()
    {
        int rows = levelMap.GetLength(0);
        int cols = levelMap.GetLength(1);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                int tileType = levelMap[row, col];
                if (tileType == 0) continue; // Skip empty spaces

                // Calculate position based on coordinate system
                Vector3 position = new Vector3(0.5f + col, 9.5f - row, 0);

                // Calculate rotation based on surrounding tiles
                float rotation = CalculateTileRotation(row, col, tileType, false, false);

                // Create the tile
                CreateTile(tileType, position, rotation);
            }
        }
    }

    void GenerateTopRightQuadrant()
    {
        int rows = levelMap.GetLength(0);
        int cols = levelMap.GetLength(1);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                int tileType = levelMap[row, col];
                if (tileType == 0) continue;

                // Mirror horizontally (coordinates: 14.5 to 27.5)
                Vector3 position = new Vector3(27.5f - col, 9.5f - row, 0);

                // Calculate rotation with horizontal flip
                float rotation = CalculateTileRotation(row, col, tileType, true, false);

                CreateTile(tileType, position, rotation);
            }
        }
    }

    void GenerateBottomLeftQuadrant()
    {
        int rows = levelMap.GetLength(0);
        int cols = levelMap.GetLength(1);

        // Skip last row to avoid duplication (specs requirement)
        for (int row = 0; row < rows - 1; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                int tileType = levelMap[row, col];
                if (tileType == 0) continue;

                // Mirror vertically
                Vector3 position = new Vector3(0.5f + col, -18.5f + row, 0);

                // Calculate rotation with vertical flip
                float rotation = CalculateTileRotation(row, col, tileType, false, true);

                CreateTile(tileType, position, rotation);
            }
        }
    }

    void GenerateBottomRightQuadrant()
    {
        int rows = levelMap.GetLength(0);
        int cols = levelMap.GetLength(1);

        // Skip last row to avoid duplication
        for (int row = 0; row < rows - 1; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                int tileType = levelMap[row, col];
                if (tileType == 0) continue;

                // Mirror both horizontally and vertically
                Vector3 position = new Vector3(27.5f - col, -18.5f + row, 0);

                // Calculate rotation with both flips
                float rotation = CalculateTileRotation(row, col, tileType, true, true);

                CreateTile(tileType, position, rotation);
            }
        }
    }

    float CalculateTileRotation(int row, int col, int tileType, bool flipHorizontal, bool flipVertical)
    {
        // Top-left corner always has default rotation (specs requirement)
        if (row == 0 && col == 0 && !flipHorizontal && !flipVertical)
        {
            return 0f;
        }

        float rotation = 0f;

        // Check what type of tile this is and calculate appropriate rotation
        if (tileType == 1 || tileType == 3) // Corners
        {
            rotation = CalculateCornerRotation(row, col);
        }
        else if (tileType == 2 || tileType == 4) // Walls
        {
            rotation = CalculateWallRotation(row, col);
        }
        else if (tileType == 7) // T-junction
        {
            rotation = CalculateTJunctionRotation(row, col);
        }
        // Pellets and other types don't need rotation

        // Apply mirroring adjustments
        if (flipHorizontal)
        {
            rotation = 360f - rotation;
        }
        if (flipVertical)
        {
            rotation = 360f - rotation;
        }

        return rotation % 360f;
    }

    float CalculateCornerRotation(int row, int col)
    {
        // Check which directions have walls or other tiles
        bool hasUp = GetTileTypeAt(row - 1, col) != 0;
        bool hasDown = GetTileTypeAt(row + 1, col) != 0;
        bool hasLeft = GetTileTypeAt(row, col - 1) != 0;
        bool hasRight = GetTileTypeAt(row, col + 1) != 0;

        // Determine corner orientation based on connections
        if (hasDown && hasRight) return 0f;   // Top-left corner
        if (hasDown && hasLeft) return 90f;   // Top-right corner
        if (hasUp && hasLeft) return 180f;    // Bottom-right corner
        if (hasUp && hasRight) return 270f;   // Bottom-left corner

        return 0f; // Default
    }

    float CalculateWallRotation(int row, int col)
    {
        // Check adjacent tiles to determine if wall should be horizontal or vertical
        bool hasUpDown = (GetTileTypeAt(row - 1, col) != 0) || (GetTileTypeAt(row + 1, col) != 0);
        bool hasLeftRight = (GetTileTypeAt(row, col - 1) != 0) || (GetTileTypeAt(row, col + 1) != 0);

        if (hasUpDown && !hasLeftRight)
        {
            return 90f; // Vertical wall
        }
        return 0f; // Horizontal wall
    }

    float CalculateTJunctionRotation(int row, int col)
    {
        // Check which three directions have connections
        bool hasUp = GetTileTypeAt(row - 1, col) != 0;
        bool hasDown = GetTileTypeAt(row + 1, col) != 0;
        bool hasLeft = GetTileTypeAt(row, col - 1) != 0;
        bool hasRight = GetTileTypeAt(row, col + 1) != 0;

        // T-junction orientation based on which direction is open
        if (!hasUp && hasDown && hasLeft && hasRight) return 0f;    // Opening up
        if (!hasRight && hasUp && hasDown && hasLeft) return 90f;   // Opening right
        if (!hasDown && hasUp && hasLeft && hasRight) return 180f;  // Opening down
        if (!hasLeft && hasUp && hasDown && hasRight) return 270f;  // Opening left

        return 0f; // Default
    }

    int GetTileTypeAt(int row, int col)
    {
        // Safe array access with bounds checking
        if (row < 0 || row >= levelMap.GetLength(0) || col < 0 || col >= levelMap.GetLength(1))
        {
            return 0; // Out of bounds = empty
        }
        return levelMap[row, col];
    }

    void CreateTile(int tileType, Vector3 position, float rotation)
    {
        // Make sure we have a prefab for this tile type
        if (tileType < tilePrefabs.Length && tilePrefabs[tileType] != null)
        {
            GameObject newTile = Instantiate(tilePrefabs[tileType], position, Quaternion.Euler(0, 0, rotation));
            newTile.transform.SetParent(proceduralLevelParent);

            // Name it for easier debugging
            newTile.name = $"Tile_{tileType}_R{position.y}_C{position.x}";
        }
    }

    void AdjustCameraToLevel()
    {
        // Calculate the bounds of the generated level
        int rows = levelMap.GetLength(0);
        int cols = levelMap.GetLength(1);

        // Total level size with all quadrants
        float totalWidth = cols * 2; // Two quadrants side by side
        float totalHeight = (rows - 1) * 2; // Account for skipped bottom row

        // Position camera at center of level
        float centerX = totalWidth / 2f;
        float centerY = -totalHeight / 4f; // Adjust for coordinate system

        gameCamera.transform.position = new Vector3(centerX, centerY, -10f);

        // Set camera size to show entire level with some padding
        float maxDimension = Mathf.Max(totalWidth, totalHeight);
        gameCamera.orthographicSize = maxDimension / 2f + 2f;
    }
}