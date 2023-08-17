public interface IHitable<T>
{
    void DamageTaken(T damageValue);
    void DestroySelf();
    T MyHealth { get; }
}