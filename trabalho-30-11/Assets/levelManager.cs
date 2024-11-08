using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelManager : MonoBehaviour
{

    // Interface IPathable define os métodos que qualquer objeto com caminho precisa implementar
    public interface IPathable
    {
        Transform StartPoint { get; }   // Ponto de início do caminho
        Transform[] Path { get; }      // Caminho a ser seguido
        void MoveAlongPath();          // Método para mover ao longo do caminho
    }

    // Classe base LevelObject que implementa IPathable
    public abstract class LevelObject : MonoBehaviour, IPathable
    {
        [Header("Level Object Settings")]
        public Transform startPoint;  // Ponto de início
        public Transform[] path;     // Caminho a ser seguido

        // Propriedades para acessar o ponto de início e o caminho
        public Transform StartPoint => startPoint;
        public Transform[] Path => path;

        // Método para mover ao longo do caminho, a ser implementado pelas subclasses
        public abstract void MoveAlongPath();
    }

    // Classe de Inimigo que herda de LevelObject
    public class Enemy : LevelObject
    {
        [Header("Enemy Settings")]
        [SerializeField] private float moveSpeed = 2f;  // Velocidade de movimento do inimigo

        private int pathIndex = 0;

        // Implementação do movimento ao longo do caminho para o inimigo
        public override void MoveAlongPath()
        {
            if (path.Length == 0) return;

            // Movimenta o inimigo para o próximo ponto no caminho
            Vector3 direction = path[pathIndex].position - transform.position;
            transform.Translate(direction.normalized * moveSpeed * Time.deltaTime);

            // Verifica se o inimigo chegou ao destino e avança para o próximo ponto
            if (Vector3.Distance(transform.position, path[pathIndex].position) < 0.1f)
            {
                pathIndex++;
                if (pathIndex >= path.Length)
                {
                    Destroy(gameObject);  // Remove o inimigo quando ele chegar ao fim do caminho
                }
            }
        }
    }

    // Classe de Torre que também herda de LevelObject (pode ser usada no futuro para outras funcionalidades)
    public class Turret : LevelObject
    {
        [Header("Turret Settings")]
        [SerializeField] private float rotationSpeed = 5f;  // Velocidade de rotação da torre

        private void Update()
        {
            RotateTowardsTarget();
        }

        // Método para a torre girar em direção ao alvo (pode ser melhorado no futuro)
        public void RotateTowardsTarget()
        {
            if (path.Length == 0) return;

            // Alvo de exemplo (poderia ser um inimigo, por exemplo)
            Transform target = path[0];
            Vector3 direction = target.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Rotaciona a torre
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        }

        // Implementação do método MoveAlongPath (poderia ter outras funcionalidades no futuro)
        public override void MoveAlongPath()
        {
            // No caso da torre, o movimento não é necessário, mas esse método pode ser expandido conforme o design
        }
    }

    // Classe LevelManager que mantém uma lista de objetos do nível (como inimigos e torres)
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager main;

        [Header("Level References")]
        public Transform startPoint;  // Ponto inicial
        public Transform[] path;     // Caminho global

        [Header("Level Objects")]
        public List<LevelObject> levelObjects;  // Lista de objetos que implementam IPathable

        private void Awake()
        {
            if (main == null)
            {
                main = this;
            }
            else if (main != this)
            {
                Destroy(gameObject);
            }

            // Adiciona todos os objetos do nível à lista (a lista pode ser preenchida no editor)
            foreach (LevelObject levelObject in levelObjects)
            {
                levelObject.startPoint = startPoint;  // Atribui o ponto inicial
                levelObject.path = path;              // Atribui o caminho
            }
        }

        private void Update()
        {
            // Atualiza o movimento de todos os objetos do nível
            foreach (LevelObject levelObject in levelObjects)
            {
                levelObject.MoveAlongPath();  // Chama o método de movimento de cada objeto
            }
        }
    }

}
