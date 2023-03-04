namespace Banks.Entities.Observers.Accrue;

public interface IAccrueObserver
{
    void Update(int days);
}