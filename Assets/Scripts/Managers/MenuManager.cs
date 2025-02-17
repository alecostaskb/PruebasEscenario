using System;
using TMPro;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [SerializeField] private GameObject _tileInfo;
    [SerializeField] private GameObject _tileUnitInfo;
    [SerializeField] private GameObject _selectedHeroInfo;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowTileInfo(Tile tile)
    {
        if (tile == null)
        {
            _tileInfo.SetActive(false);
            _tileUnitInfo.SetActive(false);

            return;
        }

        //_tileObject.GetComponentInChildren<TextMeshProUGUI>().text = $"{tile.TileName} name: {tile.name} x: {tile.x} , y: {tile.y}";
        _tileInfo.GetComponentInChildren<TextMeshProUGUI>().text = $"{tile.TileName}" + Environment.NewLine + $"x: {tile.x} , y: {tile.y}";

        _tileInfo.SetActive(true);

        if (tile.UnitInSquare)
        {
            _tileUnitInfo.GetComponentInChildren<TextMeshProUGUI>().text = tile.UnitInSquare.UnitName;
            _tileUnitInfo.SetActive(true);
        }
    }

    public void ShowSelectedHero(BaseHero hero)
    {
        if (hero == null)
        {
            _selectedHeroInfo.SetActive(false);

            return;
        }

        _selectedHeroInfo.GetComponentInChildren<TextMeshProUGUI>().text = hero.UnitName;
        _selectedHeroInfo.SetActive(true);
    }
}