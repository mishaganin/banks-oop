namespace Banks.Entities.Observers.Notify;

public interface INotifyObservable
{
    public void AddObserver(INotifyObserver o);
    public void RemoveObserver(INotifyObserver o);
    public void NotifyNotificationObservers(Notification notification);
}