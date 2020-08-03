using System;
using Microsoft.EntityFrameworkCore.Internal;

namespace Cosmos.EntityFrameworkCore
{
    public class EfCoreAutoHistoryOptions
    {
        public bool Enable { get; set; } = false;

        /// <summary>
        /// The maximum length of the 'Changed' column. <c>null</c> will use default setting 2048 unless ChangedVarcharMax is true
        /// in which case the column will be varchar(max). Default: null.
        /// </summary>
        public int? ChangedMaxLength { get; set; }

        /// <summary>
        /// Set this to true to enforce ChangedMaxLength. If this is false, ChangedMaxLength will be ignored.
        /// Default: true.
        /// </summary>
        public bool LimitChangedLength { get; set; } = true;

        /// <summary>
        /// The max length for the row id column. Default: 50.
        /// </summary>
        public int RowIdMaxLength { get; set; } = 50;

        /// <summary>
        /// The max length for the table column. Default: 128.
        /// </summary>
        public int TableMaxLength { get; set; } = 128;

        public Action<AutoHistoryOptions> ToConfigure()
        {
            return options =>
            {
                options.ChangedMaxLength = ChangedMaxLength;
                options.LimitChangedLength = LimitChangedLength;
                options.RowIdMaxLength = RowIdMaxLength;
                options.TableMaxLength = TableMaxLength;
            };
        }
    }
}