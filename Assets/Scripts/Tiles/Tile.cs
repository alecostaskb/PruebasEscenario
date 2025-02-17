using System.Collections.Generic;
using UnityEngine;

public abstract class Tile : MonoBehaviour
{
    public string TileName;
    public int x;
    public int y;

    [SerializeField] protected SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight;
    [SerializeField] private bool _isWalkable = true;

    public BaseUnit UnitInSquare;
    public bool Walkable => _isWalkable && UnitInSquare == null;

    public virtual void Init(int x, int y)
    {
    }

    private void OnMouseEnter()
    {
        // resaltamos la casilla sobre la que esta el cursor
        SetHighlight(true);

        // mostramos informaci�n de la casilla
        MenuManager.Instance.ShowTileInfo(this);

        // si hay un h�roe seleccionado, mostramos el camino hasta la casilla
        if (UnitManager.Instance.selectedHero != null)
        {
            Vector2Int heroSquare = new Vector2Int(UnitManager.Instance.selectedHero.OccupiedTile.x, UnitManager.Instance.selectedHero.OccupiedTile.y);
            Vector2Int thisSquare = new Vector2Int(x, y);

            // obtenemos las celdas del camino
            List<Tile> listaCeldas = GridManagerTiles.Instance.FindPath(heroSquare, thisSquare);
        }
    }

    private void OnMouseExit()
    {
        // dejamos de resaltar la casilla
        SetHighlight(false);

        // ocultamos la informaci�n de la casilla
        MenuManager.Instance.ShowTileInfo(null);
    }

    private void OnMouseDown()
    {
        // tiene que ser el turno de los h�roes
        if (GameManager.Instance.gameStateNow != GameManager.GameState.HeroesTurn)
        {
            return;
        }

        // tiene que haber una unidad en la casilla
        if (UnitInSquare != null)
        {
            // la casilla est� ocupada

            if (UnitInSquare.Faction == Faction.Hero)
            {
                // la unidad que ocupa la casilla es un h�roe

                UnitManager.Instance.SetSelectedHero((BaseHero)UnitInSquare);
            }
            else
            {
                // la unidad que ocupa la casilla es un enemigo

                if (UnitManager.Instance.selectedHero != null)
                {
                    // tenemos un h�roe seleccionado

                    // cogemos el enemigo que ocupa la casilla
                    BaseEnemy enemy = (BaseEnemy)UnitInSquare;

                    // destruimos el enemigo
                    Destroy(enemy.gameObject);

                    // pero otras opciones "reales" ser�an, p.e.
                    // enemy.TakeDamage();
                    // UnitManager.Instance.SelectedHero.Attack(enemy);

                    // el h�roe deja de estar seleccionado
                    UnitManager.Instance.SetSelectedHero(null);
                }
            }
        }
        else
        {
            // la casilla est� libre

            if (UnitManager.Instance.selectedHero != null)
            {
                // tenemos un h�roe seleccionado

                // movemos el h�roe a la casilla
                SetUnit(UnitManager.Instance.selectedHero);

                // el h�roe deja de estar seleccionado
                UnitManager.Instance.SetSelectedHero(null);

                // quitar el resaltado de las casillas del camino actual
                GridManagerTiles.Instance.ClearPathHighlight();
            }
        }
    }

    public void SetUnit(BaseUnit unit)
    {
        if (unit.OccupiedTile != null)
        {
            unit.OccupiedTile.UnitInSquare = null;
        }

        unit.transform.position = transform.position;

        unit.transform.localScale = new Vector3(1, 1, 0);

        UnitInSquare = unit;
        unit.OccupiedTile = this;
    }

    public void SetHighlight(bool isActive)
    {
        _highlight.SetActive(isActive);
    }
}