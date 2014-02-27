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

    public class JoinComplexTypeTransformation : TransformationWithInverse
    {
        public string ClassName { get; set; }
        public string ComplexTypeName { get; private set; }

        public JoinComplexTypeTransformation(string complexTypeName, string className, ModelTransformation inverse)
            : base(inverse)
        {
            this.ClassName = className;
            this.ComplexTypeName = complexTypeName;
        }

        public JoinComplexTypeTransformation(string complexTypeName, string className)
            : this(complexTypeName, className, null)
        {
        }

        public override IEnumerable<IModelChangeOperation> GetModelChangeOperations(IClassModelProvider modelProvider)
        {
            var complexModel = modelProvider.GetClassCodeModel(ComplexTypeName);

            foreach (var property in complexModel.Properties)
            {
                yield return new MovePropertyOperation(ComplexTypeName, ClassName, property.Name);
            }

            yield return new RemoveClassOperation(ComplexTypeName);

            var navigationProperties = modelProvider.GetClassCodeModel(ClassName).Properties.Where(p => p.Type.Contains(ComplexTypeName));

            foreach (var navigationProperty in navigationProperties)
            {
                yield return new RemovePropertyFromClassOperation(ClassName, navigationProperty.Name);
            }
        }

        public override IEnumerable<MigrationOperation> GetDbMigrationOperations(IDbMigrationOperationBuilder builder)
        {
            return builder.RenameColumnOperationsForJoinComplexType(ComplexTypeName, ClassName);
        }

    }

}