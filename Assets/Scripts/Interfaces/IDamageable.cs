using UnityEngine;

public interface IDamageable
{
    public float health { get; set; }
    public void Damage(float amount);
    public void Heal(float amount);
}
