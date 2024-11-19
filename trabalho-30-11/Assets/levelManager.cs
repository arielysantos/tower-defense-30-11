using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static gameManager;

public class LevelManager : MonoBehaviour
{

    public static LevelManager main;

    public Transform startPoint;
    public Transform[] pathPoints;
    public GameObject fastEnemyPrefab;
    public GameObject strongEnemyPrefab;

    private List<Enemy> enemies = new List<Enemy>();
    internal readonly object path;

    private void Awake()
    {
        if (main == null)
        {
            main = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Configura o caminho
        List<Transform> path = new List<Transform>(pathPoints);

        // Instancia um inimigo rápido e um inimigo forte como exemplo
        SpawnEnemy(fastEnemyPrefab, path);
        SpawnEnemy(strongEnemyPrefab, path);
    }

    private void SpawnEnemy(GameObject enemyPrefab, List<Transform> path)
    {
        GameObject enemyObject = Instantiate(enemyPrefab, startPoint.position, Quaternion.identity);
        Enemy enemy = enemyObject.GetComponent<Enemy>();
        enemy.SetPath(path);
        enemies.Add(enemy);
    }

    private void Update()
    {
        // Faz todos os inimigos se moverem
        foreach (Enemy enemy in enemies)
        {
            enemy.Move();
        }
    }

}

   

