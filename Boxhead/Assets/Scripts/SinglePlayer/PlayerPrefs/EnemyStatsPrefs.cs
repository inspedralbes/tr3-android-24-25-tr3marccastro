using UnityEngine;

public static class EnemyStatsPrefs
{
    public static void SaveEnemyStats(string enemyType)
    {
        if (enemyType == "Zombie")
        {
            PlayerPrefs.SetInt("ZombieHealth", EnemyStatsManager.ZombieStats.health);
            PlayerPrefs.SetFloat("ZombieSpeed", EnemyStatsManager.ZombieStats.speed);
            PlayerPrefs.SetInt("ZombieDamage", EnemyStatsManager.ZombieStats.damage);
            PlayerPrefs.SetString("ZombieColor", EnemyStatsManager.ZombieStats.color);
        }
        else
        {
            PlayerPrefs.SetInt("FatHealth", EnemyStatsManager.FatStats.health);
            PlayerPrefs.SetFloat("FatSpeed", EnemyStatsManager.FatStats.speed);
            PlayerPrefs.SetInt("FatDamage", EnemyStatsManager.FatStats.damage);
            PlayerPrefs.SetString("FatColor", EnemyStatsManager.FatStats.color);
        }

        PlayerPrefs.Save();
    }

    public static void LoadEnemyStats()
    {
        EnemyStatsManager.ZombieStats = new EnemyStats(
            PlayerPrefs.GetInt("ZombieHealth", 3),
            PlayerPrefs.GetFloat("ZombieSpeed", 1f),
            PlayerPrefs.GetInt("ZombieDamage", 10),
            PlayerPrefs.GetString("ZombieColor", "FFFFFF")
        );

        EnemyStatsManager.FatStats = new EnemyStats(
            PlayerPrefs.GetInt("FatHealth", 5),
            PlayerPrefs.GetFloat("FatSpeed", 0.5f),
            PlayerPrefs.GetInt("FatDamage", 20),
            PlayerPrefs.GetString("FatColor", "FFFFFF")
        );
    }

    public static void SaveDefaultEnemyStats()
    {
        PlayerPrefs.SetInt("ZombieHealth", 3);
        PlayerPrefs.SetFloat("ZombieSpeed", 1f);
        PlayerPrefs.SetInt("ZombieDamage", 10);
        PlayerPrefs.SetString("ZombieColor", "FFFFFF");

        PlayerPrefs.SetInt("FatHealth", 5);
        PlayerPrefs.SetFloat("FatSpeed", 0.5f);
        PlayerPrefs.SetInt("FatDamage", 20);
        PlayerPrefs.SetString("FatColor", "FFFFFF");

        PlayerPrefs.Save();
    }
}