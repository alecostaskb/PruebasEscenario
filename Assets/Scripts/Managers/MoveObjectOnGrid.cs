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
    private Vector2 tama�oImagenEscenario;

    private Vector3 targetPosition;
    public float tama�oCelda = 1.0f; // Tama�o de la celda
    public float moveSpeed = 5.0f; // Velocidad de movimiento

    public Vector2Int direccionVisionPersonaje; // Por defecto, el personaje mira hacia arriba

    //public float anguloVisionPersonaje = 90f; // �ngulo de visi�n en grados
    public int alcanceVisionPersonaje = 3; // Rango de la zona delantera

    public TextMeshProUGUI textoInfoPersonaje; // Referencia al objeto de texto en la UI para mostrar informaci�n del personaje

    private void Start()
    {
        if (gridManager == null)
        {
            Debug.Log("gridManager no existe");

            return;
        }

        // Ocultar el texto de info del personaje
        textoInfoPersonaje.gameObject.SetActive(false);

        // Obtener la posici�n de la imagen
        posicionImagenEscenario = gridManager.posicionImagenEscenario;
        tama�oImagenEscenario = gridManager.tama�oImagenEscenario;

        tama�oCelda = gridManager.tama�oCelda;
        targetPosition = transform.position;

        // direcci�n en la que est� mirando el personaje (se usa para calcular su arco)
        direccionVisionPersonaje = Vector2Int.up;

        // Crear el objeto Image en el Canvas
        CreateArrowGame();
    }

    private void Update()
    {
        HandleMouseClick();

        HandleMouseHover();
    }

    // Manejar el clic del rat�n para mover el personaje
    private void HandleMouseClick()
    {
        if (Input.GetMouseButtonDown(0)) // Al hacer clic
        {
            // Obtener la posici�n del clic en el mundo 2D
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0; // Asegurarse de que la Z sea 0 en 2D

            // Celdas, actual y destino
            //Vector2Int currentCell = gridManager.GetCellFromWorldPosition(transform.position);
            Vector2Int targetCell = gridManager.GetCellFromWorldPosition(mouseWorldPos);

            // Verificar si la celda de destino es v�lida
            //if (x >= 0 && x < gridManager.tama�oEscenarioX && y >= 0 && y < gridManager.tama�oEscenarioY)
            if (targetCell.x >= 0 && targetCell.x < gridManager.tama�oEscenarioX
                && targetCell.y >= 0 && targetCell.y < gridManager.tama�oEscenarioY)
            {
                // Obtener el costo de la celda (opcional)
                int cellCost = gridManager.GetCellCost(targetCell.x, targetCell.y);
                Debug.Log($"X: {targetCell.x}, Y: {targetCell.y}, costo: " + cellCost);

                // Verificar si la celda es transitable
                //if (gridManager.IsCellWalkable(targetCell.x, targetCell.y) && !gridManager.IsCellInClosedArea(targetCell))
                if (gridManager.IsCellWalkable(targetCell.x, targetCell.y) && !gridManager.IsCellClosed(targetCell.x, targetCell.y))
                {
                    // Calcular la posici�n central de la celda de destino
                    targetPosition = gridManager.GetCellCenterPosition(targetCell.x, targetCell.y);

                    // Mover suavemente el personaje a la posici�n objetivo
                    Debug.Log($"Objeto se mover� a la celda: {targetCell.x}, {targetCell.y}");
                    //transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
                    transform.position = targetPosition;
                }
                else
                {
                    Debug.Log("Celda bloqueada");
                    MostrarInfoPersonaje("No puedo mover all�:" + Environment.NewLine
                        + "Celda bloqueada");
                }
            }
            else
            {
                Debug.Log("Celda fuera del grid");
                MostrarInfoPersonaje("No puedo mover all�:" + Environment.NewLine
                    + "Celda fuera del grid");
            }

            // Ocultar el texto despu�s de 2 segundos
            Invoke(nameof(OcultarInfoPersonaje), 2f);
        }
    }

    // Manejar el hover del rat�n para mostrar informaci�n de la celda
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

    // mostrar el texto de informaci�n del personaje
    private void MostrarInfoPersonaje(string info)
    {
        textoInfoPersonaje.text = info;
        textoInfoPersonaje.gameObject.SetActive(true);

        // Posicionar el texto
        //textoInfoPersonaje.transform.position = GetComponent<SpriteRenderer>().transform.position + new Vector3(20, -20, 0);
        //textoInfoPersonaje.transform.position = transform.position + new Vector3(0, 1, 0);
    }

    // ocultar el texto de informaci�n del personaje
    private void OcultarInfoPersonaje()
    {
        textoInfoPersonaje.gameObject.SetActive(false);
    }

    #endregion InformacionPersonaje

    #region ArcOfFire

    // M�todo para dibujar la zona delantera al personaje
    private void OnDrawGizmos()
    {
        Vector2Int currentCell = gridManager.GetCellFromWorldPosition(transform.position);

        DrawCellsInFrontArc(currentCell, direccionVisionPersonaje, alcanceVisionPersonaje);
    }

    // dibujar las celdas dentro del arco (delantero, trasero, derecho, izquierdo) del personaje en funci�n de su direcci�n
    private void DrawCellsInFrontArc(Vector2Int currentCell, Vector2Int direccionVisionPersonaje, int alcanceVisionPersonaje)
    {
        List<Vector2Int> cellsInFrontArc = GetCellsInFrontArc(currentCell, direccionVisionPersonaje, alcanceVisionPersonaje);

        Gizmos.color = Color.green; // Color para resaltar las celdas

        foreach (Vector2Int cell in cellsInFrontArc)
        {
            Vector3 cellCenter = gridManager.GetCellCenterPosition(cell.x, cell.y);
            Gizmos.DrawWireCube(cellCenter, new Vector3(gridManager.tama�oCelda, gridManager.tama�oCelda, 0.1f));
        }
    }

    // obtener las celdas dentro del arco (delantero, trasero, derecho, izquierdo) del personaje en funci�n de su direcci�n
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
                // Calcular la posici�n de la celda en funci�n de la direcci�n
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
                    // Direcci�n no v�lida
                    continue;
                }

                // Verificar si la celda est� dentro del grid
                if (cell.x >= 0 && cell.x < gridManager.tama�oEscenarioX && cell.y >= 0 && cell.y < gridManager.tama�oEscenarioY)
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

    // M�todo para dibujar una flecha entre dos puntos
    private void DrawArrowDebug()
    {
        // Obtener la posici�n del cursor en el mundo
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0; // Asegurarse de que la Z sea 0 en 2D

        // Obtener la celda bajo el cursor
        Vector2Int targetCell = gridManager.GetCellFromWorldPosition(mouseWorldPos);

        // Verificar si la celda est� dentro del grid
        if (targetCell.x >= 0 && targetCell.x < gridManager.tama�oEscenarioX && targetCell.y >= 0 && targetCell.y < gridManager.tama�oEscenarioY)
        {
            // Obtener la celda del personaje
            Vector2Int characterCell = gridManager.GetCellFromWorldPosition(transform.position);

            // Obtener las posiciones centrales de las celdas
            Vector3 characterCenter = gridManager.GetCellCenterPosition(characterCell.x, characterCell.y);
            Vector3 targetCenter = gridManager.GetCellCenterPosition(targetCell.x, targetCell.y);

            // ------------

            // debug

            // Dibujar la l�nea principal de la flecha
            Debug.DrawLine(characterCenter, targetCenter, Color.red);

            // Calcular la direcci�n de la flecha
            Vector3 direction = (targetCenter - characterCenter).normalized;

            // Dibujar las dos l�neas que forman la punta de la flecha
            float arrowHeadLength = 0.5f; // Longitud de la punta de la flecha
            Vector3 perpendicular = new Vector3(-direction.y, direction.x, 0) * 0.25f; // Perpendicular a la direcci�n
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

        // A�adir el componente Image
        arrowImage = arrowObject.AddComponent<Image>();
        arrowImage.sprite = arrowSprite; // Asignar el sprite de la flecha
        arrowImage.color = Color.red; // Color de la flecha

        // A�adir el componente RectTransform
        arrowRectTransform = arrowObject.GetComponent<RectTransform>();
        //arrowRectTransform.sizeDelta = new Vector2(100, 20); // Tama�o inicial de la flecha

        arrowRectTransform.sizeDelta = new Vector2(gridManager.tama�oCelda * gridManager.tama�oEscenarioY, 20);

        // Asegurarse de que el pivote de la flecha est� en el extremo inicial (izquierdo)
        arrowRectTransform.pivot = new Vector2(0, 0.5f); // Pivote en el extremo izquierdo
    }

    // dibujar flecha desde cenro de celda en la que est� el personaje a centro de celda sobre la que est� el cursor del rat�n
    private void DrawArrowGame()
    {
        // Obtener la posici�n del cursor en el mundo
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0; // Asegurarse de que la Z sea 0 en 2D

        // Obtener la celda bajo el cursor
        Vector2Int targetCell = gridManager.GetCellFromWorldPosition(mouseWorldPos);

        // Verificar si la celda est� dentro del grid
        if (targetCell.x >= 0 && targetCell.x < gridManager.tama�oEscenarioX && targetCell.y >= 0 && targetCell.y < gridManager.tama�oEscenarioY)
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

            // Calcular la direcci�n y la distancia entre las celdas
            Vector2 direction = (targetScreenPos - characterScreenPos).normalized;
            float distance = Vector2.Distance(characterScreenPos, targetScreenPos);

            // Rotar la flecha para que apunte hacia la celda objetivo
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            arrowRectTransform.rotation = Quaternion.Euler(0, 0, angle);

            // Ajustar la longitud de la flecha seg�n la distancia
            arrowRectTransform.sizeDelta = new Vector2(distance, arrowRectTransform.sizeDelta.y);

            //gridManager.GetCellsAlongArrow(characterCell, targetCell);
            //gridManager.GetCellsAlongArrow(characterCell, targetCell);
            //gridManager.GetCellsAlongArrow3(characterCell, targetCell);
            gridManager.GetCellsAlongArrow4(characterCell, targetCell);
        }
    }
}