using UnityEngine;
using Mirror;

public class EnemyBase : NetworkBehaviour
{
    [SerializeField] private Vector2 spawnAreaMin = new Vector2(-10, -10); // Mínimo de la zona de spawn
    [SerializeField] private Vector2 spawnAreaMax = new Vector2(10, 10);   // Máximo de la zona de spawn
    [SerializeField] private int health = 3; // Vidas del zombie
    [SerializeField] private float speed = 2f; // Velocidad del zombie
    private Transform target; // Objetivo (jugador más cercano)

    // Si el servidor controla el movimiento del zombie
    void Update()
    {
        if (!isServer) return;

        // Encontrar el jugador más cercano
        FindClosestPlayer();

        if (target != null)
        {
            // Mover el zombie hacia el jugador más cercano
            Vector2 newPosition = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            RpcMoveMonster(newPosition); // Enviar la nueva posición a los clientes
        }
    }

    // Buscar el jugador más cercano
    [Server]
    void FindClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float minDistance = Mathf.Infinity;
        Transform closestPlayer = null;

        foreach (GameObject player in players)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestPlayer = player.transform;
            }
        }

        target = closestPlayer;
    }

    // Recibir daño, manejado en el servidor
    [Server]
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            // Cuando la salud llegue a 0, el zombie debe regresar al pool
            RcpReturnToPool();
        }
    }

    // Volver al pool y respawnear para todos los jugadores
    [ClientRpc]
    void RcpReturnToPool()
    {
        // Devolver el enemigo al pool (solo el servidor lo maneja)
        PoolEnemies.Instance.ReturnToPool(gameObject);

        // Re-spawnear el zombie para todos los jugadores
        SpawnZombie();
    }

    // Re-spawnear un nuevo zombie en una posición aleatoria
    [Server]
    void SpawnZombie()
    {
        // Generamos una posición aleatoria dentro de los límites del spawn
        Vector2 spawnPosition = new Vector2(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            Random.Range(spawnAreaMin.y, spawnAreaMax.y)
        );

        // Obtener el zombie del pool
        GameObject zombie = PoolEnemies.Instance.GetFromPool(spawnPosition);

        // Si el zombie es válido, hacer que se vea para todos los jugadores
        if (zombie != null)
        {
            NetworkServer.Spawn(zombie); // Respawnear el zombie para todos los jugadores
        }
    }

    // Colisión con otro objeto (recibe daño al colisionar)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isServer) return;

        // Cuando el zombie colisiona, recibe 1 de daño
        TakeDamage(1);
    }

    // Sincronizar el movimiento del zombie entre todos los clientes
    [ClientRpc]
    void RpcMoveMonster(Vector2 newPosition)
    {
        transform.position = newPosition;
    }
}
