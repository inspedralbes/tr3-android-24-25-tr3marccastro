using System.Collections.Generic;
using UnityEngine;

public class PoolBulletsManager : MonoBehaviour
{
    // Instancia est�tica del PoolBulletsManager para garantizar que solo haya una instancia en todo el juego
    private static PoolBulletsManager _instance;

    // Propiedad est�tica para acceder a la instancia del PoolBulletsManager desde cualquier lugar
    public static PoolBulletsManager Instance { get { return _instance; } }

    // Lista que almacena los objetos (balas) en el pool
    [SerializeField] private List<GameObject> pool = new List<GameObject>();

    // Prefab que se usar� para crear las balas en el pool
    [SerializeField] private GameObject prefabPool;

    // Cantidad m�xima de balas que puede haber en el pool
    public int maxBullets = 20;

    // M�todo que se ejecuta cuando el objeto se instancia en la escena
    private void Awake()
    {
        // Si ya existe una instancia y no es esta, destruye este objeto (asegurando una sola instancia)
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            // Si no existe una instancia, asigna esta instancia como la �nica v�lida
            _instance = this;
        }
    }

    // M�todo que se ejecuta al iniciar el juego
    private void Start()
    {
        // Crea el pool de balas con el tama�o m�ximo especificado
        CreatePool(maxBullets);
    }

    // M�todo para crear el pool de balas
    private void CreatePool(int maxBullets)
    {
        // Si el n�mero de balas es 0 o negativo, no se hace nada
        if (maxBullets <= 0) return;

        // Crea las balas y las a�ade al pool
        for (int i = 0; i < maxBullets; i++)
        {
            // Instancia una nueva bala usando el prefab
            GameObject go = Instantiate(prefabPool);
            go.transform.parent = transform; // Establece el objeto como hijo del PoolBulletsManager
            go.SetActive(false); // Desactiva la bala para que no est� activa al principio
            go.name = prefabPool.tag + "_" + i; // Asigna un nombre �nico a la bala
            pool.Add(go); // A�ade la bala al pool
        }
    }

    // M�todo para obtener una bala del pool
    public GameObject GetFromPool(Vector2 position, Quaternion rotation)
    {
        // Si el pool est� vac�o, retorna null
        if (pool.Count <= 0) return null;

        // Obtiene la primera bala en la lista
        GameObject go = pool[0];
        pool.RemoveAt(0); // La elimina de la lista del pool

        // Establece la posici�n y rotaci�n de la bala
        go.transform.position = position;
        go.transform.rotation = rotation;
        go.transform.parent = null; // La separa del PoolBulletsManager
        go.SetActive(true); // Activa la bala para que pueda usarse

        return go; // Devuelve la bala
    }

    // M�todo para devolver una bala al pool
    public void ReturnToPool(GameObject bullet)
    {
        // Desactiva la bala y la vuelve a colocar como hijo del PoolBulletsManager
        bullet.SetActive(false);
        bullet.transform.parent = transform;
        bullet.transform.position = Vector3.zero; // La coloca en la posici�n inicial
        bullet.transform.rotation = Quaternion.identity; // Resetea la rotaci�n
        pool.Add(bullet); // La a�ade nuevamente al pool
    }
}