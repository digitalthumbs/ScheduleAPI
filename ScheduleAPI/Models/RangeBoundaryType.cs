using System.ComponentModel;

namespace ScheduleAPI.Models
{
    public enum RangeBoundaryType
    {
        [Description("Exclusive")]
        Exclusive,

        [Description("Inclusive on both boundaries")]
        Inclusive,

        [Description("Inclusive on only the lower boundary")]
        InclusiveLowerBoundaryOnly,

        [Description("Inclusive on only the upper boundary")]
        InclusiveUpperBoundaryOnly
    }
}
