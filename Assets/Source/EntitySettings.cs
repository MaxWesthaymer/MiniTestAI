using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Entity Settings", menuName = "New Entity Settings")]
public class EntitySettings : ScriptableObject
{
    public EntityConfig playerConfig;
    public EntityConfig[] enemyConfigs;
}

[Serializable] 
public class EntityConfig
{
    public EntityConfig(int health, int speed, int attackPower, int defence)
    {
        this.health = health;
        this.speed = speed;
        this.attackPower = attackPower;
        this.defence = defence;
    }

    public int health;
    public int speed;
    public int attackPower;
    public int defence;
}
