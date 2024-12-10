using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "ScriptableObjects/UnitStats", order = 1)]
public class UnitStats : ScriptableObject
{
    public int creationTime = 5; // Время создания персонажа
    public float movementSpeed = 5f; // Скорость передвижения персонажа
    public float jumpForce = 8f; // Сила прыжка
    public int maxHealth = 3; // Максимальное здоровье
}
