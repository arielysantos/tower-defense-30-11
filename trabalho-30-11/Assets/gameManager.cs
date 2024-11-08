using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{

    // Interface para objetos atualizáveis
    public interface IUpdatable
    {
        // Método de atualização que será implementado pelas classes que herdarem essa interface
        void Update();
    }

    // Classe base para torres
    public abstract class Tower : IUpdatable
    {
        public string Name { get; set; }        // Nome da torre
        public int AttackPower { get; set; }    // Poder de ataque da torre
        public float Range { get; set; }        // Alcance de ataque da torre

        // Construtor da classe Tower, inicializa o nome, poder de ataque e alcance
        public Tower(string name, int attackPower, float range)
        {
            Name = name;
            AttackPower = attackPower;
            Range = range;
        }

        // Método abstrato de ataque que será implementado por subclasses
        public abstract void Attack(Enemy enemy);

        // Método de atualização que procura inimigos para atacar
        public virtual void Update()
        {
            Console.WriteLine($"{Name} está em busca de inimigos no alcance.");
        }
    }

    // Torre de Fogo: subclasse da Tower
    public class FireTower : Tower
    {
        // Construtor da FireTower com valores específicos de nome, ataque e alcance
        public FireTower() : base("Fire Tower", 30, 5.0f) { }

        // Implementação do método Attack para causar dano de fogo
        public override void Attack(Enemy enemy)
        {
            Console.WriteLine($"{Name} ataca {enemy.Name} com fogo causando {AttackPower} de dano.");
        }
    }

    // Torre de Gelo: subclasse da Tower
    public class IceTower : Tower
    {
        // Construtor da IceTower com valores específicos de nome, ataque e alcance
        public IceTower() : base("Ice Tower", 20, 4.0f) { }

        // Implementação do método Attack para causar dano de gelo e reduzir velocidade
        public override void Attack(Enemy enemy)
        {
            Console.WriteLine($"{Name} ataca {enemy.Name} com gelo causando {AttackPower} de dano e diminuindo sua velocidade.");
        }
    }

    // Classe base para inimigos
    public class Enemy : IUpdatable
    {
        public string Name { get; set; }   // Nome do inimigo
        public int Health { get; set; }    // Saúde do inimigo
        public float Speed { get; set; }   // Velocidade de movimento do inimigo

        // Construtor da classe Enemy que inicializa nome, saúde e velocidade
        public Enemy(string name, int health, float speed)
        {
            Name = name;
            Health = health;
            Speed = speed;
        }

        // Método de atualização que representa o movimento do inimigo
        public virtual void Update()
        {
            Console.WriteLine($"{Name} está avançando com {Speed} de velocidade.");
        }

        // Método para aplicar dano ao inimigo
        public void TakeDamage(int damage)
        {
            Health -= damage;
            Console.WriteLine($"{Name} recebeu {damage} de dano. Saúde restante: {Health}");
            if (Health <= 0)
            {
                Console.WriteLine($"{Name} foi derrotado!");
            }
        }
    }

    // GameManager que gerencia o estado do jogo
    public class GameManager
    {
        private List<Tower> towers;    // Lista de torres no jogo
        private List<Enemy> enemies;   // Lista de inimigos no jogo

        // Construtor do GameManager que inicializa listas de torres e inimigos
        public GameManager()
        {
            towers = new List<Tower>();
            enemies = new List<Enemy>();
        }

        // Adiciona uma torre à lista de torres do jogo
        public void AddTower(Tower tower)
        {
            towers.Add(tower);
        }

        // Adiciona um inimigo à lista de inimigos do jogo
        public void AddEnemy(Enemy enemy)
        {
            enemies.Add(enemy);
        }

        // Atualiza o estado do jogo, fazendo as torres atacarem os inimigos
        public void UpdateGame()
        {
            Console.WriteLine("Atualizando o estado do jogo...");

            // Atualiza e faz as torres atacarem inimigos
            foreach (var tower in towers)
            {
                tower.Update(); // Atualiza cada torre

                foreach (var enemy in enemies)
                {
                    // Verifica se o inimigo está vivo e faz a torre atacá-lo
                    if (enemy.Health > 0)
                    {
                        tower.Attack(enemy);
                        enemy.TakeDamage(tower.AttackPower);
                    }
                }
            }

            // Atualiza a posição e ações de cada inimigo
            foreach (var enemy in enemies)
            {
                if (enemy.Health > 0)
                {
                    enemy.Update();
                }
            }

            // Remove inimigos derrotados da lista
            enemies.RemoveAll(e => e.Health <= 0);
        }
    }

    // Exemplo de uso da estrutura do jogo
    public class Program
    {
        public static void Main()
        {
            // Cria o GameManager para gerenciar o estado do jogo
            GameManager gameManager = new GameManager();

            // Adiciona diferentes torres ao jogo
            gameManager.AddTower(new FireTower());
            gameManager.AddTower(new IceTower());

            // Adiciona inimigos ao jogo
            gameManager.AddEnemy(new Enemy("Orc", 100, 1.5f));
            gameManager.AddEnemy(new Enemy("Goblin", 50, 2.0f));

            // Atualiza o estado do jogo (faz torres atacarem e inimigos se moverem)
            gameManager.UpdateGame();
        }
    }
}


