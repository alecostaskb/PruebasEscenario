using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GameManager;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;

    private List<ScriptableUnit> units;
    public BaseHero selectedHero;

    private void Awake()
    {
        Instance = this;

        units = Resources.LoadAll<ScriptableUnit>("Units").ToList();
    }

    public void SpawnHeroes()
    {
        int heroCount = 2;

        for (int i = 0; i < heroCount; i++)
        {
            BaseHero randomHero = GetRandomUnit<BaseHero>(Faction.Hero);
            BaseHero spawnedHero = Instantiate(randomHero);
            Tile randomSpawnTile = GridManagerTiles.Instance.GetHeroSpawnTile();

            randomSpawnTile.SetUnit(spawnedHero);
        }

        GameManager.Instance.ChangeState(GameState.SpawnEnemies);
    }

    public void SpawnEnemies()
    {
        int enemyCount = 1;

        for (int i = 0; i < enemyCount; i++)
        {
            BaseEnemy randomEnemy = GetRandomUnit<BaseEnemy>(Faction.Enemy);
            BaseEnemy spawnedEnemy = Instantiate(randomEnemy);
            Tile randomSpawnTile = GridManagerTiles.Instance.GetEnemySpawnTile();

            randomSpawnTile.SetUnit(spawnedEnemy);
        }

        GameManager.Instance.ChangeState(GameState.HeroesTurn);
    }

    private T GetRandomUnit<T>(Faction faction) where T : BaseUnit
    {
        return (T)units.Where(unit => unit.Faction == faction).OrderBy(o => Random.value).First().UnitPrefab;
    }

    public void SetSelectedHero(BaseHero hero)
    {
        selectedHero = hero;

        MenuManager.Instance.ShowSelectedHero(hero);
    }
}