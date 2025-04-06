using UnityEngine;

public static class EnemyStatsPrefs
{
    // Método para guardar las estadísticas de un enemigo específico (Zombie o FatZombie)
    public static void SaveEnemyStats(string enemyType)
    {
        // Si el tipo de enemigo es "Zombie", guarda sus estadísticas en PlayerPrefs
        if (enemyType == "Zombie")
        {
            PlayerPrefs.SetInt("ZombieHealth", EnemyStatsManager.ZombieStats.health);
            PlayerPrefs.SetFloat("ZombieSpeed", EnemyStatsManager.ZombieStats.speed);
            PlayerPrefs.SetInt("ZombieDamage", EnemyStatsManager.ZombieStats.damage);
            PlayerPrefs.SetString("ZombieColor", EnemyStatsManager.ZombieStats.color);
        }
        else // Si el enemigo es "FatZombie", guarda sus estadísticas en PlayerPrefs
        {
            PlayerPrefs.SetInt("FatHealth", EnemyStatsManager.FatStats.health);
            PlayerPrefs.SetFloat("FatSpeed", EnemyStatsManager.FatStats.speed);
            PlayerPrefs.SetInt("FatDamage", EnemyStatsManager.FatStats.damage);
            PlayerPrefs.SetString("FatColor", EnemyStatsManager.FatStats.color);
        }

        // Guarda los cambios realizados en PlayerPrefs
        PlayerPrefs.Save();
    }

    // Método para cargar las estadísticas de los enemigos desde PlayerPrefs
    public static void LoadEnemyStats()
    {
        // Carga las estadísticas del "Zombie" desde PlayerPrefs, y si no existen, asigna valores predeterminados
        EnemyStatsManager.ZombieStats = new EnemyStats(
            PlayerPrefs.GetInt("ZombieHealth", 3),   // Valor predeterminado de salud: 3
            PlayerPrefs.GetFloat("ZombieSpeed", 1f), // Valor predeterminado de velocidad: 1
            PlayerPrefs.GetInt("ZombieDamage", 10),  // Valor predeterminado de daño: 10
            PlayerPrefs.GetString("ZombieColor", "FFFFFF") // Valor predeterminado de color: blanco
        );

        // Carga las estadísticas del "FatZombie" desde PlayerPrefs, y si no existen, asigna valores predeterminados
        EnemyStatsManager.FatStats = new EnemyStats(
            PlayerPrefs.GetInt("FatHealth", 5),   // Valor predeterminado de salud: 5
            PlayerPrefs.GetFloat("FatSpeed", 0.5f), // Valor predeterminado de velocidad: 0.5
            PlayerPrefs.GetInt("FatDamage", 20),  // Valor predeterminado de daño: 20
            PlayerPrefs.GetString("FatColor", "FFFFFF") // Valor predeterminado de color: blanco
        );
    }

    // Método para guardar las estadísticas predeterminadas de los enemigos en PlayerPrefs
    public static void SaveDefaultEnemyStats()
    {
        // Guarda las estadísticas predeterminadas del "Zombie"
        PlayerPrefs.SetInt("ZombieHealth", 3);
        PlayerPrefs.SetFloat("ZombieSpeed", 1f);
        PlayerPrefs.SetInt("ZombieDamage", 10);
        PlayerPrefs.SetString("ZombieColor", "FFFFFF");

        // Guarda las estadísticas predeterminadas del "FatZombie"
        PlayerPrefs.SetInt("FatHealth", 5);
        PlayerPrefs.SetFloat("FatSpeed", 0.5f);
        PlayerPrefs.SetInt("FatDamage", 20);
        PlayerPrefs.SetString("FatColor", "FFFFFF");

        // Guarda todos los cambios realizados en PlayerPrefs
        PlayerPrefs.Save();
    }
}
