using UnityEngine;

public static class EnemyStatsPersistence
{
    public static void SaveEnemyStats(string enemyType)
    {
        if(enemyType == "Zombie")
        {
            PlayerPrefs.SetInt("ZombieHealth", EnemyStatsManager.ZombieStats.health);
            PlayerPrefs.SetFloat("ZombieSpeed", EnemyStatsManager.ZombieStats.speed);
            PlayerPrefs.SetInt("ZombieDamage", EnemyStatsManager.ZombieStats.damage);
            // PlayerPrefs.SetString("ZombieColor", EnemyStatsManager.ZombieStats.color);
        }
        else
        {
            PlayerPrefs.SetInt("FatHealth", EnemyStatsManager.FatStats.health);
            PlayerPrefs.SetFloat("FatSpeed", EnemyStatsManager.FatStats.speed);
            PlayerPrefs.SetInt("FatDamage", EnemyStatsManager.FatStats.damage);
            // PlayerPrefs.SetString("DogColor", EnemyStatsManager.DogStats.color);
        }

        PlayerPrefs.Save();
    }

    public static void LoadEnemyStats()
    {
        EnemyStatsManager.ZombieStats = new EnemyStats(
            PlayerPrefs.GetInt("ZombieHealth", 3),
            PlayerPrefs.GetFloat("ZombieSpeed", 1f),
            PlayerPrefs.GetInt("ZombieDamage", 10)
            // PlayerPrefs.GetString("ZombieColor", "FF0000")
        );

        EnemyStatsManager.FatStats = new EnemyStats(
            PlayerPrefs.GetInt("FatHealth", 3),
            PlayerPrefs.GetFloat("FatSpeed", 2f),
            PlayerPrefs.GetInt("FatDamage", 15)
            // PlayerPrefs.GetString("DogColor", "FF00D2")
        );
    }

    public static void SaveDefaultEnemyStats()
    {
        PlayerPrefs.SetInt("ZombieHealth", 3);
        PlayerPrefs.SetFloat("ZombieSpeed", 1f);
        PlayerPrefs.SetInt("ZombieDamage", 10);
        // PlayerPrefs.SetString("ZombieColor", "FF0000");

        PlayerPrefs.SetInt("FatHealth", 3);
        PlayerPrefs.SetFloat("FatSpeed", 2f);
        PlayerPrefs.SetInt("FatDamage", 15);
        // PlayerPrefs.SetString("DogColor", "FF00D2");

        PlayerPrefs.Save();
    }
}
