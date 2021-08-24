using System;
using Cosmos.Models.Descriptors.EntityDescriptors;
using Cosmos.Models.DomainModels;

namespace Cosmos.Models.Audits
{
    public class AuditHistory : AggregateRoot<AuditHistory, int>, IVerifyModel<AuditHistory>, ICreatedTime, IAuditModel
    {
        /// <summary>
        /// Gets or sets the source of row Id
        /// </summary>
        public string RowId { get; set; }

        /// <summary>
        /// Gets or sets the name of table
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Gets or sets the original version.
        /// </summary>
        public byte[] OriginalVersion { get; set; }

        /// <summary>
        /// Gets or sets the original value
        /// </summary>
        public string OriginalValue { get; set; }

        /// <summary>
        /// Gets or sets the current version.
        /// </summary>
        public byte[] CurrentVersion { get; set; }

        /// <summary>
        /// Gets or sets the current value
        /// </summary>
        public string CurrentValue { get; set; }

        /// <summary>
        /// Gets or sets the change kind
        /// </summary>
        public AuditState Kind { get; set; }

        /// <summary>
        /// Gets or sets the operator's Id
        /// </summary>
        public string OperatorId { get; set; }

        /// <summary>
        /// Gets or sets the created time of this item.
        /// </summary>
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
    }
}