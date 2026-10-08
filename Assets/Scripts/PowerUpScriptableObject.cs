using UnityEngine;
[CreateAssetMenu(fileName = "PowerUp", menuName = "PowerUp/PowerUp Data", order = 1)]
public class PowerUpScriptableObject : ScriptableObject
{
    [SerializeField] private string powerUpType;
    [SerializeField] private float powerUpValue;
    [SerializeField] private float timeLimit;

    public string PowerUpType { get => powerUpType; set => powerUpType = value; }
    public float PowerUpValue { get => powerUpValue; set => powerUpValue = value; }
    public float TimeLimit { get => timeLimit; set => timeLimit = value; }
}
