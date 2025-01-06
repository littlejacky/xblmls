using Datory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Models;
using XBLMS.Repositories;
using XBLMS.Services;

namespace XBLMS.Core.Repositories
{
    public class StatLogRepository : IStatLogRepository
    {
        private readonly Repository<StatLog> _repository;

        public StatLogRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<StatLog>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;
        public async Task<StatLog> GetAsync(int id)
        {
            return await _repository.GetAsync(id);
        }

        public async Task InsertAsync(StatLog statLog)
        {
            await _repository.InsertAsync(statLog);
        }


        public async Task<(int total, List<StatLog> list)> GetListAsync(AuthorityAuth auth, DateTime? lowerDate, DateTime? higherDate, int pageIndex, int pageSize)
        {
            var query = Q.NewQuery();

            if (auth.AuthType == Enums.AuthorityType.Admin || auth.AuthType == Enums.AuthorityType.AdminCompany)
            {
                query.Where(nameof(ExamCer.CompanyId), auth.CurManageOrganId);
            }
            else if (auth.AuthType == Enums.AuthorityType.AdminDepartment)
            {
                query.Where(nameof(ExamCer.DepartmentId), auth.CurManageOrganId);
            }
            else
            {
                query.Where(nameof(ExamCer.CreatorId), auth.AdminId);
            }

            if (lowerDate.HasValue)
            {
                query.Where(nameof(Stat.CreatedDate), ">=", lowerDate.Value);
            }

            if (higherDate.HasValue)
            {
                query.Where(nameof(Stat.CreatedDate), "<=", higherDate.Value);
            }
            var total = await _repository.CountAsync(query);
            var list = await _repository.GetAllAsync(query.OrderByDesc(nameof(Stat.Id)).ForPage(pageIndex, pageSize));
            return (total, list);
        }

    }
}
