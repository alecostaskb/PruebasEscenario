// acci�n, y los puntos de acci�n que otorga al personaje
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
    //public float anguloVisionPersonaje = 90f; // �ngulo de visi�n en grados

    // Start is called before the first frame update
    private void Start()
    {
        Faction = Faction.Hero;

        // direcci�n en la que est� mirando el personaje (se usa para calcular su arco)
        direccionVisionPersonaje = Vector2Int.up;
    }

    // Update is called once per frame
    private void Update()
    {
        //DrawCellsInFrontArc(new Vector2Int(this.OccupiedTile.x, this.OccupiedTile.y), direccionVisionPersonaje, alcanceVisionPersonaje);
    }

    #region ArcOfFire

    // M�todo para dibujar la zona delantera al personaje
    private void OnDrawGizmos()
    {
        Vector2Int characterCell = new Vector2Int(OccupiedTile.x, OccupiedTile.y);

        DrawCellsInFrontArc(characterCell, direccionVisionPersonaje, alcanceVisionPersonaje);
    }

    // dibujar las celdas dentro del arco (delantero, trasero, derecho, izquierdo) del personaje en funci�n de su direcci�n
    private void DrawCellsInFrontArc(Vector2Int currentCell, Vector2Int direccionVisionPersonaje, int alcanceVisionPersonaje)
    {
        List<Vector2Int> cellsInFrontArc = GetCellsInFrontArc(currentCell, direccionVisionPersonaje, alcanceVisionPersonaje);

        Gizmos.color = Color.green; // Color para resaltar las celdas

        // tama�o de la celda (las celdas son cuadradas)
        float tama�oCelda = GridManagerTiles.Instance.GetCellSize();

        foreach (Vector2Int cell in cellsInFrontArc)
        {
            Vector3 cellCenter = GridManagerTiles.Instance.GetCellCenterPosition(cell.x, cell.y);
            Gizmos.DrawWireCube(cellCenter, new Vector3(tama�oCelda, tama�oCelda, 0.1f));
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