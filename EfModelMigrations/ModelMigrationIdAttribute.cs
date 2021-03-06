﻿using System;

namespace EfModelMigrations
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ModelMigrationIdAttribute : Attribute
    {
        public string Id { get; private set; }

        public ModelMigrationIdAttribute(string id)
        {
            Check.NotEmpty(id, "id");

            this.Id = id;
        }
    }
}
