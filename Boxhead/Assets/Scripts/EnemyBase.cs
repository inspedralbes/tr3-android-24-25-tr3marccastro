using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] private int _health = 3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetDamage(int damage) {
        _health -= damage;
        if(_health <= 0) {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        GetDamage(1);
    }
}
