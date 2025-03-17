using UnityEngine;
using Mirror;

public class GameManager : NetworkManager
{
    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("Servidor iniciado.");
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log("Cliente conectado.");
    }
}
