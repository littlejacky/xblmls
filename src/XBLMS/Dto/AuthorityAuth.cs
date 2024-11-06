using System.Collections.Generic;
using XBLMS.Enums;

namespace XBLMS.Dto
{
    public class AuthorityAuth
    {
        public AuthorityType AuthType { get; set; }
        public int AdminId { get; set; }
        /// <summary>
        /// 如果是单位管理员，这里存储当前管理的单位id，如果是部门管理员，这里存储当前管理的部门id
        /// </summary>
        public int CurManageOrganId { get; set; }
        public List<int> CurManageOrganIds { get; set; }
        public int CompanyId { get; set; }
        public int DepartmentId { get; set; }
    }
}
