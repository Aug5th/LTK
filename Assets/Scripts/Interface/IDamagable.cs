public interface IDamagable
{
    void TakeDamage(int amount);
    bool IsDead { get; }
    UnityEngine.Transform Transform { get; }
}