using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GameManager;
using Random = UnityEngine.Random;

public class GridManagerTiles : MonoBehaviour
{
    public static GridManagerTiles Instance;

    public SpriteRenderer imageRenderer; // Referencia al SpriteRenderer de la imagen

    public int scenarioWidth; // tamaño del grid en X (cantidad de casillas de ancho)
    public int scenarioHeight; // tamaño del grid en Y (cantidad de casillas de alto)

    // celdas que forman el escenario
    private Dictionary<Vector2, Tile> _tiles;

    [Header("Tipos de tiles")]
    //[SerializeField] private Tile _baseTile;
    [SerializeField] private Tile _normalTile;

    [SerializeField] private Tile _hinderingTile;
    [SerializeField] private Tile _obstructiveTile;

    // Camino desde la celda en la que está el personaje seleccionado a la celda sobre la que está el cursor del ratón
    // al hacer clic el personaje se muee a la celda del cursor, y se borra el camino
    private List<Tile> currentPath = new List<Tile>();

    //[SerializeField] private Transform _cam;
    //[SerializeField] private GameObject _scenarioGrid;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //GenerateGrid();

        //OnDrawGizmos();
    }

    private void Update()
    {
        //HandleMouseClick();

        //HandleMouseHover();
    }

    // Dibujar Gizmos en la vista de escena

    private void OnDrawGizmos()
    {
        Scenario scenario = GameManager.Instance.scenario;

        Color penaltyColor = Color.yellow;
        Color blockedColor = Color.red;

        if (scenario == null || imageRenderer == null)
        {
            return;
        }

        // tamaño de la celda (las celdas son cuadradas)
        float tamañoCelda = GetCellSize();

        for (int celdaX = 0; celdaX < GameManager.Instance.scenario.Width; celdaX++)
        {
            for (int celdaY = 0; celdaY < GameManager.Instance.scenario.Height; celdaY++)
            {
                // Calcular la posición de la celda en el mundo
                Vector3 centroCelda = GetCellCenterPosition(celdaX, celdaY);

                // Dibujar un cuadro en la celda según su tipo
                // Mostrar el costo de la celda como texto

                Square celda = scenario.Squares.First(s => s.x == celdaX && s.y == celdaY);

                switch (celda.terrainType)
                {
                    case GameManager.TerrainType.Obstructive:
                        Gizmos.color = penaltyColor;
                        Gizmos.DrawWireCube(centroCelda, new Vector3(tamañoCelda, tamañoCelda, 0.1f));

                        break;

                    case GameManager.TerrainType.Hindering:
                        Gizmos.color = blockedColor;
                        Gizmos.DrawWireCube(centroCelda, new Vector3(tamañoCelda, tamañoCelda, 0.1f));

                        break;

                    default:
                        break;
                }

                // Mostrar el costo de la celda como texto
                //Handles.Label(centroCelda, gridCeldasEscenario[celdaX, celdaY].tipoTerreno.ToString());

                //Handles.Label(centroCelda, GetCellCost(celdaX, celdaY).ToString());
            }
        }
    }

    // Manejar el clic del ratón para mover el personaje
    //private void HandleMouseClick()
    //{
    //    if (Input.GetMouseButtonDown(0)) // Al hacer clic
    //    {
    //        // Obtener la posición del clic en el mundo 2D
    //        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //        mouseWorldPos.z = 0; // Asegurarse de que la Z sea 0 en 2D

    //        // Celdas, actual y destino
    //        //Vector2Int currentCell = gridManager.GetCellFromWorldPosition(transform.position);
    //        Vector2Int targetCell = gridManager.GetCellFromWorldPosition(mouseWorldPos);

    //        // Verificar si la celda de destino es válida
    //        //if (x >= 0 && x < gridManager.tamañoEscenarioX && y >= 0 && y < gridManager.tamañoEscenarioY)
    //        if (targetCell.x >= 0 && targetCell.x < gridManager.tamañoEscenarioX
    //            && targetCell.y >= 0 && targetCell.y < gridManager.tamañoEscenarioY)
    //        {
    //            // Obtener el costo de la celda (opcional)
    //            int cellCost = gridManager.GetCellCost(targetCell.x, targetCell.y);
    //            Debug.Log($"X: {targetCell.x}, Y: {targetCell.y}, costo: " + cellCost);

    //            // Verificar si la celda es transitable
    //            //if (gridManager.IsCellWalkable(targetCell.x, targetCell.y) && !gridManager.IsCellInClosedArea(targetCell))
    //            if (gridManager.IsCellWalkable(targetCell.x, targetCell.y) && !gridManager.IsCellClosed(targetCell.x, targetCell.y))
    //            {
    //                // Calcular la posición central de la celda de destino
    //                targetPosition = gridManager.GetCellCenterPosition(targetCell.x, targetCell.y);

    //                // Mover suavemente el personaje a la posición objetivo
    //                Debug.Log($"Objeto se moverá a la celda: {targetCell.x}, {targetCell.y}");
    //                //transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
    //                transform.position = targetPosition;
    //            }
    //            else
    //            {
    //                Debug.Log("Celda bloqueada");
    //                MostrarInfoPersonaje("No puedo mover allí:" + Environment.NewLine
    //                    + "Celda bloqueada");
    //            }
    //        }
    //        else
    //        {
    //            Debug.Log("Celda fuera del grid");
    //            MostrarInfoPersonaje("No puedo mover allí:" + Environment.NewLine
    //                + "Celda fuera del grid");
    //        }

    //        // Ocultar el texto después de 2 segundos
    //        Invoke(nameof(OcultarInfoPersonaje), 2f);
    //    }
    //}

    // Manejar el hover del ratón para mostrar información de la celda
    private void HandleMouseHover()
    {
        //Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //mouseWorldPos.z = 0; // Asegurarse de que la Z sea 0 en 2D

        //// Obtener la celda del personaje
        //Vector2Int characterCell = gridManager.GetCellFromWorldPosition(transform.position);

        //Vector2Int celda = gridManager.GetCellFromWorldPosition(mouseWorldPos);

        //// Calcular la distancia en casillas
        //int distance = gridManager.CalculateCellDistance(characterCell, celda);

        //MostrarInfoPersonaje($"Distancia: {distance} casillas");

        //DrawArrowDebug();

        //DrawArrowGame();
    }

    public void GenerateGrid(int scenarioWidth, int scenarioHeight, List<Square> scenarioSquares)
    {
        // Calcular el tamaño del grid en base al tamaño de la imagen

        // posición de la imagen que representa el escenario
        Vector3 posicionImagenEscenario = GetScenarioImagePosition();

        // tamaño de la imagen que representa el escenario
        Vector2 tamañoImagenEscenario = GetScenarioImageSize();

        // tamaño de la celda (las celdas son cuadradas)
        float tamañoCelda = GetCellSize();

        this.scenarioWidth = scenarioWidth;
        this.scenarioHeight = scenarioHeight;

        _tiles = new Dictionary<Vector2, Tile>();

        for (int x = 0; x < scenarioWidth; x++)
        {
            for (int y = 0; y < scenarioHeight; y++)
            {
                Tile tile = null;

                if (!scenarioSquares.Any(s => s.x == x && s.y == y))
                {
                    tile = _normalTile;
                }
                else
                {
                    TerrainType terrainType = scenarioSquares.First(s => s.x == x && s.y == y).terrainType;

                    switch (terrainType)
                    {
                        case TerrainType.Normal:
                            tile = _normalTile;
                            break;

                        case TerrainType.Hindering:
                            tile = _hinderingTile;
                            break;

                        case TerrainType.Obstructive:
                            tile = _obstructiveTile;
                            break;

                        default:
                            break;
                    }
                }

                // crear Tile en posición (x, y)
                tile.x = x;
                tile.y = y;

                // Calcular la posición de la celda
                float posicioInicialX = posicionImagenEscenario.x - tamañoImagenEscenario.x / 2;
                float posicioInicialY = posicionImagenEscenario.y - tamañoImagenEscenario.y / 2;

                float posicionTileX = posicioInicialX + (x * tamañoCelda) + (tamañoCelda / 2);
                float posicionTileY = posicioInicialY + (y * tamañoCelda) + (tamañoCelda / 2);

                Tile spawnedTile = Instantiate(tile, new Vector3(posicionTileX, posicionTileY), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";

                //spawnedTile.Init(x, y);

                _tiles[new Vector2(x, y)] = spawnedTile;
            }
        }

        // cambiar estado del juego
        GameManager.Instance.ChangeState(GameState.SpawnHeroes);
    }

    public Tile GetHeroSpawnTile()
    {
        return _tiles.Where(tile => tile.Key.x < scenarioWidth / 2 && tile.Value.Walkable).OrderBy(tile => Random.value).First().Value;
    }

    public Tile GetEnemySpawnTile()
    {
        return _tiles.Where(tile => tile.Key.x > scenarioWidth / 2 && tile.Value.Walkable).OrderBy(tile => Random.value).First().Value;
    }

    public Tile GetTileAtPosition(Vector2Int pos)
    {
        if (_tiles.TryGetValue(pos, out Tile tile))
        {
            return tile;
        }

        return null;
    }

    // Implementación de un algoritmo A* para encontrar el camino en la cuadrícula.
    public List<Tile> FindPath(Vector2Int start, Vector2Int end)
    {
        // si el camino actual coincide con el inicio y el final, devolverlo
        if (currentPath != null && currentPath.Any()
            && start == new Vector2Int(currentPath.First().x, currentPath.First().y)
            && end == new Vector2Int(currentPath.Last().x, currentPath.Last().y))
        {
            return currentPath;
        }

        ClearPathHighlight();

        List<Tile> path = new List<Tile>();

        // Si el inicio y el final son el mismo, devolver una lista con la casilla inicial
        if (start == end)
        {
            path.Add(GetTileAtPosition(start));

            return path;
        }

        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        Dictionary<Vector2Int, int> costSoFar = new Dictionary<Vector2Int, int>();
        PriorityQueue<Vector2Int> frontier = new PriorityQueue<Vector2Int>();
        frontier.Enqueue(start, 0);
        costSoFar[start] = 0;

        while (frontier.Count > 0)
        {
            Vector2Int current = frontier.Dequeue();

            if (current == end)
            {
                Vector2Int cur = end;

                while (cur != start)
                {
                    path.Add(GetTileAtPosition(new Vector2Int(cur.x, cur.y)));
                    cur = cameFrom[cur];
                }

                path.Add(GetTileAtPosition(new Vector2Int(start.x, start.y)));
                path.Reverse();

                currentPath = path;

                SetPathHighlight();

                return path;
            }

            foreach (Vector2Int next in GetNeighbors(current, true))
            {
                Tile nextTile = GetTileAtPosition(new Vector2Int(next.x, next.y));

                if (nextTile == null) // || IsTileBlocked(nextTile))
                {
                    continue; // Saltar si hay un obstáculo
                }

                int newCost = costSoFar[current] + 1;

                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                {
                    costSoFar[next] = newCost;
                    int priority = newCost + Mathf.Abs(next.x - end.x) + Mathf.Abs(next.y - end.y);

                    frontier.Enqueue(next, priority);
                    cameFrom[next] = current;
                }
            }
        }

        // No se encontró un camino válido
        currentPath = null;
        return null;
    }

    // Pone el resaltado de las casillas del camino actual.
    public void SetPathHighlight()
    {
        // si hay camino, lo resaltamos
        if (currentPath != null)
        {
            foreach (Tile tile in currentPath)
            {
                tile.SetHighlight(true);
            }
        }
    }

    // Quita el resaltado de las casillas del camino actual.
    public void ClearPathHighlight()
    {
        if (currentPath != null)
        {
            foreach (Tile tile in currentPath)
            {
                tile.SetHighlight(false);
            }

            currentPath = null;
        }
    }

    private List<Vector2Int> GetNeighbors(Vector2Int celdaOrigen, bool soloOrtogonal)
    {
        // las celdas vecinas ortogonalmente
        // -------------
        // |   | V |   |
        // -------------
        // | V | O | V |
        // -------------
        // |   | V |   |
        // -------------
        List<Vector2Int> celdasVecinas = new List<Vector2Int>
        {
            new Vector2Int(celdaOrigen.x, celdaOrigen.y + 1),
            new Vector2Int(celdaOrigen.x - 1, celdaOrigen.y),
            new Vector2Int(celdaOrigen.x + 1, celdaOrigen.y),
            new Vector2Int(celdaOrigen.x, celdaOrigen.y - 1)
        };

        // las celdas vecinas diagonalmente
        // -------------
        // | V |   | V |
        // -------------
        // |   | O |   |
        // -------------
        // | V |   | V |
        // -------------
        if (!soloOrtogonal)
        {
            celdasVecinas.AddRange(new List<Vector2Int>
            {
                new Vector2Int(celdaOrigen.x - 1, celdaOrigen.y + 1),
                new Vector2Int(celdaOrigen.x + 1, celdaOrigen.y + 1),
                new Vector2Int(celdaOrigen.x - 1, celdaOrigen.y - 1),
                new Vector2Int(celdaOrigen.x + 1, celdaOrigen.y - 1)
            });
        }

        return celdasVecinas;
    }

    private bool IsTileBlocked(Tile tile)
    {
        Vector3 position = tile.transform.position;
        bool hasObstacle = Physics.CheckBox(position, new Vector3(0.4f, 0.4f, 0.4f), Quaternion.identity, LayerMask.GetMask("Obstaculos"));

        return hasObstacle; // Devuelve true si hay un obstáculo en la casilla
    }

    #region Utils

    // Método para verificar si una celda es transitable
    public bool IsCellWalkable(int x, int y)
    {
        return _tiles.First(t => t.Key.x == x && t.Key.y == y).Value.Walkable;
    }

    public bool IsSquareWalkable(int x, int y)
    {
        return GameManager.Instance.scenario.Squares.First(s => s.x == x && s.y == y).terrainType != TerrainType.Hindering;
    }

    // Método para obtener el costo de una celda (es el valor en la enumeración de tipos de terreno)
    public int GetCellCost(int x, int y)
    {
        return (int)GameManager.Instance.scenario.Squares.First(s => s.x == x && s.y == y).terrainType;
    }

    // Método para calcular la distancia en casillas entre dos celdas (distancia Manhattan, también conocida como distancia L1)
    public int CalculateCellDistance(Vector2Int cellA, Vector2Int cellB)
    {
        return Mathf.Abs(cellA.x - cellB.x) + Mathf.Abs(cellA.y - cellB.y);
    }

    // Método para calcular la posición central de una celda
    public Vector3 GetCellCenterPosition(int x, int y)
    {
        // Obtener la posición de la imagen del escenario que representa el escenario
        Vector3 posicionImagenEscenario = GetScenarioImagePosition();

        // tamaño de la imagen que representa el escenario (se usa para saber el tamaño de las celdas)
        Vector2 tamañoImagenEscenario = GetScenarioImageSize();

        // tamaño de la celda (las celdas son cuadradas)
        float tamañoCelda = GetCellSize();

        // Calcular la posición central de la celda
        // se le resta la mitad del tamaño de la imagen porque se calcula desde la posición de la imagen, que es su centro, en 0,0,0
        Vector3 cellCenter = posicionImagenEscenario + new Vector3(
            (x * tamañoCelda) + (tamañoCelda / 2) - (tamañoImagenEscenario.x / 2),
            (y * tamañoCelda) + (tamañoCelda / 2) - (tamañoImagenEscenario.y / 2),
            0
         );

        return cellCenter;
    }

    // Método para obtener la celda bajo una posición en el mundo
    public Vector2Int GetCellFromWorldPosition(Vector3 worldPosition)
    {
        Vector3 imagePosition = transform.position; // Posición del grid

        // tamaño de la imagen que representa el escenario (se usa para saber el tamaño de las celdas)
        Vector2 tamañoImagenEscenario = GetScenarioImageSize();

        // tamaño de la celda (las celdas son cuadradas)
        float tamañoCelda = GetCellSize();

        int x = Mathf.FloorToInt((worldPosition.x - imagePosition.x + tamañoImagenEscenario.x / 2) / tamañoCelda);
        int y = Mathf.FloorToInt((worldPosition.y - imagePosition.y + tamañoImagenEscenario.y / 2) / tamañoCelda);

        return new Vector2Int(x, y);
    }

    public float GetCellSize()
    {
        // el tamaño de los tiles y de las casillas es el ancho de la imagen del escenario dividido por el número de casillas que tiene de ancho
        // los tiles y las celdas son cuadrados
        float tamañoCelda = imageRenderer.sprite.bounds.size.x / scenarioWidth;

        return tamañoCelda;
    }

    private Vector3 GetScenarioImagePosition()
    {
        // posición de la imagen del escenario
        Vector3 posicionImagenEscenario = imageRenderer.transform.position; // posición de la imagen que representa el escenario

        return posicionImagenEscenario;
    }

    private Vector2 GetScenarioImageSize()
    {
        // tamaño de la imagen del escenario
        Vector2 vector2 = imageRenderer.sprite.bounds.size;

        return vector2;
    }

    #endregion Utils
}