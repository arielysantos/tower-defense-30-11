using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class enemyspawn : MonoBehaviour
{
 
// Interface que define o comportamento b�sico de qualquer entidade spawn�vel
public interface ISpawnable
{
    void OnSpawn();   // M�todo chamado quando a entidade � gerada
    void OnDestroy(); // M�todo chamado quando a entidade � destru�da
}

// Classe base de inimigo, implementa ISpawnable para ser um tipo de entidade spawn�vel
public abstract class EnemyBase : MonoBehaviour, ISpawnable
{
    [Header("Enemy Attributes")]
    public int health = 100;     // Vida do inimigo
    public float speed = 2f;     // Velocidade do inimigo

    // M�todo chamado quando o inimigo � gerado
    public virtual void OnSpawn()
    {
        Debug.Log($"{gameObject.name} foi gerado.");
    }

    // M�todo chamado quando o inimigo � destru�do
    public virtual void OnDestroy()
    {
        Debug.Log($"{gameObject.name} foi destru�do.");
        EnemySpawn.onEnemyDestroy.Invoke(); // Notifica o EnemySpawn que o inimigo foi destru�do
    }

    // M�todo para aplicar dano ao inimigo
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject); // Destr�i o inimigo se a vida chegar a zero
        }
    }
}

// Subclasse de inimigo espec�fica - Inimigo R�pido
public class FastEnemy : EnemyBase
{
    public FastEnemy()
    {
        speed = 4f; // Velocidade aumentada para inimigos r�pidos
    }

    public override void OnSpawn()
    {
        base.OnSpawn();
        Debug.Log("Inimigo r�pido foi gerado.");
    }
}

// Subclasse de inimigo espec�fica - Inimigo Tanque
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
    [SerializeField] private int baseEnemies = 8;                // N�mero base de inimigos por onda
    [SerializeField] private float enemiesPerSecond = 0.5f;      // Velocidade de gera��o dos inimigos
    [SerializeField] private float timeBetweenWaves = 5f;        // Tempo entre as ondas
    [SerializeField] private float difficultyScalingFactor = 0.75f; // Fator de escala de dificuldade

    [Header("Events")]
    public static UnityEvent onEnemyDestroy = new UnityEvent(); // Evento que � chamado quando um inimigo � destru�do

    private int currentWave = 1;         // N�mero da onda atual
    private float timeSinceLastSpawn;    // Tempo desde o �ltimo spawn
    private int enemiesAlive;            // Contador de inimigos vivos
    private int enemiesLeftToSpawn;      // N�mero de inimigos restantes para gerar
    private bool isSpawning = false;     // Indica se a onda est� em progresso

    private List<EnemyBase> spawnedEnemies = new List<EnemyBase>(); // Lista dos inimigos gerados

    private void Awake()
    {
        onEnemyDestroy.AddListener(EnemyDestroyed); // Adiciona o m�todo para ser chamado quando um inimigo � destru�do
    }

    private void Update()
    {
        // Se n�o estamos em processo de gera��o, nada a fazer
        if (!isSpawning) return;

        // Verifica se � hora de gerar o pr�ximo inimigo
        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn >= (1f / enemiesPerSecond) && enemiesLeftToSpawn > 0)
        {
            SpawnEnemy();
            enemiesLeftToSpawn--;
            enemiesAlive++;
            timeSinceLastSpawn = 0f;
        }

        // Se n�o h� inimigos vivos e a onda est� completa, inicia a pr�xima onda
        if (enemiesAlive <= 0 && enemiesLeftToSpawn <= 0)
        {
            Invoke(nameof(StartNextWave), timeBetweenWaves); // Inicia a pr�xima onda ap�s um intervalo
            isSpawning = false;
        }
    }

    // M�todo chamado quando um inimigo � destru�do
    private void EnemyDestroyed()
    {
        enemiesAlive--;
    }

    // Inicia a pr�xima onda
    private void StartNextWave()
    {
        currentWave++;
        isSpawning = true;
        enemiesLeftToSpawn = EnemiesPerWave(); // Define o n�mero de inimigos para a pr�xima onda
    }

    // M�todo para gerar um inimigo aleat�rio da lista de prefabs
    private void SpawnEnemy()
    {
        EnemyBase enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        EnemyBase spawnedEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        spawnedEnemy.OnSpawn(); // Chama o m�todo OnSpawn do inimigo gerado
        spawnedEnemies.Add(spawnedEnemy); // Adiciona � lista de inimigos gerados
    }

    // Calcula o n�mero de inimigos para a onda com base no fator de dificuldade
    private int EnemiesPerWave()
    {
        return Mathf.RoundToInt(baseEnemies * Mathf.Pow(currentWave, difficultyScalingFactor));
    }
}

}
