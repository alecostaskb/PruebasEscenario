using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoveObjectOnGrid : MonoBehaviour
{
    public GridManager gridManager; // Referencia al GridManager

    public SpriteRenderer imageRenderer; // Referencia al SpriteRenderer de la imagen
    private Vector3 posicionImagenEscenario;
    private Vector2 tamañoImagenEscenario;

    private Vector3 targetPosition;
    public float tamañoCelda = 1.0f; // Tamaño de la celda
    public float moveSpeed = 5.0f; // Velocidad de movimiento

    public Vector2Int direccionVisionPersonaje; // Por defecto, el personaje mira hacia arriba

    //public float anguloVisionPersonaje = 90f; // Ángulo de visión en grados
    public int alcanceVisionPersonaje = 3; // Rango de la zona delantera

    public TextMeshProUGUI textoInfoPersonaje; // Referencia al objeto de texto en la UI para mostrar información del personaje

    private void Start()
    {
        if (gridManager == null)
        {
            Debug.Log("gridManager no existe");

            return;
        }

        // Ocultar el texto de info del personaje
        textoInfoPersonaje.gameObject.SetActive(false);

        // Obtener la posición de la imagen
        posicionImagenEscenario = gridManager.posicionImagenEscenario;
        tamañoImagenEscenario = gridManager.tamañoImagenEscenario;

        tamañoCelda = gridManager.tamañoCelda;
        targetPosition = transform.position;

        // dirección en la que está mirando el personaje (se usa para calcular su arco)
        direccionVisionPersonaje = Vector2Int.up;

        // Crear el objeto Image en el Canvas
        CreateArrowGame();
    }

    private void Update()
    {
        HandleMouseClick();

        HandleMouseHover();
    }

    // Manejar el clic del ratón para mover el personaje
    private void HandleMouseClick()
    {
        if (Input.GetMouseButtonDown(0)) // Al hacer clic
        {
            // Obtener la posición del clic en el mundo 2D
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0; // Asegurarse de que la Z sea 0 en 2D

            // Celdas, actual y destino
            //Vector2Int currentCell = gridManager.GetCellFromWorldPosition(transform.position);
            Vector2Int targetCell = gridManager.GetCellFromWorldPosition(mouseWorldPos);

            // Verificar si la celda de destino es válida
            //if (x >= 0 && x < gridManager.tamañoEscenarioX && y >= 0 && y < gridManager.tamañoEscenarioY)
            if (targetCell.x >= 0 && targetCell.x < gridManager.tamañoEscenarioX
                && targetCell.y >= 0 && targetCell.y < gridManager.tamañoEscenarioY)
            {
                // Obtener el costo de la celda (opcional)
                int cellCost = gridManager.GetCellCost(targetCell.x, targetCell.y);
                Debug.Log($"X: {targetCell.x}, Y: {targetCell.y}, costo: " + cellCost);

                // Verificar si la celda es transitable
                //if (gridManager.IsCellWalkable(targetCell.x, targetCell.y) && !gridManager.IsCellInClosedArea(targetCell))
                if (gridManager.IsCellWalkable(targetCell.x, targetCell.y) && !gridManager.IsCellClosed(targetCell.x, targetCell.y))
                {
                    // Calcular la posición central de la celda de destino
                    targetPosition = gridManager.GetCellCenterPosition(targetCell.x, targetCell.y);

                    // Mover suavemente el personaje a la posición objetivo
                    Debug.Log($"Objeto se moverá a la celda: {targetCell.x}, {targetCell.y}");
                    //transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
                    transform.position = targetPosition;
                }
                else
                {
                    Debug.Log("Celda bloqueada");
                    MostrarInfoPersonaje("No puedo mover allí:" + Environment.NewLine
                        + "Celda bloqueada");
                }
            }
            else
            {
                Debug.Log("Celda fuera del grid");
                MostrarInfoPersonaje("No puedo mover allí:" + Environment.NewLine
                    + "Celda fuera del grid");
            }

            // Ocultar el texto después de 2 segundos
            Invoke(nameof(OcultarInfoPersonaje), 2f);
        }
    }

    // Manejar el hover del ratón para mostrar información de la celda
    private void HandleMouseHover()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0; // Asegurarse de que la Z sea 0 en 2D

        // Obtener la celda del personaje
        Vector2Int characterCell = gridManager.GetCellFromWorldPosition(transform.position);

        Vector2Int celda = gridManager.GetCellFromWorldPosition(mouseWorldPos);

        // Calcular la distancia en casillas
        int distance = gridManager.CalculateCellDistance(characterCell, celda);

        MostrarInfoPersonaje($"Distancia: {distance} casillas");

        DrawArrowDebug();

        DrawArrowGame();

        List<Vector2Int> listaCeldas = gridManager.EncontrarCamino(characterCell, celda);

        if (listaCeldas.Any())
        {
            foreach (Vector2Int cel in listaCeldas)
            {
                //gridManager.list

                //t.Highlight();
                //rend.material.color = highlightColor;
            }
        }
    }

    #region InformacionPersonaje

    // mostrar el texto de información del personaje
    private void MostrarInfoPersonaje(string info)
    {
        textoInfoPersonaje.text = info;
        textoInfoPersonaje.gameObject.SetActive(true);

        // Posicionar el texto
        //textoInfoPersonaje.transform.position = GetComponent<SpriteRenderer>().transform.position + new Vector3(20, -20, 0);
        //textoInfoPersonaje.transform.position = transform.position + new Vector3(0, 1, 0);
    }

    // ocultar el texto de información del personaje
    private void OcultarInfoPersonaje()
    {
        textoInfoPersonaje.gameObject.SetActive(false);
    }

    #endregion InformacionPersonaje

    #region ArcOfFire

    // Método para dibujar la zona delantera al personaje
    private void OnDrawGizmos()
    {
        Vector2Int currentCell = gridManager.GetCellFromWorldPosition(transform.position);

        DrawCellsInFrontArc(currentCell, direccionVisionPersonaje, alcanceVisionPersonaje);
    }

    // dibujar las celdas dentro del arco (delantero, trasero, derecho, izquierdo) del personaje en función de su dirección
    private void DrawCellsInFrontArc(Vector2Int currentCell, Vector2Int direccionVisionPersonaje, int alcanceVisionPersonaje)
    {
        List<Vector2Int> cellsInFrontArc = GetCellsInFrontArc(currentCell, direccionVisionPersonaje, alcanceVisionPersonaje);

        Gizmos.color = Color.green; // Color para resaltar las celdas

        foreach (Vector2Int cell in cellsInFrontArc)
        {
            Vector3 cellCenter = gridManager.GetCellCenterPosition(cell.x, cell.y);
            Gizmos.DrawWireCube(cellCenter, new Vector3(gridManager.tamañoCelda, gridManager.tamañoCelda, 0.1f));
        }
    }

    // obtener las celdas dentro del arco (delantero, trasero, derecho, izquierdo) del personaje en función de su dirección
    private List<Vector2Int> GetCellsInFrontArc(Vector2Int startCell, Vector2Int direction, int maxRows)
    {
        List<Vector2Int> cellsInView = new List<Vector2Int>();

        for (int row = 1; row <= maxRows; row++)
        {
            // Calcular el rango de celdas laterales para la fila actual
            int startOffset = -(row - 1);
            int endOffset = (row - 1);

            for (int offset = startOffset; offset <= endOffset; offset++)
            {
                // Calcular la posición de la celda en función de la dirección
                Vector2Int cell;
                if (direction == Vector2Int.up)
                {
                    cell = new Vector2Int(startCell.x + offset, startCell.y + row);
                }
                else if (direction == Vector2Int.down)
                {
                    cell = new Vector2Int(startCell.x + offset, startCell.y - row);
                }
                else if (direction == Vector2Int.left)
                {
                    cell = new Vector2Int(startCell.x - row, startCell.y + offset);
                }
                else if (direction == Vector2Int.right)
                {
                    cell = new Vector2Int(startCell.x + row, startCell.y + offset);
                }
                else
                {
                    // Dirección no válida
                    continue;
                }

                // Verificar si la celda está dentro del grid
                if (cell.x >= 0 && cell.x < gridManager.tamañoEscenarioX && cell.y >= 0 && cell.y < gridManager.tamañoEscenarioY)
                {
                    cellsInView.Add(cell);
                }
            }
        }

        return cellsInView;
    }

    #endregion ArcOfFire

    public Sprite arrowSprite; // Sprite de la flecha

    private Image arrowImage; // Referencia al componente Image de la flecha
    public RectTransform arrowRectTransform; // Referencia al RectTransform de la flecha

    // Método para dibujar una flecha entre dos puntos
    private void DrawArrowDebug()
    {
        // Obtener la posición del cursor en el mundo
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0; // Asegurarse de que la Z sea 0 en 2D

        // Obtener la celda bajo el cursor
        Vector2Int targetCell = gridManager.GetCellFromWorldPosition(mouseWorldPos);

        // Verificar si la celda está dentro del grid
        if (targetCell.x >= 0 && targetCell.x < gridManager.tamañoEscenarioX && targetCell.y >= 0 && targetCell.y < gridManager.tamañoEscenarioY)
        {
            // Obtener la celda del personaje
            Vector2Int characterCell = gridManager.GetCellFromWorldPosition(transform.position);

            // Obtener las posiciones centrales de las celdas
            Vector3 characterCenter = gridManager.GetCellCenterPosition(characterCell.x, characterCell.y);
            Vector3 targetCenter = gridManager.GetCellCenterPosition(targetCell.x, targetCell.y);

            // ------------

            // debug

            // Dibujar la línea principal de la flecha
            Debug.DrawLine(characterCenter, targetCenter, Color.red);

            // Calcular la dirección de la flecha
            Vector3 direction = (targetCenter - characterCenter).normalized;

            // Dibujar las dos líneas que forman la punta de la flecha
            float arrowHeadLength = 0.5f; // Longitud de la punta de la flecha
            Vector3 perpendicular = new Vector3(-direction.y, direction.x, 0) * 0.25f; // Perpendicular a la dirección
            Debug.DrawLine(targetCenter, targetCenter - direction * arrowHeadLength + perpendicular, Color.red);
            Debug.DrawLine(targetCenter, targetCenter - direction * arrowHeadLength - perpendicular, Color.red);
        }
    }

    // Crear un GameObject para la flecha
    private void CreateArrowGame()
    {
        // Crear un nuevo GameObject para la flecha
        GameObject arrowObject = new GameObject("Arrow");

        arrowObject.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>().transform);

        // Añadir el componente Image
        arrowImage = arrowObject.AddComponent<Image>();
        arrowImage.sprite = arrowSprite; // Asignar el sprite de la flecha
        arrowImage.color = Color.red; // Color de la flecha

        // Añadir el componente RectTransform
        arrowRectTransform = arrowObject.GetComponent<RectTransform>();
        //arrowRectTransform.sizeDelta = new Vector2(100, 20); // Tamaño inicial de la flecha

        arrowRectTransform.sizeDelta = new Vector2(gridManager.tamañoCelda * gridManager.tamañoEscenarioY, 20);

        // Asegurarse de que el pivote de la flecha esté en el extremo inicial (izquierdo)
        arrowRectTransform.pivot = new Vector2(0, 0.5f); // Pivote en el extremo izquierdo
    }

    // dibujar flecha desde cenro de celda en la que está el personaje a centro de celda sobre la que está el cursor del ratón
    private void DrawArrowGame()
    {
        // Obtener la posición del cursor en el mundo
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0; // Asegurarse de que la Z sea 0 en 2D

        // Obtener la celda bajo el cursor
        Vector2Int targetCell = gridManager.GetCellFromWorldPosition(mouseWorldPos);

        // Verificar si la celda está dentro del grid
        if (targetCell.x >= 0 && targetCell.x < gridManager.tamañoEscenarioX && targetCell.y >= 0 && targetCell.y < gridManager.tamañoEscenarioY)
        {
            // Obtener la celda del personaje
            Vector2Int characterCell = gridManager.GetCellFromWorldPosition(transform.position);

            // Obtener las posiciones centrales de las celdas
            Vector3 characterCenter = gridManager.GetCellCenterPosition(characterCell.x, characterCell.y);
            Vector3 targetCenter = gridManager.GetCellCenterPosition(targetCell.x, targetCell.y);

            // Convertir las posiciones del mundo a la pantalla
            Vector2 characterScreenPos = Camera.main.WorldToScreenPoint(characterCenter);
            Vector2 targetScreenPos = Camera.main.WorldToScreenPoint(targetCenter);

            // Posicionar la flecha en el centro de la celda del personaje
            arrowRectTransform.position = characterScreenPos;

            // Calcular la dirección y la distancia entre las celdas
            Vector2 direction = (targetScreenPos - characterScreenPos).normalized;
            float distance = Vector2.Distance(characterScreenPos, targetScreenPos);

            // Rotar la flecha para que apunte hacia la celda objetivo
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            arrowRectTransform.rotation = Quaternion.Euler(0, 0, angle);

            // Ajustar la longitud de la flecha según la distancia
            arrowRectTransform.sizeDelta = new Vector2(distance, arrowRectTransform.sizeDelta.y);

            //gridManager.GetCellsAlongArrow(characterCell, targetCell);
            //gridManager.GetCellsAlongArrow(characterCell, targetCell);
            //gridManager.GetCellsAlongArrow3(characterCell, targetCell);
            gridManager.GetCellsAlongArrow4(characterCell, targetCell);
        }
    }
}