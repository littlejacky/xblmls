using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Enums;
using XBLMS.Models;
using XBLMS.Utils;

namespace XBLMS.Core.Services
{
    public partial class ExamManager
    {
        public async Task ClearQuestionnaire(int examPaperId)
        {
            await _examQuestionnaireAnswerRepository.ClearByPaperAsync(examPaperId);
            await _examQuestionnaireTmRepository.DeleteByPaperAsync(examPaperId);
            await _examQuestionnaireUserRepository.ClearByPaperAsync(examPaperId);
        }

        public async Task SetQuestionnairTm(List<ExamQuestionnaireTm> tmList, int paperId)
        {
            if (tmList != null && tmList.Count > 0)
            {
                foreach (var tm in tmList)
                {
                    tm.ExamPaperId = paperId;
                    await _examQuestionnaireTmRepository.InsertAsync(tm);
                }
            }
        }

        public async Task ArrangeQuestionnaire(AuthorityAuth auth, ExamQuestionnaire paper)
        {
            if (paper.Published)
            {
                return;
            }
            var userIds = new List<int>();

            if (paper.UserGroupIds != null && paper.UserGroupIds.Count > 0)
            {
                foreach (int groupId in paper.UserGroupIds)
                {
                    var group = await _userGroupRepository.GetAsync(groupId);
                    if (group != null)
                    {
                        if (group.GroupType == UsersGroupType.Fixed)
                        {
                            if (group.UserIds != null && group.UserIds.Count > 0)
                            {
                                userIds.AddRange(group.UserIds);
                            }

                        }
                        if (group.GroupType == Enums.UsersGroupType.Range)
                        {
                            if ((group.CompanyIds != null && group.CompanyIds.Count > 0) || (group.DepartmentIds != null && group.DepartmentIds.Count > 0) || (group.DutyIds != null && group.DutyIds.Count > 0))
                            {
                                var letUserIds = await _userRepository.GetUserIdsWithOutLockedAsync(auth, group.CompanyIds, group.DepartmentIds, group.DutyIds);
                                if (letUserIds != null && letUserIds.Count > 0)
                                {
                                    userIds.AddRange(letUserIds);
                                }
                            }


                        }
                        if (group.GroupType == UsersGroupType.All)
                        {
                            var letUserIds = await _userRepository.GetUserIdsWithOutLockedAsync(auth);
                            if (letUserIds != null && letUserIds.Count > 0)
                            {
                                userIds.AddRange(letUserIds);
                            }
                        }
                    }

                }
            }

            if (userIds.Count > 0)
            {
                userIds = userIds.Distinct().ToList();
                foreach (int userId in userIds)
                {
                    var exist = await _examQuestionnaireUserRepository.ExistsAsync(paper.Id, userId);
                    if (!exist)
                    {
                        var user = await _userRepository.GetByUserIdAsync(userId);
                        await _examQuestionnaireUserRepository.InsertAsync(new ExamQuestionnaireUser
                        {
                            ExamPaperId = paper.Id,
                            UserId = user.Id,
                            KeyWords = paper.Title,
                            KeyWordsAdmin = await _organManager.GetUserKeyWords(userId),
                            Locked = paper.Locked,
                            ExamBeginDateTime = paper.ExamBeginDateTime,
                            ExamEndDateTime = paper.ExamEndDateTime,
                            CompanyId = user.CompanyId,
                            DepartmentId = user.DepartmentId,
                            CreatorId = user.CreatorId
                        });
                    }
                }
            }
        }
    }
}
