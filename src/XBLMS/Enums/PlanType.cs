using Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace XBLMS.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PlanType
    {
        [DataEnum(DisplayName = "考试", Value = "Paper")] Paper,
        [DataEnum(DisplayName = "练习", Value = "Practice")] Practice,
    }
}
