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
            PlayerPrefs.SetString("ZombieColor", EnemyStatsManager.ZombieStats.color);
        }
        else
        {
            PlayerPrefs.SetInt("DogHealth", EnemyStatsManager.DogStats.health);
            PlayerPrefs.SetFloat("DogSpeed", EnemyStatsManager.DogStats.speed);
            PlayerPrefs.SetInt("DogDamage", EnemyStatsManager.DogStats.damage);
            PlayerPrefs.SetString("DogColor", EnemyStatsManager.DogStats.color);
        }

        PlayerPrefs.Save();
    }

    public static void LoadEnemyStats()
    {
        EnemyStatsManager.ZombieStats = new EnemyStats(
            PlayerPrefs.GetInt("ZombieHealth", 3),
            PlayerPrefs.GetFloat("ZombieSpeed", 1f),
            PlayerPrefs.GetInt("ZombieDamage", 10),
            PlayerPrefs.GetString("ZombieColor", "FF0000")
        );

        EnemyStatsManager.DogStats = new EnemyStats(
            PlayerPrefs.GetInt("DogHealth", 3),
            PlayerPrefs.GetFloat("DogSpeed", 2f),
            PlayerPrefs.GetInt("DogDamage", 15),
            PlayerPrefs.GetString("DogColor", "FF00D2")
        );
    }

    public static void SaveDefaultEnemyStats()
    {
        PlayerPrefs.SetInt("ZombieHealth", 3);
        PlayerPrefs.SetFloat("ZombieSpeed", 1f);
        PlayerPrefs.SetInt("ZombieDamage", 10);
        PlayerPrefs.SetString("ZombieColor", "FF0000");

        PlayerPrefs.SetInt("DogHealth", 3);
        PlayerPrefs.SetFloat("DogSpeed", 2f);
        PlayerPrefs.SetInt("DogDamage", 15);
        PlayerPrefs.SetString("DogColor", "FF00D2");

        PlayerPrefs.Save();
    }
}
