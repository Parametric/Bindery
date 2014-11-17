using System.ComponentModel;
using Bindery.Interfaces.Basic;

namespace Bindery.Interfaces.Observables
{
    public interface ISubscriptionComplete<TSource> 
    {
        IBasicSourceBinder<TSource> Subscribe();
    }
}