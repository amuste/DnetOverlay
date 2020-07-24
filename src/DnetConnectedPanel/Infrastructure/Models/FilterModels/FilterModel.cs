using System.ComponentModel.DataAnnotations;

namespace DnetConnectedPanel.Infrastructure.Models.FilterModels
{
    public class FilterModel
    {
        [Required]
        public string Column { get; set; }

        [Required]
        public string Value { get; set; }

        [Required]
        public string AdditionalValue { get; set; }

        [Required] public FilterType Type { get; set; } = FilterType.Text;

        public FilterOperator Operator { get; set; } = FilterOperator.Contains;

        public FilterOperator AdditionalOperator { get; set; } = FilterOperator.None;

        public FilterCondition Condition { get; set; } = FilterCondition.None;

    }
}
