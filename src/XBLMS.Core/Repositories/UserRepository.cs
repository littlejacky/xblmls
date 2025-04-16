using Datory;
using SqlKata;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using XBLMS.Core.Utils;
using XBLMS.Dto;
using XBLMS.Enums;
using XBLMS.Models;
using XBLMS.Repositories;
using XBLMS.Services;
using XBLMS.Utils;

namespace XBLMS.Core.Repositories
{
    public partial class UserRepository : IUserRepository
    {
        private readonly Repository<User> _repository;
        private readonly ICacheManager _cacheManager;
        private readonly IConfigRepository _configRepository;

        public UserRepository(ISettingsManager settingsManager, ICacheManager cacheManager, IConfigRepository configRepository)
        {
            _repository = new Repository<User>(settingsManager.Database, settingsManager.Redis);
            _cacheManager = cacheManager;
            _configRepository = configRepository;
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;
        public async Task<bool> ExistsAsync(int id)
        {
            return await _repository.ExistsAsync(id);
        }
        public async Task<(bool success, string errorMessage)> ValidateAsync(string userName, string email, string mobile, string password, string employeeId)
        {
            var config = await _configRepository.GetAsync();

            if (string.IsNullOrEmpty(password))
            {
                return (false, "密码不能为空");
            }
            if (password.Length < config.UserPasswordMinLength)
            {
                return (false, $"密码长度必须大于等于{config.UserPasswordMinLength}");
            }
            if (!PasswordRestrictionUtils.IsValid(password, config.UserPasswordRestriction))
            {
                return (false, $"密码不符合规则，请包含{config.UserPasswordRestriction.GetDisplayName()}");
            }
            if (string.IsNullOrEmpty(userName))
            {
                return (false, "用户名为空，请填写用户名");
            }
            if (!string.IsNullOrEmpty(userName) && await IsUserNameExistsAsync(userName))
            {
                return (false, "用户名已被注册，请更换用户名");
            }
            if (!IsUserNameCompliant(userName.Replace("@", string.Empty).Replace(".", string.Empty)))
            {
                return (false, "用户名包含不规则字符，请更换用户名");
            }

            if (!string.IsNullOrEmpty(email) && await IsEmailExistsAsync(email))
            {
                return (false, "电子邮件地址已被注册，请更换邮箱");
            }
            if (!string.IsNullOrEmpty(mobile) && await IsMobileExistsAsync(mobile))
            {
                return (false, "手机号码已被注册，请更换手机号码");
            }
            if (!string.IsNullOrEmpty(employeeId) && await IsEmployeeIdExistsAsync(employeeId))
            {
                return (false, "工号已被注册，请更换工号");
            }

            return (true, string.Empty);
        }
        private async Task<(bool success, string errorMessage)> InsertValidateAsync(string userName, string email, string mobile, string password, string ipAddress, string employeeId)
        {
            var config = await _configRepository.GetAsync();
            if (string.IsNullOrEmpty(password))
            {
                return (false, "密码不能为空");
            }
            if (password.Length < config.UserPasswordMinLength)
            {
                return (false, $"密码长度必须大于等于{config.UserPasswordMinLength}");
            }
            if (!PasswordRestrictionUtils.IsValid(password, config.UserPasswordRestriction))
            {
                return (false, $"密码不符合规则，请包含{config.UserPasswordRestriction.GetDisplayName()}");
            }
            if (string.IsNullOrEmpty(userName))
            {
                return (false, "用户名为空，请填写用户名");
            }
            if (!string.IsNullOrEmpty(userName) && await IsUserNameExistsAsync(userName))
            {
                return (false, "用户名已被注册，请更换用户名");
            }
            if (!IsUserNameCompliant(userName.Replace("@", string.Empty).Replace(".", string.Empty)))
            {
                return (false, "用户名包含不规则字符，请更换用户名");
            }

            if (!string.IsNullOrEmpty(email) && await IsEmailExistsAsync(email))
            {
                return (false, "电子邮件地址已被注册，请更换邮箱");
            }
            if (!string.IsNullOrEmpty(mobile) && await IsMobileExistsAsync(mobile))
            {
                return (false, "手机号码已被注册，请更换手机号码");
            }
            if (!string.IsNullOrEmpty(employeeId) && await IsEmployeeIdExistsAsync(employeeId))
            {
                return (false, "工号已被注册，请更换工号");
            }

            return (true, string.Empty);
        }

        public async Task<(User user, string errorMessage)> InsertAsync(User user, string password, bool isChecked, string ipAddress)
        {

            if (StringUtils.IsMobile(user.UserName) && string.IsNullOrEmpty(user.Mobile))
            {
                user.Mobile = user.UserName;
            }

            var (success, errorMessage) = await InsertValidateAsync(user.UserName, user.Email, user.Mobile, password, ipAddress, user.EmployeeId);
            if (!success)
            {
                return (null, errorMessage);
            }

            var passwordSalt = GenerateSalt();
            password = EncodePassword(password, PasswordFormat.Encrypted, passwordSalt);
            user.LastActivityDate = DateTime.Now;
            user.LastResetPasswordDate = DateTime.Now;

            user.Id = await InsertWithoutValidationAsync(user, password, PasswordFormat.Encrypted, passwordSalt);

            return (user, string.Empty);
        }

        private async Task<int> InsertWithoutValidationAsync(User user, string password, PasswordFormat passwordFormat, string passwordSalt)
        {
            user.LastActivityDate = DateTime.Now;
            user.LastResetPasswordDate = DateTime.Now;

            user.Password = password;
            user.PasswordFormat = passwordFormat;
            user.PasswordSalt = passwordSalt;
            user.Set("ConfirmPassword", string.Empty);
            user.Id = await _repository.InsertAsync(user);

            return user.Id;
        }

        public async Task<(bool success, string errorMessage)> UpdateAsync(User user)
        {
            var entity = await GetByUserIdAsync(user.Id);
            var cacheKeys = GetCacheKeysToRemove(entity);


            var valid = await UpdateValidateAsync(entity, user.Email, user.Mobile, user.EmployeeId);
            if (!valid.IsValid) return valid;

            await _repository.UpdateAsync(Q
                       .Set(nameof(user.LastActivityDate), user.LastActivityDate)
                       .Set(nameof(user.CountOfLogin), user.CountOfLogin)
                       .Set(nameof(user.CountOfFailedLogin), user.CountOfFailedLogin)
                       .Set(nameof(user.Locked), user.Locked)
                       .Set(nameof(user.DisplayName), user.DisplayName)
                       .Set(nameof(user.Mobile), user.Mobile)
                       .Set(nameof(user.Email), user.Email)
                       .Set(nameof(user.AvatarUrl), user.AvatarUrl)
                       .Set(nameof(user.AvatarbgUrl), user.AvatarbgUrl)
                       .Set(nameof(user.CompanyId), user.CompanyId)
                       .Set(nameof(user.DepartmentId), user.DepartmentId)
                       .Set(nameof(user.DutyId), user.DutyId)
                       .Where(nameof(user.Id), user.Id)
                       .CachingRemove(cacheKeys)
                   );

            return (true, string.Empty);
        }
        private async Task<(bool IsValid, string ErrorMessage)> UpdateValidateAsync(User user, string email, string mobile, string employeeId)
        {
            if (user.Mobile != null && user.Mobile != mobile)
            {
                if (!string.IsNullOrEmpty(user.Mobile) && await IsMobileExistsAsync(user.Mobile))
                {
                    return (false, "手机号码已被注册，请更换手机号码");
                }
            }

            if (user.Email != null && user.Email != email)
            {
                if (!string.IsNullOrEmpty(user.Email) && await IsEmailExistsAsync(user.Email))
                {
                    return (false, "电子邮件地址已被注册，请更换邮箱");
                }
            }

            if (user.EmployeeId != null && user.EmployeeId != employeeId)
            {
                if (!string.IsNullOrEmpty(user.EmployeeId) && await IsEmployeeIdExistsAsync(user.EmployeeId))
                {
                    return (false, "工号已被注册，请更换工号");
                }
            }

            return (true, string.Empty);
        }

        private async Task UpdateLastActivityDateAndCountOfFailedLoginAsync(User user)
        {
            if (user == null) return;

            user.LastActivityDate = DateTime.Now;
            user.CountOfFailedLogin += 1;

            await _repository.UpdateAsync(Q
                .Set(nameof(User.LastActivityDate), user.LastActivityDate)
                .Set(nameof(User.CountOfFailedLogin), user.CountOfFailedLogin)
                .Where(nameof(User.Id), user.Id)
                .CachingRemove(GetCacheKeysToRemove(user))
            );
        }

        public async Task UpdateLastActivityDateAndCountOfLoginAsync(User user)
        {
            if (user == null) return;

            user.LastActivityDate = DateTime.Now;
            user.CountOfLogin += 1;
            user.CountOfFailedLogin = 0;

            await _repository.UpdateAsync(Q
                .Set(nameof(User.LastActivityDate), user.LastActivityDate)
                .Set(nameof(User.CountOfLogin), user.CountOfLogin)
                .Set(nameof(User.CountOfFailedLogin), user.CountOfFailedLogin)
                .Where(nameof(User.Id), user.Id)
                .CachingRemove(GetCacheKeysToRemove(user))
            );
        }

        public async Task UpdateLastActivityDateAsync(User user)
        {
            if (user == null) return;

            user.LastActivityDate = DateTime.Now;

            await _repository.UpdateAsync(Q
                .Set(nameof(User.LastActivityDate), user.LastActivityDate)
                .Where(nameof(User.Id), user.Id)
                .CachingRemove(GetCacheKeysToRemove(user))
            );
        }

        private static string EncodePassword(string password, PasswordFormat passwordFormat, string passwordSalt)
        {
            var retVal = string.Empty;

            if (passwordFormat == PasswordFormat.Clear)
            {
                retVal = password;
            }
            else if (passwordFormat == PasswordFormat.Hashed)
            {
                var src = Encoding.Unicode.GetBytes(password);
                var buffer2 = Convert.FromBase64String(passwordSalt);
                var dst = new byte[buffer2.Length + src.Length];
                byte[] inArray = null;
                Buffer.BlockCopy(buffer2, 0, dst, 0, buffer2.Length);
                Buffer.BlockCopy(src, 0, dst, buffer2.Length, src.Length);
                var algorithm = SHA1.Create(); // HashAlgorithm.Create("SHA1");
                if (algorithm != null) inArray = algorithm.ComputeHash(dst);

                if (inArray != null) retVal = Convert.ToBase64String(inArray);
            }
            else if (passwordFormat == PasswordFormat.Encrypted)
            {
                retVal = TranslateUtils.EncryptStringBySecretKey(password, passwordSalt);

                //var des = new DesEncryptor
                //{
                //    InputString = password,
                //    EncryptKey = passwordSalt
                //};
                //des.DesEncrypt();

                //retVal = des.OutString;
            }
            return retVal;
        }

        private static string DecodePassword(string password, PasswordFormat passwordFormat, string passwordSalt)
        {
            var retVal = string.Empty;
            if (passwordFormat == PasswordFormat.Clear)
            {
                retVal = password;
            }
            else if (passwordFormat == PasswordFormat.Hashed)
            {
                throw new Exception("can not decode hashed password");
            }
            else if (passwordFormat == PasswordFormat.Encrypted)
            {
                retVal = TranslateUtils.DecryptStringBySecretKey(password, passwordSalt);

                //var des = new DesEncryptor
                //{
                //    InputString = password,
                //    DecryptKey = passwordSalt
                //};
                //des.DesDecrypt();

                //retVal = des.OutString;
            }
            return retVal;
        }

        private static string GenerateSalt()
        {
            var data = new byte[0x10];
            // new RNGCryptoServiceProvider().GetBytes(data);
            var rand = RandomNumberGenerator.Create();
            rand.GetBytes(data);

            return Convert.ToBase64String(data);
        }

        public async Task<(bool success, string errorMessage)> ChangePasswordAsync(int userId, string password)
        {
            var config = await _configRepository.GetAsync();
            if (password.Length < config.UserPasswordMinLength)
            {
                return (false, $"密码长度必须大于等于{config.UserPasswordMinLength}");
            }
            if (!PasswordRestrictionUtils.IsValid(password, config.UserPasswordRestriction))
            {
                return (false, $"密码不符合规则，请包含{config.UserPasswordRestriction.GetDisplayName()}");
            }

            var passwordSalt = GenerateSalt();
            password = EncodePassword(password, PasswordFormat.Encrypted, passwordSalt);
            await ChangePasswordAsync(userId, PasswordFormat.Encrypted, passwordSalt, password);
            return (true, string.Empty);
        }

        private async Task ChangePasswordAsync(int userId, PasswordFormat passwordFormat, string passwordSalt, string password)
        {
            var user = await GetByUserIdAsync(userId);
            if (user == null) return;

            user.LastResetPasswordDate = DateTime.Now;

            await _repository.UpdateAsync(Q
                .Set(nameof(User.Password), password)
                .Set(nameof(User.PasswordFormat), passwordFormat.GetValue())
                .Set(nameof(User.PasswordSalt), passwordSalt)
                .Set(nameof(User.LastResetPasswordDate), user.LastResetPasswordDate)
                .Where(nameof(User.Id), user.Id)
                .CachingRemove(GetCacheKeysToRemove(user))
            );
        }

        public async Task LockAsync(IList<int> userIds)
        {
            var cacheKeys = new List<string>();
            foreach (var userId in userIds)
            {
                var user = await GetByUserIdAsync(userId);
                cacheKeys.AddRange(GetCacheKeysToRemove(user));
            }

            await _repository.UpdateAsync(Q
                .Set(nameof(User.Locked), true)
                .WhereIn(nameof(User.Id), userIds)
                .CachingRemove(cacheKeys.ToArray())
            );
        }

        public async Task UnLockAsync(IList<int> userIds)
        {
            var cacheKeys = new List<string>();
            foreach (var userId in userIds)
            {
                var user = await GetByUserIdAsync(userId);
                cacheKeys.AddRange(GetCacheKeysToRemove(user));
            }

            await _repository.UpdateAsync(Q
                .Set(nameof(User.Locked), false)
                .Set(nameof(User.CountOfFailedLogin), 0)
                .WhereIn(nameof(User.Id), userIds)
                .CachingRemove(cacheKeys.ToArray())
            );
        }

        public async Task<bool> IsUserNameExistsAsync(string userName)
        {
            if (string.IsNullOrEmpty(userName)) return false;

            return await _repository.ExistsAsync(Q.Where(nameof(User.UserName), userName));
        }

        private static bool IsUserNameCompliant(string userName)
        {
            if (userName.IndexOf("　", StringComparison.Ordinal) != -1 || userName.IndexOf(" ", StringComparison.Ordinal) != -1 || userName.IndexOf("'", StringComparison.Ordinal) != -1 || userName.IndexOf(":", StringComparison.Ordinal) != -1 || userName.IndexOf(".", StringComparison.Ordinal) != -1)
            {
                return false;
            }
            return DirectoryUtils.IsDirectoryNameCompliant(userName);
        }

        public async Task<bool> IsMobileExistsAsync(string mobile)
        {
            if (string.IsNullOrEmpty(mobile)) return false;

            var exists = await IsUserNameExistsAsync(mobile);
            if (exists) return true;

            return await _repository.ExistsAsync(Q.Where(nameof(User.Mobile), mobile));
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;

            var exists = await IsUserNameExistsAsync(email);
            if (exists) return true;

            return await _repository.ExistsAsync(Q.Where(nameof(User.Email), email));
        }

        // 添加工号是否存在的检查方法
        public async Task<bool> IsEmployeeIdExistsAsync(string employeeId)
        {
            if (string.IsNullOrEmpty(employeeId)) return false;

            var exists = await IsUserNameExistsAsync(employeeId);
            if (exists) return true;

            return await _repository.ExistsAsync(Q.Where(nameof(User.EmployeeId), employeeId));
        }

        public async Task<(User user, string userName, string errorMessage)> ValidateAsync(string account, string password, bool isPasswordMd5)
        {
            if (string.IsNullOrEmpty(account))
            {
                return (null, null, "账号不能为空");
            }
            if (string.IsNullOrEmpty(password))
            {
                return (null, null, "密码不能为空");
            }

            var user = await GetByAccountAsync(account);

            if (string.IsNullOrEmpty(user?.UserName))
            {
                return (null, null, "帐号或密码错误");
            }

            var (success, errorMessage) = await ValidateStateAsync(user);
            if (!success)
            {
                return (null, user.UserName, errorMessage);
            }

            var userEntity = await GetByUserIdAsync(user.Id);

            var checkPassword = await CheckPasswordByUserIdAsync(user.Id, password, isPasswordMd5);

            if (!checkPassword)
            {
                await UpdateLastActivityDateAndCountOfFailedLoginAsync(user);
                return (null, user.UserName, "帐号或密码错误");
            }

            return (user, user.UserName, string.Empty);
        }
        private async Task<bool> CheckPasswordByUserIdAsync(int userId, string password, bool isPasswordMd5)
        {
            var dbUser = await _repository.GetAsync<User>(Q
                .Select(nameof(User.Password))
                .Select(nameof(User.PasswordFormat))
                .Select(nameof(User.PasswordSalt))
                .Where(nameof(User.Id), userId)
            );

            if (dbUser == null || string.IsNullOrEmpty(dbUser.Password) || string.IsNullOrEmpty(dbUser.PasswordSalt))
            {
                return false;
            }

            var decodePassword = DecodePassword(dbUser.Password, dbUser.PasswordFormat, dbUser.PasswordSalt);
            if (isPasswordMd5)
            {
                return password == AuthUtils.Md5ByString(decodePassword);
            }
            return password == decodePassword;
        }
        public async Task<(bool success, string errorMessage)> ValidateStateAsync(User user)
        {

            if (user.Locked)
            {
                return (false, "此账号被锁定，无法登录");
            }

            var config = await _configRepository.GetAsync();

            if (config.IsUserLockLogin)
            {
                if (user.CountOfFailedLogin > 0 && user.CountOfFailedLogin >= config.UserLockLoginCount)
                {
                    var lockType = TranslateUtils.ToEnum(config.UserLockLoginType, LockType.Hours);
                    if (lockType == LockType.Forever)
                    {
                        return (false, "此账号错误登录次数过多，已被永久锁定");
                    }
                    if (lockType == LockType.Hours && user.LastActivityDate.HasValue)
                    {
                        var ts = new TimeSpan(DateTime.Now.Ticks - user.LastActivityDate.Value.Ticks);
                        var hours = Convert.ToInt32(config.UserLockLoginHours - ts.TotalHours);
                        if (hours > 0)
                        {
                            return (false, $"此账号错误登录次数过多，已被锁定，请等待{hours}小时后重试");
                        }
                    }
                }
            }

            return (true, null);
        }

        public async Task<(int total, List<User> list)> GetListAsync(AuthorityAuth auth, List<int> companyIds, List<int> departmentIds, List<int> dutyIds, string keyWords, int pageIndex, int pageSize)
        {
            var query = Q.NewQuery();

            if (auth.AuthType == AuthorityType.AdminCompany || auth.AuthType == AuthorityType.Admin)
            {
                query.WhereIn(nameof(User.CompanyId), auth.CurManageOrganIds);
            }
            if (auth.AuthType == AuthorityType.AdminDepartment)
            {
                query.WhereIn(nameof(User.DepartmentId), auth.CurManageOrganIds);
            }
            if (auth.AuthType == AuthorityType.AdminSelf)
            {
                query.Where(nameof(User.CreatorId), auth.AdminId);
            }


            if (companyIds != null && companyIds.Count > 0)
            {
                query.WhereIn(nameof(User.CompanyId), companyIds);
            }
            if (departmentIds != null && departmentIds.Count > 0)
            {
                query.WhereIn(nameof(User.DepartmentId), departmentIds);
            }
            if (dutyIds != null && dutyIds.Count > 0)
            {
                query.WhereIn(nameof(User.DutyId), dutyIds);
            }

            if (!string.IsNullOrEmpty(keyWords))
            {
                var like = $"%{keyWords}%";
                query.Where(q => q
                    .WhereLike(nameof(User.UserName), like)
                    .OrWhereLike(nameof(User.Email), like)
                    .OrWhereLike(nameof(User.Mobile), like)
                    .OrWhereLike(nameof(User.DisplayName), like)
                );
            }
            var count = await _repository.CountAsync(query);
            var list = await _repository.GetAllAsync(query.ForPage(pageIndex, pageSize));
            return (count, list);
        }
        public async Task<List<int>> GetUserIdsWithOutLockedAsync(AuthorityAuth auth, List<int> companyIds, List<int> departmentIds, List<int> dutyIds)
        {
            var query = Q.Select(nameof(User.Id));

            if (auth.AuthType == AuthorityType.AdminCompany || auth.AuthType == AuthorityType.Admin)
            {
                query.WhereIn(nameof(User.CompanyId), auth.CurManageOrganIds);
            }
            if (auth.AuthType == AuthorityType.AdminDepartment)
            {
                query.WhereIn(nameof(User.DepartmentId), auth.CurManageOrganIds);
            }
            if (auth.AuthType == AuthorityType.AdminSelf)
            {
                query.Where(nameof(User.CreatorId), auth.AdminId);
            }

            if (companyIds != null && companyIds.Count > 0)
            {
                query.WhereIn(nameof(User.CompanyId), companyIds);
            }
            if (departmentIds != null && departmentIds.Count > 0)
            {
                query.WhereIn(nameof(User.DepartmentId), departmentIds);
            }
            if (dutyIds != null && dutyIds.Count > 0)
            {
                query.WhereIn(nameof(User.DutyId), dutyIds);
            }
            query.WhereNullOrFalse(nameof(User.Locked));
            return await _repository.GetAllAsync<int>(query);
        }
        public async Task<List<int>> GetUserIdsWithOutLockedAsync(AuthorityAuth auth)
        {
            var query = Q.Select(nameof(User.Id));
            query.WhereNullOrFalse(nameof(User.Locked));


            if (auth.AuthType == AuthorityType.AdminCompany || auth.AuthType == AuthorityType.Admin)
            {
                query.WhereIn(nameof(User.CompanyId), auth.CurManageOrganIds);
            }
            if (auth.AuthType == AuthorityType.AdminDepartment)
            {
                query.WhereIn(nameof(User.DepartmentId), auth.CurManageOrganIds);
            }
            if (auth.AuthType == AuthorityType.AdminSelf)
            {
                query.Where(nameof(User.CreatorId), auth.AdminId);
            }

            return await _repository.GetAllAsync<int>(query);
        }
        public async Task<int> GetCountAsync(int companyId, int departmentId, int dutyId)
        {
            var query = Q.NewQuery();
            if (companyId > 0)
            {
                query.Where(nameof(User.CompanyId), companyId);
            }
            if (departmentId > 0)
            {
                query.Where(nameof(User.DepartmentId), departmentId);
            }
            if (dutyId > 0)
            {
                query.Where(nameof(User.DutyId), dutyId);
            }

            return await _repository.CountAsync(query);
        }
        public async Task<int> GetCountAsync(List<int> companyIds, List<int> departmentIds, List<int> dutyIds)
        {
            var query = new Query();


            if (companyIds != null && companyIds.Count > 0)
            {
                query.WhereIn(nameof(User.CompanyId), companyIds);
            }
            if (departmentIds != null && departmentIds.Count > 0)
            {
                query.WhereIn(nameof(User.DepartmentId), departmentIds);
            }
            if (dutyIds != null && dutyIds.Count > 0)
            {
                query.WhereIn(nameof(User.DutyId), dutyIds);
            }

            return await _repository.CountAsync(query);
        }

        public async Task<User> DeleteAsync(int userId)
        {
            var user = await GetByUserIdAsync(userId);

            await _repository.DeleteAsync(userId, Q.CachingRemove(GetCacheKeysToRemove(user)));

            return user;
        }

    }
}

