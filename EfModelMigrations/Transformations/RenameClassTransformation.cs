﻿using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Operations;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Transformations
{
    public class RenameClassTransformation : ModelTransformation
    {
        public string OldName { get; private set; }
        public string NewName { get; private set; }

        public RenameClassTransformation(string oldName, string newName)
        {
            this.OldName = oldName;
            this.NewName = newName;
        }

        public override IEnumerable<IModelChangeOperation> GetModelChangeOperations(IClassModelProvider modelProvider)
        {
            yield return new RenameClassOperation(OldName, NewName);
        }

        //TODO: V DB by bylo treba prejmenovat i vsechny reference - napr. jmena cizich klicu atd... ??
        public override MigrationOperation GetDbMigrationOperation(IDbMigrationOperationBuilder builder)
        {
            return builder.RenameTableOperation(OldName, NewName);
        }

        public override ModelTransformation Inverse()
        {
            return new RenameClassTransformation(NewName, OldName);
        }
    }
}
