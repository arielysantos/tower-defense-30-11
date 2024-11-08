using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameObject : MonoBehaviour
{
    public interface IGameObject
    {
        void Update(); // Atualiza o estado do objeto
        void Interact(); // Interage com outros objetos (exemplo: atacar inimigos)
    }
    public class GameObject : IGameObject
    {
        public string Name { get; set; }
        public int Health { get; set; }

        public virtual void Update()
        {
            // Lógica para atualizar o estado básico do objeto
            Console.WriteLine($"{Name} is being updated.");
        }

        public virtual void Interact()
        {
            // Lógica para interagir com outros objetos
            Console.WriteLine($"{Name} is interacting.");
        }
    }
    public class Tower : GameObject
    {
        public int Damage { get; set; }

        public Tower(string name, int health, int damage)
        {
            Name = name;
            Health = health;
            Damage = damage;
        }

        public override void Update()
        {
            base.Update();
            Console.WriteLine($"{Name} Tower is attacking!");
        }

        public override void Interact()
        {
            // Especificar interação das torres (por exemplo, atacar inimigos próximos)
            Console.WriteLine($"{Name} Tower is attacking nearby enemies for {Damage} damage!");
        }
    }
    public class Enemy : GameObject
    {
        public int Speed { get; set; }
        public int Damage { get; set; }

        public Enemy(string name, int health, int speed, int damage)
        {
            Name = name;
            Health = health;
            Speed = speed;
            Damage = damage;
        }

        public override void Update()
        {
            base.Update();
            Console.WriteLine($"{Name} is moving towards the tower!");
        }

        public override void Interact()
        {
            // Especificar como o inimigo interage com a torre (dano ao ser atingido por uma torre, etc.)
            Console.WriteLine($"{Name} enemy is taking {Damage} damage!");
        }
    }
    public class Game
    {
        private List<IGameObject> gameObjects;

        public Game()
        {
            gameObjects = new List<IGameObject>();
        }

        public void AddObject(IGameObject gameObject)
        {
            gameObjects.Add(gameObject);
        }

        public void UpdateGame()
        {
            foreach (var gameObject in gameObjects)
            {
                gameObject.Update(); // Atualiza todos os objetos no jogo
                gameObject.Interact(); // Faz interação entre os objetos
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            // Criando o jogo
            Game game = new Game();

            // Adicionando torres
            Tower tower1 = new Tower("Cannon Tower", 100, 25);
            game.AddObject(tower1);

            // Adicionando inimigos
            Enemy enemy1 = new Enemy("Goblin", 50, 5, 10);
            game.AddObject(enemy1);

            Enemy enemy2 = new Enemy("Orc", 100, 3, 15);
            game.AddObject(enemy2);

            // Atualizando o estado do jogo
            game.UpdateGame();
        }
    }

}
