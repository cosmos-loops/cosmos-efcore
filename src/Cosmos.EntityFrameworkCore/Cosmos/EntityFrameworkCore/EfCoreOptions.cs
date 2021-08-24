using System;
using Cosmos.Models.Audits;
using Cosmos.Reflection;

namespace Cosmos.EntityFrameworkCore
{
    /// <summary>
    /// EfCore options
    /// </summary>
    public class EfCoreOptions
    {
        /// <summary>
        /// Connection name
        /// </summary>
        public string ConnectionName { get; set; }

        /// <summary>
        /// Connection string
        /// </summary>
        public string ConnectionString { get; set; }

        #region RangeOperation

        /// <summary>
        /// Enable Audit, default is true.
        /// </summary>
        public bool EnableRangeOperation { get; set; } = true;

        #endregion
        
        #region AuditHistory

        /// <summary>
        /// Enable Audit, default is false.
        /// </summary>
        public bool EnableAudit { get; set; } = false;

        /// <summary>
        /// Internal
        /// </summary>
        internal AuditHistoryOptions AuditHistoryOptions { get; set; }

        /// <summary>
        /// Configure AuditHistoryOptions
        /// </summary>
        /// <param name="configure"></param>
        public void AuditConfig(Action<AuditHistoryOptions> configure)
        {
            if (configure is not null)
            {
                EnableAudit = true;
                if (AuditHistoryOptions is null)
                    AuditHistoryOptions = AuditHistoryOptions.New();
                configure.Invoke(AuditHistoryOptions);
            }
        }

        #endregion

        /// <summary>
        /// To Create a new Microsoft EntityFramework Core options
        /// </summary>
        /// <returns></returns>
        public static EfCoreOptions Create() => new();

        /// <summary>
        /// To Create a new Microsoft EntityFramework Core options
        /// </summary>
        /// <param name="optAct"></param>
        /// <returns></returns>
        public static EfCoreOptions Create(Action<EfCoreOptions> optAct)
        {
            var opt = Create();
            optAct?.Invoke(opt);
            return opt;
        }
    }

    public static class EfCoreOptionsGuard
    {
        /// <summary>
        /// Guard Microsoft EntityFramework Core options
        /// </summary>
        /// <param name="options"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void For(EfCoreOptions options)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            if (string.IsNullOrWhiteSpace(options.ConnectionString))
                throw new ArgumentNullException(nameof(options.ConnectionString));
        }
    }
}