using Mirror;
using UnityEngine;

public class PlayerMultiplayerTest : NetworkBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;

    public GameObject bulletPrefab;        // Prefab de la bala
    public Transform shootingPoint;        // Punto desde donde se disparará la bala
    public float fireRate = 0.5f;          // Frecuencia de disparo
    private float nextFireTime = 0f;       // Control de la frecuencia de disparo

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!isLocalPlayer) return;  // Asegúrate de que solo el jugador local puede moverse

        // Movimiento
        MovePlayer();

        // Rotación hacia el ratón
        RotateTowardsMouse();

        // Disparo
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            CmdShoot();  // Llamar al método para disparar en el servidor
        }
    }

    private void MovePlayer()
    {
        // Movimiento del jugador
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(moveX, moveY) * speed;
        rb.linearVelocity = movement;  // Utiliza la propiedad velocity para el movimiento en Rigidbody2D
    }

    private void RotateTowardsMouse()
    {
        // Obtener la posición del ratón en el mundo
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;  // Mantener en el plano 2D

        // Calcular la dirección hacia el ratón
        Vector3 direction = mousePosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Rotar el jugador hacia el ratón
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    // Método que se ejecuta en el servidor para crear la bala
    [Command]
    void CmdShoot()
    {
        RpcShoot();  // Llamar al método RPC para todos los clientes
    }

    // Método RPC que se ejecuta en todos los clientes
    [ClientRpc]
    void RpcShoot()
    {
        // Crear la bala en todos los clientes
        GameObject bullet = Instantiate(bulletPrefab, shootingPoint.position, shootingPoint.rotation);
        NetworkServer.Spawn(bullet);  // Sincronizar la bala entre todos los clientes
    }
}
