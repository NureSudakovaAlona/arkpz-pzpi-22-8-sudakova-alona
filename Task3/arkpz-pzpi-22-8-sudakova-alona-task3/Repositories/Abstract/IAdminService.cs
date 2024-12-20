namespace FocusLearn.Repositories.Abstract
{
    public interface IAdminService
    {
        Task<bool> ChangeUserStatusAsync(int userId, string status);
        Task<string> BackupDatabaseAsync();
        Task<IEnumerable<string>> ExportDataAsync(IEnumerable<string> tableNames);
        Task<string> ImportDataAsync(string tableName, IFormFile file);
    }
}
