using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GameManager;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // escenario
    public Scenario scenario;

    public int scenarioNumber;

    // estado del juego
    public GameState gameStateNow;

    public enum GameState
    {
        GenerateGrid = 0,
        SpawnHeroes = 1,
        SpawnEnemies = 2,
        HeroesTurn = 3,
        EnemiesTurn = 4
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ChangeState(GameState.GenerateGrid);
    }

    public void ChangeState(GameState newState)
    {
        gameStateNow = newState;

        switch (newState)
        {
            case GameState.GenerateGrid:

                scenario = GenerateScenario_1();

                GridManagerTiles.Instance.GenerateGrid(scenario.Width, scenario.Height, scenario.Squares);

                break;

            case GameState.SpawnHeroes:
                //UnitManager.Instance.SpawnHeroes();

                break;

            case GameState.SpawnEnemies:
                //UnitManager.Instance.SpawnEnemies();

                break;

            case GameState.HeroesTurn:
                break;

            case GameState.EnemiesTurn:
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

    private Scenario GenerateScenario_1()
    {
        int number = 1;
        string title = "scenario 1";

        int width = 8; // tamaño del grid en X (cantidad de casillas de ancho)
        int height = 10;// tamaño del grid en Y (cantidad de casillas de alto)

        string introText = "asdasdas";
        string exitText = "asdas";

        List<Square> squares = new List<Square>();

        // Llenar la matriz con celdas con información inicial
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                squares.Add(new Square() { x = x, y = y });
            }
        }

        // -------------

        // aplicar el tipo de terreno a las celdas

        // con penalización al movimiento
        SetSquareTerrainType(squares, 0, 2, TerrainType.Obstructive);
        SetSquareTerrainType(squares, 0, 3, TerrainType.Obstructive);
        SetSquareTerrainType(squares, 1, 3, TerrainType.Obstructive);

        SetSquareTerrainType(squares, 0, 4, TerrainType.Obstructive);
        SetSquareTerrainType(squares, 0, 5, TerrainType.Obstructive);
        SetSquareTerrainType(squares, 0, 6, TerrainType.Obstructive);

        SetSquareTerrainType(squares, 3, 1, TerrainType.Obstructive);

        SetSquareTerrainType(squares, 3, 4, TerrainType.Obstructive);
        SetSquareTerrainType(squares, 3, 5, TerrainType.Obstructive);
        SetSquareTerrainType(squares, 3, 6, TerrainType.Obstructive);

        SetSquareTerrainType(squares, 5, 0, TerrainType.Obstructive);
        SetSquareTerrainType(squares, 6, 0, TerrainType.Obstructive);
        SetSquareTerrainType(squares, 7, 0, TerrainType.Obstructive);

        SetSquareTerrainType(squares, 7, 2, TerrainType.Obstructive);
        SetSquareTerrainType(squares, 5, 3, TerrainType.Obstructive);
        SetSquareTerrainType(squares, 6, 3, TerrainType.Obstructive);
        SetSquareTerrainType(squares, 7, 3, TerrainType.Obstructive);

        SetSquareTerrainType(squares, 5, 4, TerrainType.Obstructive);

        SetSquareTerrainType(squares, 5, 6, TerrainType.Obstructive);

        SetSquareTerrainType(squares, 5, 9, TerrainType.Obstructive);

        SetSquareTerrainType(squares, 7, 4, TerrainType.Obstructive);
        SetSquareTerrainType(squares, 7, 5, TerrainType.Obstructive);
        SetSquareTerrainType(squares, 7, 6, TerrainType.Obstructive);

        SetSquareTerrainType(squares, 0, 9, TerrainType.Obstructive);
        SetSquareTerrainType(squares, 1, 9, TerrainType.Obstructive);
        SetSquareTerrainType(squares, 2, 9, TerrainType.Obstructive);

        // bloquedas (por fuego)
        SetSquareTerrainType(squares, 4, 2, TerrainType.Hindering);
        SetSquareTerrainType(squares, 1, 4, TerrainType.Hindering);

        // -------------

        // aplicar los tipos de laterales de las celdas (paredes, puertas, ...)

        // lateral:
        // Front = 0,
        // Right = 1,
        // Back = 2,
        // Left = 3

        // tipo de lateral:
        // Nothing = 0,
        // Wall = 1,
        // Door_Open = 2,
        // Door_Closed = 3

        // Llenar las celdas del borde del escenario con paredes
        for (int x = 0; x < width; x++)
        {
            // paredes en la parte inferior de las celdas de la fila inferior del esecenario
            SetSquareSideType(squares, x, 0, SquareSide.Back, SquareSideType.Wall);

            // paredes en la parte superior de las celdas de la fila superior del esecenario
            SetSquareSideType(squares, x, height - 1, SquareSide.Front, SquareSideType.Wall);
        }

        for (int y = 0; y < height; y++)
        {
            // paredes en la parte Left de las celdas de la fila Left del esecenario
            SetSquareSideType(squares, 0, y, SquareSide.Left, SquareSideType.Wall);

            // paredes en la parte Right de las celdas de la fila Right del esecenario
            SetSquareSideType(squares, width - 1, y, SquareSide.Right, SquareSideType.Wall);
        }

        // otras celdas con paredes, puertas, ...
        // va indicado por x, y, desde la 0 (inferior, Left) hasta tamañoEscenarioX, tamañoEscenarioY (superior, Right)

        // fila x, columna 0

        SetSquareSideType(squares, 4, 0, SquareSide.Right, SquareSideType.Wall);
        SetSquareSideType(squares, 5, 0, SquareSide.Left, SquareSideType.Wall);

        // fila x, columna 1

        SetSquareSideType(squares, 4, 1, SquareSide.Right, SquareSideType.Door_Closed);
        SetSquareSideType(squares, 5, 1, SquareSide.Left, SquareSideType.Door_Closed);

        // fila x, columna 2

        SetSquareSideType(squares, 4, 2, SquareSide.Right, SquareSideType.Wall);
        SetSquareSideType(squares, 5, 2, SquareSide.Left, SquareSideType.Wall);

        // fila x, columna 3
        SetSquareSideType(squares, 0, 3, SquareSide.Front, SquareSideType.Wall);
        SetSquareSideType(squares, 1, 3, SquareSide.Front, SquareSideType.Wall);
        SetSquareSideType(squares, 2, 3, SquareSide.Front, SquareSideType.Wall);

        SetSquareSideType(squares, 4, 3, SquareSide.Right, SquareSideType.Wall);
        SetSquareSideType(squares, 5, 3, SquareSide.Front, SquareSideType.Wall);
        SetSquareSideType(squares, 5, 3, SquareSide.Left, SquareSideType.Wall);
        SetSquareSideType(squares, 6, 3, SquareSide.Front, SquareSideType.Wall);
        SetSquareSideType(squares, 7, 3, SquareSide.Front, SquareSideType.Wall);

        // fila x, columna 4

        SetSquareSideType(squares, 0, 4, SquareSide.Back, SquareSideType.Wall);
        SetSquareSideType(squares, 1, 4, SquareSide.Back, SquareSideType.Wall);
        SetSquareSideType(squares, 2, 4, SquareSide.Back, SquareSideType.Wall);

        SetSquareSideType(squares, 5, 4, SquareSide.Right, SquareSideType.Wall);
        SetSquareSideType(squares, 5, 4, SquareSide.Back, SquareSideType.Wall);
        SetSquareSideType(squares, 6, 4, SquareSide.Back, SquareSideType.Wall);
        SetSquareSideType(squares, 7, 4, SquareSide.Back, SquareSideType.Wall);

        // fila x, columna 5

        SetSquareSideType(squares, 5, 5, SquareSide.Right, SquareSideType.Door_Closed);
        SetSquareSideType(squares, 6, 5, SquareSide.Left, SquareSideType.Door_Closed);

        // fila x, columna 6

        SetSquareSideType(squares, 0, 6, SquareSide.Front, SquareSideType.Wall);
        SetSquareSideType(squares, 1, 6, SquareSide.Front, SquareSideType.Wall);
        SetSquareSideType(squares, 2, 6, SquareSide.Front, SquareSideType.Wall);

        SetSquareSideType(squares, 5, 6, SquareSide.Right, SquareSideType.Wall);
        SetSquareSideType(squares, 6, 6, SquareSide.Left, SquareSideType.Wall);

        // fila x, columna 7

        SetSquareSideType(squares, 0, 7, SquareSide.Back, SquareSideType.Wall);
        SetSquareSideType(squares, 1, 7, SquareSide.Back, SquareSideType.Wall);
        SetSquareSideType(squares, 2, 7, SquareSide.Back, SquareSideType.Wall);

        // fila x, columna 8
        // nada

        // fila x, columna 9
        SetSquareSideType(squares, 5, 9, SquareSide.Right, SquareSideType.Wall);
        SetSquareSideType(squares, 6, 9, SquareSide.Left, SquareSideType.Wall);

        // -------------

        // indicar las celdas en zonas cerradas (a las que no pueden acceder los personajes)

        SetSquareIsClosed(squares, 5, 0, true);
        SetSquareIsClosed(squares, 6, 0, true);
        SetSquareIsClosed(squares, 7, 0, true);
        SetSquareIsClosed(squares, 5, 1, true);
        SetSquareIsClosed(squares, 6, 1, true);
        SetSquareIsClosed(squares, 7, 1, true);
        SetSquareIsClosed(squares, 5, 2, true);
        SetSquareIsClosed(squares, 6, 2, true);
        SetSquareIsClosed(squares, 7, 2, true);
        SetSquareIsClosed(squares, 5, 3, true);
        SetSquareIsClosed(squares, 6, 3, true);
        SetSquareIsClosed(squares, 7, 3, true);

        // -------------

        // crear el escenario
        Scenario scenario = new Scenario()
        {
            Number = number,
            Title = title,
            Width = width,
            Height = height,
            IntroText = introText,
            ExitText = exitText,
            Squares = squares
        };

        return scenario;
    }

    #region AsignarValoresCelda

    // asignar un tipo de terreno a una celda
    private void SetSquareTerrainType(List<Square> squares, int x, int y, TerrainType terrainType)
    {
        squares.First(s => s.x == x && s.y == y).terrainType = terrainType;
    }

    // asignar un tipo de lateral a un lateral de una celda
    private void SetSquareSideType(List<Square> squares, int x, int y, SquareSide squareSide, SquareSideType squareSideType)
    {
        squares.First(s => s.x == x && s.y == y).sides[(int)squareSide].squareSideType = squareSideType;
    }

    // asignar a una celda si está en una zona cerradas (a la que no pueden acceder los personajes)
    private void SetSquareIsClosed(List<Square> squares, int x, int y, bool isClosed)
    {
        squares.First(s => s.x == x && s.y == y).isInClosedZone = isClosed;
    }

    #endregion AsignarValoresCelda

    public class Scenario
    {
        public int Number { get; set; }
        public string Title { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string IntroText { get; set; }
        public string ExitText { get; set; }
        public bool IsBetweenScenarios { get; set; } = false;
        public List<Square> Squares { get; set; } = new List<Square>();
    }

    public enum TerrainType
    {
        Normal = 0,
        Hindering = 1,
        Obstructive = 2
    }

    public enum SquareSide
    {
        Front = 0,
        Right = 1,
        Back = 2,
        Left = 3
    }

    public enum SquareSideType
    {
        Nothing = 0,
        Wall = 1,
        Door_Open = 2,
        Door_Closed = 3
    }

    public class Square
    {
        public int x { get; set; }
        public int y { get; set; }
        public TerrainType terrainType { get; set; } = TerrainType.Normal; // por defecto terreno normal

        public bool isFire { get; set; } = false;

        // indica si la celda está en una zona cerrada
        public bool isInClosedZone = false;

        // lateral y tipo de lateral (hay una pared, puerta, ...)

        // lateral:
        // Front = 0,
        // Right = 1,
        // Back = 2,
        // Left = 3

        // tipo de lateral:
        // Nothing = 0,
        // Wall = 1,
        // Door_Open = 2,
        // Door_Closed = 3
        public SquareSideInfo[] sides = new SquareSideInfo[]
        {
            new SquareSideInfo(SquareSide.Front, SquareSideType.Nothing), // por defecto no hay nada en el lateral de la celda
            new SquareSideInfo(SquareSide.Right, SquareSideType.Nothing),
            new SquareSideInfo(SquareSide.Back, SquareSideType.Nothing),
            new SquareSideInfo(SquareSide.Left, SquareSideType.Nothing)
        };
    }

    public class SquareSideInfo
    {
        // el lateral de la celda a la que se le pondrá el tipo de lateral
        // Front = 0,
        // Right = 1,
        // Back = 2,
        // Left = 3
        public SquareSide squareSide;

        // el tipo de lateral que tiene el lateral de la celda
        // Nothing = 0,
        // Wall = 1,
        // Door_Open = 2,
        // Door_Closed = 3
        public SquareSideType squareSideType;

        public SquareSideInfo(SquareSide squareSide, SquareSideType squareSideType)
        {
            this.squareSide = squareSide;
            this.squareSideType = squareSideType;
        }
    }
}