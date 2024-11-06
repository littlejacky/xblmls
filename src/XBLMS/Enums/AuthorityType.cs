using Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace XBLMS.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AuthorityType
    {
        [DataEnum(DisplayName = "超级管理员",Value = "Admin")] Admin,
        [DataEnum(DisplayName = "单位管理员", Value = "AdminCompany")] AdminCompany,
        [DataEnum(DisplayName = "部门管理员", Value = "AdminDepartment")] AdminDepartment,
        [DataEnum(DisplayName = "普通管理员", Value = "AdminSelf")] AdminSelf,
    }
}
