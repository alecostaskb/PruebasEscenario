// acción, y los puntos de acción que otorga al personaje
using System.Collections.Generic;
using UnityEngine;

public enum Action
{
    Static = 0,
    Walking = 4,
    Running = 8
}

public class BaseHero : BaseUnit
{
    public int ActionPoints;
    public int RemainingActionPoints;
    public Action CurrentAction;

    public Vector2Int direccionVisionPersonaje; // Por defecto, el personaje mira hacia arriba
    public int alcanceVisionPersonaje = 3; // Rango de la zona delantera
    //public float anguloVisionPersonaje = 90f; // Ángulo de visión en grados

    // Start is called before the first frame update
    private void Start()
    {
        Faction = Faction.Hero;

        // dirección en la que está mirando el personaje (se usa para calcular su arco)
        direccionVisionPersonaje = Vector2Int.up;
    }

    // Update is called once per frame
    private void Update()
    {
        //DrawCellsInFrontArc(new Vector2Int(this.OccupiedTile.x, this.OccupiedTile.y), direccionVisionPersonaje, alcanceVisionPersonaje);
    }

    #region ArcOfFire

    // Método para dibujar la zona delantera al personaje
    private void OnDrawGizmos()
    {
        Vector2Int characterCell = new Vector2Int(OccupiedTile.x, OccupiedTile.y);

        DrawCellsInFrontArc(characterCell, direccionVisionPersonaje, alcanceVisionPersonaje);
    }

    // dibujar las celdas dentro del arco (delantero, trasero, derecho, izquierdo) del personaje en función de su dirección
    private void DrawCellsInFrontArc(Vector2Int currentCell, Vector2Int direccionVisionPersonaje, int alcanceVisionPersonaje)
    {
        List<Vector2Int> cellsInFrontArc = GetCellsInFrontArc(currentCell, direccionVisionPersonaje, alcanceVisionPersonaje);

        Gizmos.color = Color.green; // Color para resaltar las celdas

        // tamaño de la celda (las celdas son cuadradas)
        float tamañoCelda = GridManagerTiles.Instance.GetCellSize();

        foreach (Vector2Int cell in cellsInFrontArc)
        {
            Vector3 cellCenter = GridManagerTiles.Instance.GetCellCenterPosition(cell.x, cell.y);
            Gizmos.DrawWireCube(cellCenter, new Vector3(tamañoCelda, tamañoCelda, 0.1f));
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
                if (cell.x >= 0 && cell.x < GridManagerTiles.Instance.scenarioWidth && cell.y >= 0 && cell.y < GridManagerTiles.Instance.scenarioHeight)
                {
                    cellsInView.Add(cell);
                }
            }
        }

        return cellsInView;
    }

    #endregion ArcOfFire
}