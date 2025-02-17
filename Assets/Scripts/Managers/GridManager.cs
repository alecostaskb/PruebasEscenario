using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public SpriteRenderer imageRenderer; // Referencia al SpriteRenderer de la imagen
    public Vector3 posicionImagenEscenario; // posición de la imagen que representa el escenario
    public Vector2 tamañoImagenEscenario; // tamaño de la imagen que representa el escenario (se usa poara saber el tamaño de las celdas)

    public int tamañoEscenarioX = 8; // Tamaño del grid en X
    public int tamañoEscenarioY = 10; // Tamaño del grid en Y

    // Matriz para almacenar información de cada celda
    public InfoCelda[,] gridCeldasEscenario;

    public float tamañoCelda = 1.0f; // Tamaño de cada celda

    public TextMeshProUGUI textoInfoCelda; // Referencia al objeto de texto en la UI para mostrar información de la celda

    // Matriz para almacenar si una celda está en una zona cerrada
    private bool[,] gridCeldasCerradas;

    // Colores para las zonas bloqueadas y con penalización
    public Color normalColor = Color.white;

    public Color penaltyColor = Color.yellow;
    public Color blockedColor = Color.red;

    private void Start()
    {
        // Ocultar el texto de info de la celda
        textoInfoCelda.gameObject.SetActive(false);

        // Obtener la posición de la imagen
        posicionImagenEscenario = imageRenderer.transform.position;
        tamañoImagenEscenario = imageRenderer.sprite.bounds.size;

        InitializeGrid();
    }

    private void Update()
    {
        HandleMouseHover();
    }

    // Dibujar Gizmos en la vista de escena
    private void OnDrawGizmos()
    {
        if (gridCeldasEscenario == null || imageRenderer == null)
        {
            return;
        }

        for (int celdaX = 0; celdaX < tamañoEscenarioX; celdaX++)
        {
            for (int celdaY = 0; celdaY < tamañoEscenarioY; celdaY++)
            {
                // Calcular la posición de la celda en el mundo
                // se le resta la mitad del tamaño de la imagen porque se calcula desde la posición de la imagen, que es su centro, en 0,0,0
                Vector3 centroCelda = posicionImagenEscenario
                    + new Vector3(
                        (celdaX * tamañoCelda) + (tamañoCelda / 2) - (tamañoImagenEscenario.x / 2),
                        (celdaY * tamañoCelda) + (tamañoCelda / 2) - (tamañoImagenEscenario.y / 2),
                        0
                        );

                // Dibujar un cuadro en la celda según su tipo
                // Mostrar el costo de la celda como texto
                switch (gridCeldasEscenario[celdaX, celdaY].tipoTerreno)
                {
                    case (int)tiposTerreno.Obstructivo:
                        Gizmos.color = penaltyColor;
                        Gizmos.DrawWireCube(centroCelda, new Vector3(tamañoCelda, tamañoCelda, 0.1f));

                        break;

                    case (int)tiposTerreno.Prohibido:
                        Gizmos.color = blockedColor;
                        Gizmos.DrawWireCube(centroCelda, new Vector3(tamañoCelda, tamañoCelda, 0.1f));

                        break;

                    default:
                        break;
                }

                // Mostrar el costo de la celda como texto
                //Handles.Label(centroCelda, gridCeldasEscenario[celdaX, celdaY].tipoTerreno.ToString());

                Handles.Label(centroCelda, GetCellCost(celdaX, celdaY).ToString());
            }
        }
    }

    private void InitializeGrid()
    {
        // Calcular el tamaño del grid en base al tamaño de la imagen
        Vector2 tamañoImagenEscenario = imageRenderer.sprite.bounds.size; // Tamaño de la imagen en unidades

        // tamaño de la celda (las celdas son cuadradas)
        tamañoCelda = tamañoImagenEscenario.x / tamañoEscenarioX;

        // Inicializar la matriz del grid
        gridCeldasEscenario = new InfoCelda[tamañoEscenarioX, tamañoEscenarioY];

        // Llenar la matriz con celdas con información inicial
        for (int x = 0; x < tamañoEscenarioX; x++)
        {
            for (int y = 0; y < tamañoEscenarioY; y++)
            {
                gridCeldasEscenario[x, y] = new InfoCelda();
            }
        }

        // -------------

        // aplicar el tipo de terreno a las celdas

        // bloquedas por fuego
        AsignarTipoTerrenoCelda(4, 2, tiposTerreno.Prohibido);
        AsignarTipoTerrenoCelda(1, 4, tiposTerreno.Prohibido);

        // con penalización al movimiento
        AsignarTipoTerrenoCelda(0, 2, tiposTerreno.Obstructivo);
        AsignarTipoTerrenoCelda(0, 3, tiposTerreno.Obstructivo);
        AsignarTipoTerrenoCelda(1, 3, tiposTerreno.Obstructivo);

        AsignarTipoTerrenoCelda(0, 4, tiposTerreno.Obstructivo);
        AsignarTipoTerrenoCelda(0, 5, tiposTerreno.Obstructivo);
        AsignarTipoTerrenoCelda(0, 6, tiposTerreno.Obstructivo);

        AsignarTipoTerrenoCelda(3, 1, tiposTerreno.Obstructivo);

        AsignarTipoTerrenoCelda(3, 4, tiposTerreno.Obstructivo);
        AsignarTipoTerrenoCelda(3, 5, tiposTerreno.Obstructivo);
        AsignarTipoTerrenoCelda(3, 6, tiposTerreno.Obstructivo);

        AsignarTipoTerrenoCelda(5, 0, tiposTerreno.Obstructivo);
        AsignarTipoTerrenoCelda(6, 0, tiposTerreno.Obstructivo);
        AsignarTipoTerrenoCelda(7, 0, tiposTerreno.Obstructivo);

        AsignarTipoTerrenoCelda(7, 2, tiposTerreno.Obstructivo);
        AsignarTipoTerrenoCelda(5, 3, tiposTerreno.Obstructivo);
        AsignarTipoTerrenoCelda(6, 3, tiposTerreno.Obstructivo);
        AsignarTipoTerrenoCelda(7, 3, tiposTerreno.Obstructivo);

        AsignarTipoTerrenoCelda(5, 4, tiposTerreno.Obstructivo);

        AsignarTipoTerrenoCelda(5, 6, tiposTerreno.Obstructivo);

        AsignarTipoTerrenoCelda(5, 9, tiposTerreno.Obstructivo);

        AsignarTipoTerrenoCelda(7, 4, tiposTerreno.Obstructivo);
        AsignarTipoTerrenoCelda(7, 5, tiposTerreno.Obstructivo);
        AsignarTipoTerrenoCelda(7, 6, tiposTerreno.Obstructivo);

        AsignarTipoTerrenoCelda(0, 9, tiposTerreno.Obstructivo);
        AsignarTipoTerrenoCelda(1, 9, tiposTerreno.Obstructivo);
        AsignarTipoTerrenoCelda(2, 9, tiposTerreno.Obstructivo);

        // -------------

        // aplicar laterales de las celdas

        // lateral:
        // Arriba = 0,
        // Derecha = 1,
        // Abajo = 2,
        // Izquierda = 3

        // tipo de lateral:
        // Nada = 0,
        // Pared = 1,
        // PuertaAbierta = 2,
        // PuertaCerrada = 3

        // Llenar las celdas del borde del escenario con paredes
        for (int x = 0; x < tamañoEscenarioX; x++)
        {
            // paredes en la parte inferior de las celdas de la fila inferior del esecenario
            AsignarTipoLateralCelda(x, 0, lateralesCelda.Abajo, tiposLateralCelda.Pared);

            // paredes en la parte superior de las celdas de la fila superior del esecenario
            AsignarTipoLateralCelda(x, tamañoEscenarioY - 1, lateralesCelda.Arriba, tiposLateralCelda.Pared);
        }

        for (int y = 0; y < tamañoEscenarioY; y++)
        {
            // paredes en la parte izquierda de las celdas de la fila izquierda del esecenario
            AsignarTipoLateralCelda(0, y, lateralesCelda.Izquierda, tiposLateralCelda.Pared);

            // paredes en la parte derecha de las celdas de la fila derecha del esecenario
            AsignarTipoLateralCelda(tamañoEscenarioX - 1, y, lateralesCelda.Derecha, tiposLateralCelda.Pared);
        }

        // otras celdas con paredes, puertas, ...
        // va indicado por x, y, desde la 0 (inferior, izquierda) hasta tamañoEscenarioX, tamañoEscenarioY (superior, derecha)

        // fila x, columna 0

        AsignarTipoLateralCelda(4, 0, lateralesCelda.Derecha, tiposLateralCelda.Pared);
        AsignarTipoLateralCelda(5, 0, lateralesCelda.Izquierda, tiposLateralCelda.Pared);

        // fila x, columna 1

        AsignarTipoLateralCelda(4, 1, lateralesCelda.Derecha, tiposLateralCelda.PuertaCerrada);
        AsignarTipoLateralCelda(5, 1, lateralesCelda.Izquierda, tiposLateralCelda.PuertaCerrada);

        // fila x, columna 2

        AsignarTipoLateralCelda(4, 2, lateralesCelda.Derecha, tiposLateralCelda.Pared);
        AsignarTipoLateralCelda(5, 2, lateralesCelda.Izquierda, tiposLateralCelda.Pared);

        // fila x, columna 3
        AsignarTipoLateralCelda(0, 3, lateralesCelda.Arriba, tiposLateralCelda.Pared);
        AsignarTipoLateralCelda(1, 3, lateralesCelda.Arriba, tiposLateralCelda.Pared);
        AsignarTipoLateralCelda(2, 3, lateralesCelda.Arriba, tiposLateralCelda.Pared);

        AsignarTipoLateralCelda(4, 3, lateralesCelda.Derecha, tiposLateralCelda.Pared);
        AsignarTipoLateralCelda(5, 3, lateralesCelda.Arriba, tiposLateralCelda.Pared);
        AsignarTipoLateralCelda(5, 3, lateralesCelda.Izquierda, tiposLateralCelda.Pared);
        AsignarTipoLateralCelda(6, 3, lateralesCelda.Arriba, tiposLateralCelda.Pared);
        AsignarTipoLateralCelda(7, 3, lateralesCelda.Arriba, tiposLateralCelda.Pared);

        // fila x, columna 4

        AsignarTipoLateralCelda(0, 4, lateralesCelda.Abajo, tiposLateralCelda.Pared);
        AsignarTipoLateralCelda(1, 4, lateralesCelda.Abajo, tiposLateralCelda.Pared);
        AsignarTipoLateralCelda(2, 4, lateralesCelda.Abajo, tiposLateralCelda.Pared);

        AsignarTipoLateralCelda(5, 4, lateralesCelda.Derecha, tiposLateralCelda.Pared);
        AsignarTipoLateralCelda(5, 4, lateralesCelda.Abajo, tiposLateralCelda.Pared);
        AsignarTipoLateralCelda(6, 4, lateralesCelda.Abajo, tiposLateralCelda.Pared);
        AsignarTipoLateralCelda(7, 4, lateralesCelda.Abajo, tiposLateralCelda.Pared);

        // fila x, columna 5

        AsignarTipoLateralCelda(5, 5, lateralesCelda.Derecha, tiposLateralCelda.PuertaCerrada);
        AsignarTipoLateralCelda(6, 5, lateralesCelda.Izquierda, tiposLateralCelda.PuertaCerrada);

        // fila x, columna 6

        AsignarTipoLateralCelda(0, 6, lateralesCelda.Arriba, tiposLateralCelda.Pared);
        AsignarTipoLateralCelda(1, 6, lateralesCelda.Arriba, tiposLateralCelda.Pared);
        AsignarTipoLateralCelda(2, 6, lateralesCelda.Arriba, tiposLateralCelda.Pared);

        AsignarTipoLateralCelda(5, 6, lateralesCelda.Derecha, tiposLateralCelda.Pared);
        AsignarTipoLateralCelda(6, 6, lateralesCelda.Izquierda, tiposLateralCelda.Pared);

        // fila x, columna 7

        AsignarTipoLateralCelda(0, 7, lateralesCelda.Abajo, tiposLateralCelda.Pared);
        AsignarTipoLateralCelda(1, 7, lateralesCelda.Abajo, tiposLateralCelda.Pared);
        AsignarTipoLateralCelda(2, 7, lateralesCelda.Abajo, tiposLateralCelda.Pared);

        // fila x, columna 8
        // nada

        // fila x, columna 9
        AsignarTipoLateralCelda(5, 9, lateralesCelda.Derecha, tiposLateralCelda.Pared);
        AsignarTipoLateralCelda(6, 9, lateralesCelda.Izquierda, tiposLateralCelda.Pared);

        // -------------

        // indica las celdas en zonas cerradas (a las que no pueden acceder los personajes)

        AsignarCierreCelda(5, 0, true);
        AsignarCierreCelda(6, 0, true);
        AsignarCierreCelda(7, 0, true);
        AsignarCierreCelda(5, 1, true);
        AsignarCierreCelda(6, 1, true);
        AsignarCierreCelda(7, 1, true);
        AsignarCierreCelda(5, 2, true);
        AsignarCierreCelda(6, 2, true);
        AsignarCierreCelda(7, 2, true);
        AsignarCierreCelda(5, 3, true);
        AsignarCierreCelda(6, 3, true);
        AsignarCierreCelda(7, 3, true);
    }

    // Método para mostrar información de la celda bajo el cursor
    private void HandleMouseHover()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0; // Asegurarse de que la Z sea 0 en 2D

        Vector2Int celda = GetCellFromWorldPosition(mouseWorldPos);

        if (celda.x >= 0 && celda.x < tamañoEscenarioX && celda.y >= 0 && celda.y < tamañoEscenarioY)
        {
            // Mostrar información de la celda
            string info = $"Celda: ({celda.x}, {celda.y})" + Environment.NewLine
                + (IsCellWalkable(celda.x, celda.y) ? "Transitable" : "Bloqueada") + Environment.NewLine
                + ((tiposTerreno)gridCeldasEscenario[celda.x, celda.y].tipoTerreno).ToString() + Environment.NewLine
                + $"Costo: {GetCellCost(celda.x, celda.y)}";

            textoInfoCelda.text = info;
            textoInfoCelda.gameObject.SetActive(true);

            // Posicionar el texto cerca del cursor
            //textoInfoCelda.transform.position = Input.mousePosition + new Vector3(20, -20, 0); // Ajusta el offset según sea necesario
        }
        else
        {
            // Ocultar el texto si el cursor no está sobre una celda válida
            textoInfoCelda.gameObject.SetActive(false);
        }
    }

    // Método para verificar si una celda es transitable
    public bool IsCellWalkable(int x, int y)
    {
        if (x >= 0 && x < tamañoEscenarioX && y >= 0 && y < tamañoEscenarioY)
        {
            return gridCeldasEscenario[x, y].tipoTerreno != (int)tiposTerreno.Prohibido; // 1 = bloqueada
        }

        return false;
    }

    // Método para obtener el costo de una celda (es el valor en la enumeración de tipos de terreno)
    public int GetCellCost(int x, int y)
    {
        if (x >= 0 && x < tamañoEscenarioX && y >= 0 && y < tamañoEscenarioY)
        {
            return gridCeldasEscenario[x, y].tipoTerreno;
        }

        return 0;
    }

    // Método para calcular la distancia en casillas entre dos celdas ( distancia Manhattan, también conocida como distancia L1)
    public int CalculateCellDistance(Vector2Int cellA, Vector2Int cellB)
    {
        return Mathf.Abs(cellA.x - cellB.x) + Mathf.Abs(cellA.y - cellB.y);
    }

    // Método para calcular la posición central de una celda
    public Vector3 GetCellCenterPosition(int x, int y)
    {
        // Calcular la posición central de la celda
        Vector3 cellCenter = posicionImagenEscenario + new Vector3(
            x * tamañoCelda + tamañoCelda / 2 - tamañoImagenEscenario.x / 2,
            y * tamañoCelda + tamañoCelda / 2 - tamañoImagenEscenario.y / 2,
            0
         );

        return cellCenter;
    }

    // Método para obtener la celda bajo una posición en el mundo
    public Vector2Int GetCellFromWorldPosition(Vector3 worldPosition)
    {
        Vector3 imagePosition = transform.position; // Posición del grid
        Vector2 imageSize = new Vector2(tamañoEscenarioX * tamañoCelda, tamañoEscenarioY * tamañoCelda); // Tamaño del grid

        int x = Mathf.FloorToInt((worldPosition.x - imagePosition.x + imageSize.x / 2) / tamañoCelda);
        int y = Mathf.FloorToInt((worldPosition.y - imagePosition.y + imageSize.y / 2) / tamañoCelda);

        return new Vector2Int(x, y);
    }

    #region AsignarValoresCelda

    // asignar un tipo de terreno a una celda
    public void AsignarTipoTerrenoCelda(int x, int y, tiposTerreno tipoTerreno)
    {
        if (x >= 0 && x < tamañoEscenarioX && y >= 0 && y < tamañoEscenarioY)
        {
            gridCeldasEscenario[x, y].tipoTerreno = (int)tipoTerreno;
        }
    }

    // asignar a una celda si está en una zona cerradas (a la que no pueden acceder los personajes)
    public void AsignarCierreCelda(int x, int y, bool cerrada)
    {
        if (x >= 0 && x < tamañoEscenarioX && y >= 0 && y < tamañoEscenarioY)
        {
            gridCeldasEscenario[x, y].estaEnZonaCerrada = cerrada;
        }
    }

    // asignar un tipo de lateral a un lateral de una celda
    public void AsignarTipoLateralCelda(int x, int y, lateralesCelda lateralCelda, tiposLateralCelda tipoLateralCelda)
    {
        if (x >= 0 && x < tamañoEscenarioX && y >= 0 && y < tamañoEscenarioY)
        {
            gridCeldasEscenario[x, y].laterales[(int)lateralCelda].tipoLateralCelda = tipoLateralCelda;
        }
    }

    #endregion AsignarValoresCelda

    #region ComprobarLateralesCelda

    // verificar si un lateral de una celda está cerrado (es de tipo <> Nada)
    public bool CeldaTieneCierreLateral(int x, int y, lateralesCelda lateralCelda)
    {
        bool res = false;

        if (x >= 0 && x < tamañoEscenarioX && y >= 0 && y < tamañoEscenarioY)
        {
            res = gridCeldasEscenario[x, y].laterales[(int)lateralCelda].tipoLateralCelda != tiposLateralCelda.Nada;
        }

        return res;
    }

    // verificar si un lateral de una celda está cerrado con una pared
    public bool CeldaTieneCierreLateralPared(int x, int y, lateralesCelda lateralCelda)
    {
        bool res = false;

        if (x >= 0 && x < tamañoEscenarioX && y >= 0 && y < tamañoEscenarioY)
        {
            res = gridCeldasEscenario[x, y].laterales[(int)lateralCelda].tipoLateralCelda == tiposLateralCelda.Pared;
        }

        return res;
    }

    // verificar si un lateral de una celda está cerrado con una puerta
    public bool CeldaTieneCierreLateralPuerta(int x, int y, lateralesCelda lateralCelda)
    {
        bool res = false;

        if (x >= 0 && x < tamañoEscenarioX && y >= 0 && y < tamañoEscenarioY)
        {
            res = gridCeldasEscenario[x, y].laterales[(int)lateralCelda].tipoLateralCelda == tiposLateralCelda.PuertaCerrada;
        }

        return res;
    }

    // verificar si hay una separación entre dos celdas adyacentes
    public bool CeldasTienenSeparacion(Vector2Int fromCell, Vector2Int toCell)
    {
        if (fromCell.x == toCell.x)
        {
            // Movimiento vertical
            if (fromCell.y < toCell.y)
            {
                // Movimiento hacia arriba: verificar el lado superior de la celda actual
                return CeldaTieneCierreLateral(fromCell.x, fromCell.y, lateralesCelda.Arriba);
            }
            else
            {
                // Movimiento hacia abajo: verificar el lado inferior de la celda actual
                return CeldaTieneCierreLateral(fromCell.x, fromCell.y, lateralesCelda.Abajo);
            }
        }
        else if (fromCell.y == toCell.y)
        {
            // Movimiento horizontal
            if (fromCell.x < toCell.x)
            {
                // Movimiento hacia la derecha: verificar el lado derecho de la celda actual
                return CeldaTieneCierreLateral(fromCell.x, fromCell.y, lateralesCelda.Derecha);
            }
            else
            {
                // Movimiento hacia la izquierda: verificar el lado izquierdo de la celda actual
                return CeldaTieneCierreLateral(fromCell.x, fromCell.y, lateralesCelda.Izquierda);
            }
        }

        // Si las celdas no son adyacentes, no hay pared
        return false;
    }

    #endregion ComprobarLateralesCelda

    // Método para verificar si una celda está en una zona cerrada
    public bool IsCellInClosedArea(Vector2Int cell)
    {
        // Si la celda no es transitable, no puede estar en una zona cerrada (ya está bloqueada)
        if (!IsCellWalkable(cell.x, cell.y))
        {
            return true;
        }

        // Usar BFS para verificar si la celda está en una zona cerrada
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        queue.Enqueue(cell);
        visited.Add(cell);

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            // Verificar si la celda actual está en el borde del grid
            if (current.x == 0 || current.x == tamañoEscenarioX - 1 || current.y == 0 || current.y == tamañoEscenarioY - 1)
            {
                // Si la celda está en el borde, no está en una zona cerrada
                return false;
            }

            // Explorar las celdas adyacentes
            Vector2Int[] directions = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

            foreach (Vector2Int dir in directions)
            {
                Vector2Int neighbor = current + dir;

                // Verificar si la celda vecina es transitable y no ha sido visitada
                if (IsCellWalkable(neighbor.x, neighbor.y) && !visited.Contains(neighbor) && !CeldasTienenSeparacion(current, neighbor))
                {
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                }
            }
        }

        // Si no se encontró una salida al borde, la celda está en una zona cerrada
        return true;
    }

    // Método para calcular las zonas cerradas
    private void CalculateClosedAreas()
    {
        // Usar BFS para marcar todas las celdas transitables conectadas al borde del grid
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        // Agregar todas las celdas en el borde del grid a la cola
        for (int x = 0; x < tamañoEscenarioX; x++)
        {
            for (int y = 0; y < tamañoEscenarioY; y++)
            {
                if (x == 0 || x == tamañoEscenarioX - 1 || y == 0 || y == tamañoEscenarioY - 1)
                {
                    if (IsCellWalkable(x, y))
                    {
                        queue.Enqueue(new Vector2Int(x, y));
                        visited.Add(new Vector2Int(x, y));
                    }
                }
            }
        }

        // Explorar todas las celdas transitables conectadas al borde
        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            // Explorar celdas adyacentes
            Vector2Int[] directions = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

            foreach (Vector2Int dir in directions)
            {
                Vector2Int neighbor = current + dir;

                // Verificar si la celda vecina está dentro del grid
                if (neighbor.x >= 0 && neighbor.x < tamañoEscenarioX && neighbor.y >= 0 && neighbor.y < tamañoEscenarioY)
                {
                    // Verificar si la celda vecina es transitable, no ha sido visitada y no hay una pared en el camino
                    if (IsCellWalkable(neighbor.x, neighbor.y) && !visited.Contains(neighbor) && !CeldasTienenSeparacion(current, neighbor))
                    {
                        queue.Enqueue(neighbor);
                        visited.Add(neighbor);
                    }
                }
            }
        }

        // Marcar las celdas no visitadas como cerradas
        for (int x = 0; x < tamañoEscenarioX; x++)
        {
            for (int y = 0; y < tamañoEscenarioY; y++)
            {
                if (IsCellWalkable(x, y) && !visited.Contains(new Vector2Int(x, y)))
                {
                    gridCeldasCerradas[x, y] = true;
                }
            }
        }
    }

    // Método para verificar si una celda está en una zona cerrada
    public bool IsCellClosed(int x, int y)
    {
        if (x >= 0 && x < tamañoEscenarioX && y >= 0 && y < tamañoEscenarioY)
        {
            return gridCeldasEscenario[x, y].estaEnZonaCerrada;
        }
        return false;
    }

    // obtener las celdas por las que pasaría una línea entre el centro de una celda inicial y el centro de una celda final
    public List<Vector2Int> GetCellsAlongArrow(Vector2Int startCell, Vector2Int endCell)
    {
        List<Vector2Int> cells = new List<Vector2Int>();

        // Coordenadas de la celda inicial
        int xInicial = startCell.x;
        int yInicial = startCell.y;

        // Coordenadas de la celda final
        int xFinal = endCell.x;
        int yFinal = endCell.y;

        // Diferencias absolutas entre las coordenadas
        int diferenciaX = Mathf.Abs(xFinal - xInicial);
        int diferenciaY = Mathf.Abs(yFinal - yInicial);

        // Dirección del movimiento (1 o -1, dependiendo de la dirección).
        // direccionMovimientoX = 1 si hay que moverse hacia la derecha, -1 si hay que moverse hacia la izquierda
        // direccionMovimientoY = 1 si hay que moverse hacia arriba, -1 si hay que moverse hacia abajo
        int direccionMovimientoX = (xInicial < xFinal) ? 1 : -1;
        int direccionMovimientoY = (yInicial < yFinal) ? 1 : -1;

        // Variable de error para decidir cuándo mover en la dirección X o Y
        // diferencia entre la distancia en X y Y
        int err = diferenciaX - diferenciaY;

        // Agregar la celda inicial a la lista
        cells.Add(new Vector2Int(xInicial, yInicial));

        while (true)
        {
            // Si llegamos a la celda final, salir del bucle
            if (xInicial == xFinal && yInicial == yFinal)
            {
                break;
            }

            // Calcular el doble del error acumulado
            int error2 = 2 * err;

            // Si el error es mayor que -diferenciaY, moverse en la dirección X
            if (error2 > -diferenciaY)
            {
                err -= diferenciaY;
                xInicial += direccionMovimientoX;
            }

            // Si el error es menor que diferenciaX, moverse en la dirección Y
            if (error2 < diferenciaX)
            {
                err += diferenciaX;
                yInicial += direccionMovimientoY;

                //cells.Add(new Vector2Int(xInicial, yInicial));
            }

            // Agregar la celda actual a la lista
            cells.Add(new Vector2Int(xInicial, yInicial));

            // Agregar la celda actual nuevamente si hay un cambio simultáneo en X e Y
            //if (error2 > -diferenciaY && error2 < diferenciaX)
            //{
            //    cells.Add(new Vector2Int(xInicial, yInicial));
            //}
        }

        Debug.Log($"Celdas entre: {startCell} y {endCell}:");
        foreach (Vector2Int cell in cells)
        {
            Debug.Log($"Celda: {cell}");
        }

        return cells;
    }

    // obtener las celdas por las que pasaría una línea entre el centro de una celda inicial y el centro de una celda final
    public List<Vector2Int> GetCellsAlongArrow2(Vector2Int startCell, Vector2Int endCell)
    {
        List<Vector2Int> cells = new List<Vector2Int>();

        // Coordenadas de la celda inicial
        int xInicial = startCell.x;
        int yInicial = startCell.y;

        // Coordenadas de la celda final
        int xFinal = endCell.x;
        int yFinal = endCell.y;

        // Diferencias absolutas entre las coordenadas
        int diferenciaX = Mathf.Abs(xFinal - xInicial);
        int diferenciaY = Mathf.Abs(yFinal - yInicial);

        // Dirección del movimiento (1 o -1, dependiendo de la dirección).
        // direccionMovimientoX = 1 si hay que moverse hacia la derecha, -1 si hay que moverse hacia la izquierda
        // direccionMovimientoY = 1 si hay que moverse hacia arriba, -1 si hay que moverse hacia abajo
        int direccionMovimientoX = diferenciaX > 0 ? 1 : -1;
        int direccionMovimientoY = diferenciaY > 0 ? 1 : -1;

        // Agregar la celda inicial a la lista
        cells.Add(new Vector2Int(xInicial, yInicial));

        for (int ix = 0, iy = 0; ix < diferenciaX || iy < diferenciaY;)
        {
            // Variable para decidir cuándo mover en la dirección X o Y
            // diferencia entre la distancia en X y Y
            int decision = (1 + 2 * ix) * diferenciaY - (1 + 2 * iy) * diferenciaX;

            if (decision == 0)
            {
                // diagonal
                xInicial += direccionMovimientoX;
                yInicial += direccionMovimientoY;

                ix++;
                iy++;
            }
            else if (decision < 0)
            {
                // horizontal
                xInicial += direccionMovimientoX;

                ix++;
            }
            else
            {
                // vertical
                yInicial += direccionMovimientoY;

                iy++;
            }

            // Agregar la celda actual a la lista
            cells.Add(new Vector2Int(xInicial, yInicial));
        }

        Debug.Log($"Celdas entre: {startCell} y {endCell}:");
        foreach (Vector2Int cell in cells)
        {
            Debug.Log($"Celda: {cell}");
        }

        return cells;
    }

    // obtener las celdas por las que pasaría una línea entre el centro de una celda inicial y el centro de una celda final
    public List<Vector2Int> GetCellsAlongArrow3(Vector2Int startCell, Vector2Int endCell)
    {
        List<Vector2Int> cells = new List<Vector2Int>();

        // Coordenadas de la celda inicial
        int xInicial = startCell.x;
        int yInicial = startCell.y;

        // Coordenadas de la celda final
        int xFinal = endCell.x;
        int yFinal = endCell.y;

        // Diferencias absolutas entre las coordenadas
        int diferenciaX = Mathf.Abs(xFinal - xInicial);
        int diferenciaY = Mathf.Abs(yFinal - yInicial);

        // the step on y and x axis
        int avanceX;
        int avanceY;

        int error;           // the error accumulated during the increment
        int errorprev;       // *vision the previous value of the error variable

        if (diferenciaY < 0)
        {
            avanceY = -1;
            diferenciaY = -diferenciaY;
        }
        else
        {
            avanceY = 1;
        }

        if (diferenciaX < 0)
        {
            avanceX = -1;
            diferenciaX = -diferenciaX;
        }
        else
        {
            avanceX = 1;
        }

        // compulsory variables: the double values of diferenciaX and diferenciaY
        // work with double values for full precision
        int dobleDiferenciaX = 2 * diferenciaX;
        int dobleDiferenciaY = 2 * diferenciaY;

        // Agregar la celda inicial a la lista
        cells.Add(new Vector2Int(xInicial, yInicial));

        if (dobleDiferenciaX >= dobleDiferenciaY)
        {
            // first octant (0 <= slope <= 1)
            // compulsory initialization (even for errorprev, needed when diferenciaX==diferenciaY)
            errorprev = error = diferenciaX;  // start in the middle of the square

            for (int i = 0; i < diferenciaX; i++)
            {  // do not use the first point (already done)
                xInicial += avanceX;
                error += dobleDiferenciaY;

                if (error > dobleDiferenciaX)
                {  // increment y if AFTER the middle ( > )
                    yInicial += avanceY;
                    error -= dobleDiferenciaX;

                    // three cases (octant == right->right-top for directions below):
                    if (error + errorprev < dobleDiferenciaX)  // bottom square also
                    {
                        //POINT(y - ystep, x);

                        yInicial -= avanceY;
                    }
                    else if (error + errorprev > dobleDiferenciaX)  // left square also
                    {
                        //POINT(y, x - xstep);

                        xInicial -= avanceX;
                    }
                    else
                    {  // corner: bottom and left squares also
                        //POINT(y - ystep, x);
                        //POINT(y, x - xstep);

                        xInicial -= avanceX;
                        yInicial -= avanceY;
                    }
                }

                //POINT(y, x);

                // Agregar la celda actual a la lista
                cells.Add(new Vector2Int(xInicial, yInicial));

                errorprev = error;
            }
        }
        else
        {  // the same as above
            errorprev = error = diferenciaY;

            for (int i = 0; i < diferenciaY; i++)
            {
                yInicial += avanceY;
                error += dobleDiferenciaX;

                if (error > dobleDiferenciaY)
                {
                    xInicial += avanceX;
                    error -= dobleDiferenciaY;
                    if (error + errorprev < dobleDiferenciaY)
                    {
                        //POINT(y, x - xstep);

                        xInicial -= avanceX;
                    }
                    else if (error + errorprev > dobleDiferenciaY)
                    {
                        //POINT(y - ystep, x);

                        yInicial -= avanceY;
                    }
                    else
                    {
                        //POINT(y, x - xstep);
                        //POINT(y - ystep, x);

                        xInicial -= avanceX;
                        yInicial -= avanceY;
                    }
                }

                //POINT(y, x);

                // Agregar la celda actual a la lista
                cells.Add(new Vector2Int(xInicial, yInicial));

                errorprev = error;
            }
        }

        Debug.Log($"Celdas entre: {startCell} y {endCell}:");
        foreach (Vector2Int cell in cells)
        {
            Debug.Log($"Celda: {cell}");
        }

        return cells;
    }

    // obtener las celdas por las que pasaría una línea entre el centro de una celda inicial y el centro de una celda final
    public List<Vector2Int> GetCellsAlongArrow4(Vector2Int startCell, Vector2Int endCell)
    {
        // Lista para almacenar las celdas por las que pasa la flecha
        List<Vector2Int> cells = new List<Vector2Int>();

        // Coordenadas iniciales (celda del personaje)
        int x0 = startCell.x;
        int y0 = startCell.y;

        // Coordenadas finales (celda del cursor)
        int x1 = endCell.x;
        int y1 = endCell.y;

        // Calcular las diferencias en las coordenadas X e Y
        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);

        // Determinar la dirección del movimiento en X e Y
        int sx = (x0 < x1) ? 1 : -1;
        int sy = (y0 < y1) ? 1 : -1;

        // Inicializar el error
        int err = dx - dy;

        // Bucle principal para recorrer las celdas
        while (true)
        {
            // Agregar la celda actual a la lista
            cells.Add(new Vector2Int(x0, y0));

            // Si llegamos a la celda final, salir del bucle
            if (x0 == x1 && y0 == y1)
                break;

            // Calcular el doble del error acumulado
            int e2 = 2 * err;

            // Si el error es mayor que -dy, moverse en la dirección X
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }

            // Si el error es menor que dx, moverse en la dirección Y
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;

                // Si la línea atraviesa una esquina, agregar ambas celdas adyacentes
                if (e2 > -dy && e2 < dx)
                {
                    cells.Add(new Vector2Int(x0 - sx, y0));
                    cells.Add(new Vector2Int(x0, y0 - sy));
                }
            }
        }

        Debug.Log($"Celdas entre: {startCell} y {endCell}:");

        foreach (Vector2Int cell in cells)
        {
            Debug.Log($"Celda: {cell}");
        }

        return cells;
    }

    // Implementación de un algoritmo A* para encontrar el camino en la cuadrícula.
    public List<Vector2Int> EncontrarCamino(Vector2Int celdaInicial, Vector2Int celdaFinal)
    {
        List<Vector2Int> celdasRuta = new List<Vector2Int>();

        if (celdaInicial == celdaFinal)
        {
            celdasRuta.Add(celdaInicial);

            return celdasRuta;
        }

        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        Dictionary<Vector2Int, int> costSoFar = new Dictionary<Vector2Int, int>();
        PriorityQueue<Vector2Int> frontier = new PriorityQueue<Vector2Int>();

        frontier.Enqueue(celdaInicial, 0);
        costSoFar[celdaInicial] = 0;

        while (frontier.Count > 0)
        {
            Vector2Int current = frontier.Dequeue();

            if (current == celdaFinal)
            {
                Vector2Int cur = celdaFinal;

                while (cur != celdaInicial)
                {
                    celdasRuta.Add(cur);

                    cur = cameFrom[cur];
                }

                celdasRuta.Add(celdaInicial);

                celdasRuta.Reverse();

                return celdasRuta;
            }

            List<Vector2Int> celdasVecinas = ObtenerCeldasVecinas(current, false);

            foreach (Vector2Int next in celdasVecinas)
            {
                Vector2Int nextTile = next;

                int newCost = costSoFar[current] + 1;

                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                {
                    costSoFar[next] = newCost;
                    int priority = newCost + Mathf.Abs(next.x - celdaFinal.x) + Mathf.Abs(next.y - celdaFinal.y);

                    frontier.Enqueue(next, priority);
                    cameFrom[next] = current;
                }
            }
        }

        return null;  // No se encontró camino
    }

    // Devuelve las celdas vecinas a una celda origen
    private List<Vector2Int> ObtenerCeldasVecinas(Vector2Int celdaOrigen, bool soloOrtogonal)
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
}

public class PriorityQueue<T>
{
    private List<KeyValuePair<T, int>> elements = new List<KeyValuePair<T, int>>();

    public int Count
    {
        get
        {
            return elements.Count;
        }
    }

    public void Enqueue(T item, int priority)
    {
        elements.Add(new KeyValuePair<T, int>(item, priority));
    }

    public T Dequeue()
    {
        int bestIndex = 0;

        for (int i = 0; i < elements.Count; i++)
        {
            if (elements[i].Value < elements[bestIndex].Value)
            {
                bestIndex = i;
            }
        }

        T bestItem = elements[bestIndex].Key;
        elements.RemoveAt(bestIndex);

        return bestItem;
    }
}