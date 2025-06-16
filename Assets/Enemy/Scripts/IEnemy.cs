public interface IEnemy
{
    public EnemyState State { get; set; }
    public bool CanMove { get; set; }
    public float MoveSpeed { get; set; }
    public float Damage { get; set; }

    public void MoveUpdate();
    public void AttackUpdate(Player player);
}

[System.Serializable]
public enum EnemyState
{
    Idle = 0,
    Attack
}
