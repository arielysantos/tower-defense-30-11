using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class enemyMoviments : MonoBehaviour

{
    // Interface que define o comportamento básico de movimento
    public interface IMovable
    {
        void MoveTowardsTarget(); // Movimenta o objeto em direção ao alvo
        void UpdateTarget();      // Atualiza o próximo ponto de destino
    }

    // Classe base para todos os inimigos, implementa a interface IMovable
    public abstract class EnemyBase : MonoBehaviour, IMovable
    {
        [Header("References")]
        [SerializeField] protected Rigidbody2D rb; // Referência ao Rigidbody2D para manipulação física

        [Header("Attributes")]
        [SerializeField] protected float moveSpeed = 2f; // Velocidade padrão de movimento

        protected Transform target; // Próximo ponto no caminho
        protected int pathIndex = 0; // Índice do ponto atual no caminho

        // Inicializa o primeiro destino
        protected virtual void Start()
        {
            target = LevelManager.main.path[pathIndex];
        }

        // Método para movimentar o inimigo em direção ao alvo
        public virtual void MoveTowardsTarget()
        {
            if (target == null) return;

            Vector2 direction = (target.position - transform.position).normalized;
            rb.MovePosition((Vector2)transform.position + direction * moveSpeed * Time.deltaTime);

            if (Vector2.Distance(target.position, transform.position) <= 0.1f)
            {
                UpdateTarget();
            }
        }

        // Atualiza o próximo ponto de destino
        public virtual void UpdateTarget()
        {
            pathIndex++;
            if (pathIndex < LevelManager.main.path.Length)
            {
                target = LevelManager.main.path[pathIndex];
            }
            else
            {
                // Quando o inimigo chega ao fim do caminho, remove-o
                EnemySpawner.onEnemyDestroy.Invoke();
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            MoveTowardsTarget();
        }
    }

    // Subclasse de inimigo rápido com velocidade personalizada
    public class FastEnemy : EnemyBase
    {
        protected override void Start()
        {
            base.Start();
            moveSpeed = 3.5f; // Velocidade aumentada para o inimigo rápido
        }
    }

    // Subclasse de inimigo tanque com velocidade reduzida e mais vida
    public class TankEnemy : EnemyBase
    {
        [SerializeField] private int health = 200; // Vida extra para o inimigo tanque

        protected override void Start()
        {
            base.Start();
            moveSpeed = 1.5f; // Velocidade reduzida para o inimigo tanque
        }

        // Método adicional para reduzir a vida ao receber dano
        public void TakeDamage(int damage)
        {
            health -= damage;
            if (health <= 0)
            {
                EnemySpawner.onEnemyDestroy.Invoke();
                Destroy(gameObject);
            }
        }
    }

    // Classe responsável pelo gerenciamento de inimigos
    public class EnemySpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private EnemyBase[] enemyPrefabs; // Prefabs de inimigos disponíveis para spawn

        [Header("Attributes")]
        [SerializeField] private int baseEnemies = 8;              // Número de inimigos na onda inicial
        [SerializeField] private float spawnRate = 0.5f;           // Frequência de spawn em inimigos por segundo

        public static UnityEvent onEnemyDestroy = new UnityEvent(); // Evento chamado quando um inimigo é destruído

        private List<EnemyBase> activeEnemies = new List<EnemyBase>(); // Lista de inimigos ativos

        private void Update()
        {
            // Atualiza o movimento de todos os inimigos ativos
            foreach (EnemyBase enemy in activeEnemies)
            {
                enemy.MoveTowardsTarget();
            }
        }

        // Método para spawnar um inimigo aleatório e adicioná-lo à lista de inimigos ativos
        private void SpawnEnemy()
        {
            EnemyBase enemyToSpawn = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            EnemyBase spawnedEnemy = Instantiate(enemyToSpawn, transform.position, Quaternion.identity);
            spawnedEnemy.OnSpawn();
            activeEnemies.Add(spawnedEnemy);
        }

        // Método chamado quando um inimigo é destruído
        public void OnEnemyDestroyed(EnemyBase enemy)
        {
            activeEnemies.Remove(enemy);
        }
    }


