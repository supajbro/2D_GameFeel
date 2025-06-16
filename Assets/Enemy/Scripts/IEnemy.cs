public interface IEnemy
{
    public EnemyState State { get; set; }
    public bool CanMove { get; set; }
    public float MoveSpeed { get; set; }

    public void MoveUpdate();
    public void AttackUpdate();
}

[System.Serializable]
public enum EnemyState
{
    Idle = 0,
    Attack
}
