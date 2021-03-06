﻿using EfModelMigrations.Operations.Mapping.Model;
using EfModelMigrations.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.CodeModel
{
    public sealed class ColumnInfo
    {
        public ColumnInfo()
        {
            this.ColumnAnnotations = new Dictionary<string, object>();
        }

        public bool? IsNullable { get; set; }
        public string ColumnName { get; set; }
        public string ColumnType { get; set; }
        public int? ColumnOrder { get; set; }
        public IDictionary<string, object> ColumnAnnotations { get; private set; }
        public DatabaseGeneratedOption? DatabaseGeneratedOption { get; set; }
        public bool? IsConcurrencyToken { get; set; }
        public string ParameterName { get; set; }
        public int? MaxLength { get; set; }
        public bool? IsMaxLength { get; set; }
        public bool? IsFixedLength { get; set; }
        public bool? IsUnicode { get; set; }
        public byte? Precision { get; set; }
        public byte? Scale { get; set; }
        public bool? IsRowVersion { get; set; }


        public ColumnInfo Copy()
        {
            var copy = new ColumnInfo();

            copy.IsNullable = IsNullable;
            copy.ColumnName = ColumnName;
            copy.ColumnType = ColumnType;
            copy.ColumnOrder = ColumnOrder;
            copy.ColumnAnnotations.AddRange(ColumnAnnotations);
            copy.DatabaseGeneratedOption = DatabaseGeneratedOption;
            copy.IsConcurrencyToken = IsConcurrencyToken;
            copy.ParameterName = ParameterName;
            copy.MaxLength = MaxLength;
            copy.IsMaxLength = IsMaxLength;
            copy.IsFixedLength = IsFixedLength;
            copy.IsUnicode = IsUnicode;
            copy.Precision = Precision;
            copy.Scale = Scale;
            copy.IsRowVersion = IsRowVersion;

            return copy;
        }

        public bool IsSomethingSpecified()
        {
            return IsNullable.HasValue &&
                !string.IsNullOrWhiteSpace(ColumnName) &&
                !string.IsNullOrWhiteSpace(ColumnType) &&
                ColumnOrder.HasValue &&
                ColumnAnnotations.Any() &&
                DatabaseGeneratedOption.HasValue &&
                IsConcurrencyToken.HasValue &&
                !string.IsNullOrWhiteSpace(ParameterName) &&
                MaxLength.HasValue &&
                IsMaxLength.HasValue &&
                IsFixedLength.HasValue &&
                IsUnicode.HasValue &&
                Precision.HasValue &&
                Scale.HasValue &&
                IsRowVersion.HasValue;
        }
    }
}
