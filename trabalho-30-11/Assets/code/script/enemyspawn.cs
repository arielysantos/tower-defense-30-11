using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class enemyspawn : MonoBehaviour
{
 
// Interface que define o comportamento básico de qualquer entidade spawnável
public interface ISpawnable
{
    void OnSpawn();   // Método chamado quando a entidade é gerada
    void OnDestroy(); // Método chamado quando a entidade é destruída
}

// Classe base de inimigo, implementa ISpawnable para ser um tipo de entidade spawnável
public abstract class EnemyBase : MonoBehaviour, ISpawnable
{
    [Header("Enemy Attributes")]
    public int health = 100;     // Vida do inimigo
    public float speed = 2f;     // Velocidade do inimigo

    // Método chamado quando o inimigo é gerado
    public virtual void OnSpawn()
    {
        Debug.Log($"{gameObject.name} foi gerado.");
    }

    // Método chamado quando o inimigo é destruído
    public virtual void OnDestroy()
    {
        Debug.Log($"{gameObject.name} foi destruído.");
        EnemySpawn.onEnemyDestroy.Invoke(); // Notifica o EnemySpawn que o inimigo foi destruído
    }

    // Método para aplicar dano ao inimigo
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject); // Destrói o inimigo se a vida chegar a zero
        }
    }
}

// Subclasse de inimigo específica - Inimigo Rápido
public class FastEnemy : EnemyBase
{
    public FastEnemy()
    {
        speed = 4f; // Velocidade aumentada para inimigos rápidos
    }

    public override void OnSpawn()
    {
        base.OnSpawn();
        Debug.Log("Inimigo rápido foi gerado.");
    }
}

// Subclasse de inimigo específica - Inimigo Tanque
public class TankEnemy : EnemyBase
{
    public TankEnemy()
    {
        health = 200; // Aumenta a vida para um inimigo tanque
        speed = 1f;   // Diminui a velocidade
    }

    public override void OnSpawn()
    {
        base.OnSpawn();
        Debug.Log("Inimigo tanque foi gerado.");
    }
}

// Classe EnemySpawn para gerenciar o spawn de inimigos
public class EnemySpawn : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyBase[] enemyPrefabs; // Prefabs dos inimigos a serem gerados

    [Header("Attributes")]
    [SerializeField] private int baseEnemies = 8;                // Número base de inimigos por onda
    [SerializeField] private float enemiesPerSecond = 0.5f;      // Velocidade de geração dos inimigos
    [SerializeField] private float timeBetweenWaves = 5f;        // Tempo entre as ondas
    [SerializeField] private float difficultyScalingFactor = 0.75f; // Fator de escala de dificuldade

    [Header("Events")]
    public static UnityEvent onEnemyDestroy = new UnityEvent(); // Evento que é chamado quando um inimigo é destruído

    private int currentWave = 1;         // Número da onda atual
    private float timeSinceLastSpawn;    // Tempo desde o último spawn
    private int enemiesAlive;            // Contador de inimigos vivos
    private int enemiesLeftToSpawn;      // Número de inimigos restantes para gerar
    private bool isSpawning = false;     // Indica se a onda está em progresso

    private List<EnemyBase> spawnedEnemies = new List<EnemyBase>(); // Lista dos inimigos gerados

    private void Awake()
    {
        onEnemyDestroy.AddListener(EnemyDestroyed); // Adiciona o método para ser chamado quando um inimigo é destruído
    }

    private void Update()
    {
        // Se não estamos em processo de geração, nada a fazer
        if (!isSpawning) return;

        // Verifica se é hora de gerar o próximo inimigo
        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn >= (1f / enemiesPerSecond) && enemiesLeftToSpawn > 0)
        {
            SpawnEnemy();
            enemiesLeftToSpawn--;
            enemiesAlive++;
            timeSinceLastSpawn = 0f;
        }

        // Se não há inimigos vivos e a onda está completa, inicia a próxima onda
        if (enemiesAlive <= 0 && enemiesLeftToSpawn <= 0)
        {
            Invoke(nameof(StartNextWave), timeBetweenWaves); // Inicia a próxima onda após um intervalo
            isSpawning = false;
        }
    }

    // Método chamado quando um inimigo é destruído
    private void EnemyDestroyed()
    {
        enemiesAlive--;
    }

    // Inicia a próxima onda
    private void StartNextWave()
    {
        currentWave++;
        isSpawning = true;
        enemiesLeftToSpawn = EnemiesPerWave(); // Define o número de inimigos para a próxima onda
    }

    // Método para gerar um inimigo aleatório da lista de prefabs
    private void SpawnEnemy()
    {
        EnemyBase enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        EnemyBase spawnedEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        spawnedEnemy.OnSpawn(); // Chama o método OnSpawn do inimigo gerado
        spawnedEnemies.Add(spawnedEnemy); // Adiciona à lista de inimigos gerados
    }

    // Calcula o número de inimigos para a onda com base no fator de dificuldade
    private int EnemiesPerWave()
    {
        return Mathf.RoundToInt(baseEnemies * Mathf.Pow(currentWave, difficultyScalingFactor));
    }
}

}
