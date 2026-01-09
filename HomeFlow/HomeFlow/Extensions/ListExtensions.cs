


using HomeFlow.Interfaces;

namespace HomeFlow.Extensions;

public static class ListExtensions
{
    public static void Reorder<T>( this IList<T> items, int oldIndex, int newIndex )
    {
        if ( items == null )
            throw new ArgumentNullException( nameof( items ) );
        if ( oldIndex < 0 || oldIndex >= items.Count )
            throw new ArgumentOutOfRangeException( nameof( oldIndex ) );
        if ( newIndex < 0 || newIndex >= items.Count )
            throw new ArgumentOutOfRangeException( nameof( newIndex ) );
        if ( oldIndex == newIndex )
            return;

        T item = items[oldIndex];

        items.RemoveAt( oldIndex );

        items.Insert( newIndex, item );

        for ( int i = 0; i < items.Count; i++ )
        {
            if ( items[i] is IOrderable orderable )
            {
                orderable.Order = i;
            }
        }
    }
}
