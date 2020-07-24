using System;
using System.Collections.Generic;

namespace DnetAutocompleteComponent.Infrastructure.Interfaces
{
    public interface IAutoCompleteListService<TItem>
    {
        event Action<List<TItem>> OnUpdateList;

        void UdateList(List<TItem> items);
    }
}
