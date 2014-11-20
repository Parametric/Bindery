namespace Bindery.Interfaces.Observables
{
    public interface ISubscriptionComplete<TSource> 
    {
        ISourceBinder<TSource> Subscribe();
    }
}