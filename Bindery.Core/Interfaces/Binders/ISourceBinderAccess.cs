using Bindery.Implementations;

namespace Bindery.Interfaces.Binders
{
    internal interface ISourceBinderAccess<TSource>
    {
        SourceBinder<TSource> GetSourceBinder();
    }
}
