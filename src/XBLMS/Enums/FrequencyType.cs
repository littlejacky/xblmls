using Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace XBLMS.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FrequencyType
    {
        [DataEnum(DisplayName = "工作日", Value = "PerWeekday")] PerWeekday,
        [DataEnum(DisplayName = "每天", Value = "Daily")] Daily,
        [DataEnum(DisplayName = "每周", Value = "Weekly")] Weekly,
        [DataEnum(DisplayName = "每月", Value = "Monthly")] Monthly,
        [DataEnum(DisplayName = "每年", Value = "Yearly")] Yearly,
    }
}
