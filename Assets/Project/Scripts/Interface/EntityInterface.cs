public interface IBattle
{
    void OnAttack(); // 공격 => TODO: GameObject 인자로 받아서 대상 확인하고 OnDamage 호출할 수 있도록.
    void OnDamage(float damage); // 피격
}