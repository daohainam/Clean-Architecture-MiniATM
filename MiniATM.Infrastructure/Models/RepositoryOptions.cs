namespace MiniATM.Infrastructure.Models
{
    public class RepositoryOptions
    {
        public RepositoryTypes RepositoryType { get; set; } = RepositoryTypes.SqlServer;
    }

    public enum RepositoryTypes
    {
        SqlServer,
        Api
    }
}
