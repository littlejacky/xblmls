using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Enums;
using XBLMS.Models;

namespace XBLMS.Core.Services
{
    public partial class StudyManager
    {
        public async Task PlanArrangeUser(StudyPlan plan, AuthorityAuth auth)
        {
            var userIds = new List<int>();

            if (plan.UserGroupIds != null && plan.UserGroupIds.Count > 0)
            {
                var allUser = false;
                foreach (int groupId in plan.UserGroupIds)
                {
                    var group = await _userGroupRepository.GetAsync(groupId);
                    if (group.GroupType == UsersGroupType.All)
                    {
                        allUser = true;
                        var letUserIds = await _userRepository.GetUserIdsWithOutLockedAsync(auth);
                        if (letUserIds != null && letUserIds.Count > 0)
                        {
                            userIds.AddRange(letUserIds);
                        }
                        break;
                    }
                }

                if (!allUser)
                {
                    foreach (int groupId in plan.UserGroupIds)
                    {
                        var group = await _userGroupRepository.GetAsync(groupId);
                        if (group.GroupType == UsersGroupType.Fixed)
                        {
                            if (group.UserIds != null && group.UserIds.Count > 0)
                            {
                                userIds.AddRange(group.UserIds);
                            }
                        }
                        if (group.GroupType == Enums.UsersGroupType.Range)
                        {
                            var letUserIds = await _userRepository.Group_IdsWithoutLockedAsync(auth, group);
                            if (letUserIds != null && letUserIds.Count > 0)
                            {
                                userIds.AddRange(letUserIds);
                            }
                        }
                    }

                }
            }

            if (userIds != null && userIds.Count > 0)
            {
                userIds = userIds.Distinct().ToList();
                foreach (int userId in userIds)
                {
                    var exist = await _studyPlanUserRepository.ExistsAsync(plan.Id, userId);
                    if (!exist)
                    {
                        var user = await _userRepository.GetByUserIdAsync(userId);

                        await _studyPlanUserRepository.InsertAsync(new StudyPlanUser
                        {
                            State = StudyStatType.Weikaishi,
                            PlanId = plan.Id,
                            PlanYear = plan.PlanYear,
                            Credit = plan.PlanCredit,
                            UserId = user.Id,
                            KeyWordsAdmin = await _organManager.GetUserKeyWords(userId),
                            KeyWords = plan.PlanName,
                            PlanBeginDateTime = plan.PlanBeginDateTime,
                            PlanEndDateTime = plan.PlanEndDateTime,
                            Locked = plan.Locked,
                            CompanyId = plan.CompanyId,
                            DepartmentId = plan.DepartmentId,
                            CreatorId = plan.CreatorId
                        });
                    }
                }
            }
        }
    }
}
