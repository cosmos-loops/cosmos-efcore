﻿namespace Cosmos.EntityFrameworkCore
{
    public class KeyEntry
    {
        public string PropertyName { get; set; } = null!;

        public string ColumnName { get; set; } = null!;

        public object Value { get; set; } = null!;
    }
}