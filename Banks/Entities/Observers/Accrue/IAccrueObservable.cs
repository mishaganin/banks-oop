namespace Banks.Entities.Observers.Accrue;

public interface IAccrueObservable
{
    public void AddObserver(IAccrueObserver o);
    public void RemoveObserver(IAccrueObserver o);
    public void NotifyAccrueObservers(int days);
}