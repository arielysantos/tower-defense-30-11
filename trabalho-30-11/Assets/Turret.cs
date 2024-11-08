using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Turret : MonoBehaviour
{

// Interface que define o comportamento b�sico de uma torre
public interface ITurret
{
    void FindTarget();
    void RotateTowardsTarget();
}

// Classe base de torre com funcionalidades comuns
public abstract class TurretBase : MonoBehaviour, ITurret
{
    [Header("References")]
    [SerializeField] protected Transform turretRotationPoint;  // Ponto de rota��o da torre

    [Header("Attributes")]
    [SerializeField] protected float targetingRange = 5f;     // Alcance de mira
    [SerializeField] protected LayerMask enemyMask;           // Camada de inimigos

    protected Transform target;                               // Alvo atual

    private void Update()
    {
        if (target == null)
        {
            FindTarget(); // Procura um alvo se n�o houver um
        }
        else
        {
            RotateTowardsTarget(); // Rotaciona a torre para o alvo atual
        }
    }

    // M�todo para encontrar o alvo dentro do alcance da torre
    public virtual void FindTarget()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, targetingRange, Vector2.zero, 0f, enemyMask);

        if (hits.Length > 0)
        {
            target = hits[0].transform; // Define o primeiro inimigo encontrado como alvo
        }
    }

    // M�todo para rotacionar a torre na dire��o do alvo
    public virtual void RotateTowardsTarget()
    {
        if (target == null) return;

        Vector3 direction = target.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        turretRotationPoint.rotation = Quaternion.Euler(0, 0, angle);
    }

    // M�todo para desenhar o alcance de mira no editor
    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, Vector3.forward, targetingRange);
    }
}

// Subclasse de torre de fogo
public class FireTurret : TurretBase
{
    // Implementa��o espec�fica da torre de fogo
    public override void RotateTowardsTarget()
    {
        base.RotateTowardsTarget();
        Debug.Log("Rotacionando torre de fogo em dire��o ao alvo");
    }
}

// Subclasse de torre de gelo
public class IceTurret : TurretBase
{
    // Implementa��o espec�fica da torre de gelo
    public override void RotateTowardsTarget()
    {
        base.RotateTowardsTarget();
        Debug.Log("Rotacionando torre de gelo em dire��o ao alvo e aplicando efeito de lentid�o");
    }
}

// GameManager que gerencia uma lista de torres no jogo
public class GameManager : MonoBehaviour
{
    private List<TurretBase> turrets = new List<TurretBase>(); // Lista de todas as torres

    // Adiciona uma torre � lista de torres do jogo
    public void AddTurret(TurretBase turret)
    {
        turrets.Add(turret);
    }

    private void Update()
    {
        // Atualiza todas as torres na lista
        foreach (TurretBase turret in turrets)
        {
            turret.FindTarget(); // Atualiza alvo
            turret.RotateTowardsTarget(); // Rotaciona em dire��o ao alvo
        }
    }
}

}
