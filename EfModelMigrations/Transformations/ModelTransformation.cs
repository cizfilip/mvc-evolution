﻿using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Operations;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;

namespace EfModelMigrations.Transformations
{
    public abstract class ModelTransformation
    {
        public abstract IEnumerable<ModelChangeOperation> GetModelChangeOperations();

        public abstract MigrationOperation GetDbMigrationOperation(IDbMigrationOperationBuilder builder);

        public abstract ModelTransformation Inverse();
    }
}
