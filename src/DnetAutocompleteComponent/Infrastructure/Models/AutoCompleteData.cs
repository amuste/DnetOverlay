using System.Collections.Generic;

namespace DnetAutocompleteComponent.Infrastructure.Models
{
    public class AutoCompleteData<TItem>
    {
        public int AutoCompleteDataId { get; set; }

        public TItem Item { get; set; }

    }
}
