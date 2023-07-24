using DAOLayer.Net7.Logs;
using LogHelper;
using System.Runtime.CompilerServices;

namespace FitappAdminWeb.Net7.Classes.Repositories
{
    public class LogRepository
    {
        private LogsContext _logcontext;
        private readonly ILogger<LogRepository> _logger;

        private List<Logs> LogQueue { get; set; }

        public LogRepository(ILogger<LogRepository> logger, LogsContext logcontext)
        {
            _logger = logger;
            _logcontext = logcontext;
        }

        private async Task AddLog(Logs log)
        {
            try
            {
                _logcontext.Add(log);
                await _logcontext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Warning, ex, "Failed to insert log");
            }
        }

        public async Task Log<sType>(Severity severity, string message, string innermessage, [CallerMemberName] string callerName = "")
        {
            try
            {
                Logs l = sLogHelper.Log(Component.Backend, nameof(sType), callerName, (int)severity, message, innermessage);
                await AddLog(l);

            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to add to Logs table.");
            }
        }
    }
}
