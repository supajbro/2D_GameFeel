public interface IHealth
{
    public float CurrentHealth {  get; set; }
    public float MaxHealth { get; set; }

    public void ChangeHealth(float health);
    public void Die();
}
