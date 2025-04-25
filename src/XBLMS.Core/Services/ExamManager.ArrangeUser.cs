using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Enums;
using XBLMS.Models;
using XBLMS.Services; // Assuming INotificationService is in this namespace

namespace XBLMS.Core.Services
{
    public partial class ExamManager
    {
        // Assuming INotificationService _notificationService; is declared and injected in another partial class file or constructor

        public async Task Arrange(ExamPaper paper, AuthorityAuth auth)
        {
            var userIds = new List<int>();

            if (paper.UserGroupIds != null && paper.UserGroupIds.Count > 0)
            {
                var allUser = false;
                foreach (int groupId in paper.UserGroupIds)
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
                    foreach (int groupId in paper.UserGroupIds)
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
                    var exist = await _examPaperUserRepository.ExistsAsync(paper.Id, userId);
                    if (!exist)
                    {
                        var user = await _userRepository.GetByUserIdAsync(userId);

                        await _examPaperUserRepository.InsertAsync(new ExamPaperUser
                        {
                            ExamTimes = paper.ExamTimes,
                            ExamBeginDateTime = paper.ExamBeginDateTime,
                            ExamEndDateTime = paper.ExamEndDateTime,
                            ExamPaperId = paper.Id,
                            UserId = user.Id,
                            KeyWordsAdmin = await _organManager.GetUserKeyWords(userId),
                            KeyWords = paper.Title,
                            Locked = paper.Locked,
                            Moni = paper.Moni,
                            CompanyId = user.CompanyId,
                            DepartmentId = user.DepartmentId,
                            CreatorId = user.CreatorId
                        });

                        // Send notification after arranging exam for the user
                        // TODO: Ensure _notificationService is properly injected and available
                        // await _notificationService.SendExamArrangedNotificationAsync(userId, paper.Id, paper.Title);
                        await _notificationManager.SendExamPaperArrangedNotificationAsync(user, paper);
                    }
                }
            }
        }
        public async Task Arrange(int paperId, int userId)
        {
            var paper = await _examPaperRepository.GetAsync(paperId);
            var exists = await _examPaperUserRepository.ExistsAsync(paperId, userId);
            if (!exists)
            {
                var user = await _userRepository.GetByUserIdAsync(userId);
                await _examPaperUserRepository.InsertAsync(new ExamPaperUser
                {
                    ExamTimes = paper.ExamTimes,
                    ExamBeginDateTime = paper.ExamBeginDateTime,
                    ExamEndDateTime = paper.ExamEndDateTime,
                    ExamPaperId = paper.Id,
                    UserId = userId,
                    KeyWordsAdmin = await _organManager.GetUserKeyWords(userId),
                    KeyWords = paper.Title,
                    Locked = paper.Locked,
                    Moni = paper.Moni,
                    CompanyId = user.CompanyId,
                    DepartmentId = user.DepartmentId,
                    CreatorId = user.CreatorId
                });

                // Send notification after arranging exam for the user
                // TODO: Ensure _notificationService is properly injected and available
                // await _notificationService.SendExamArrangedNotificationAsync(userId, paper.Id, paper.Title);
                await _notificationManager.SendExamPaperArrangedNotificationAsync(user, paper);
            }
        }

        public async Task<List<int>> Arrange(ExamPlan paper, AuthorityAuth auth)
        {
            var userIds = new List<int>();

            if (paper.UserGroupIds != null && paper.UserGroupIds.Count > 0)
            {
                var allUser = false;
                foreach (int groupId in paper.UserGroupIds)
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
                    foreach (int groupId in paper.UserGroupIds)
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
                    var exist = await _examPaperUserRepository.ExistsAsync(paper.Id, userId);
                    if (!exist)
                    {
                        var user = await _userRepository.GetByUserIdAsync(userId);

                        await _examPaperUserRepository.InsertAsync(new ExamPaperUser
                        {
                            ExamTimes = paper.ExamTimes,
                            ExamBeginDateTime = paper.ExamBeginDateTime,
                            ExamEndDateTime = paper.ExamEndDateTime,
                            ExamPaperId = paper.Id,
                            UserId = user.Id,
                            KeyWordsAdmin = await _organManager.GetUserKeyWords(userId),
                            KeyWords = paper.Title,
                            Locked = paper.Locked,
                            Moni = paper.Moni,
                            CompanyId = user.CompanyId,
                            DepartmentId = user.DepartmentId,
                            CreatorId = user.CreatorId
                        });

                        // Send notification after arranging exam for the user
                        // TODO: Ensure _notificationService is properly injected and available
                        // await _notificationService.SendExamArrangedNotificationAsync(userId, paper.Id, paper.Title);
                        await _notificationManager.SendExamPaperArrangedNotificationAsync(user, paper);
                    }
                }
            }

            return userIds;
        }
    }
}
