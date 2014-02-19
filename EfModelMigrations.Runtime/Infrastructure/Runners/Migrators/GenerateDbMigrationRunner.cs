﻿using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Infrastructure.Generators;
using EfModelMigrations.Runtime.Infrastructure.Migrations;
using EfModelMigrations.Transformations;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Design;
using System.Data.Entity.Migrations.Model;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace EfModelMigrations.Runtime.Infrastructure.Runners.Migrators
{
    [Serializable]
    internal class GenerateDbMigrationRunner : MigratorBaseRunner
    {
        public bool IsRevert { get; set; }
        public string OldEdmxModel { get; set; }

        public override void Run()
        {
            var operationBuilder = new DbMigrationOperationBuilder(Configuration.ModelNamespace, LoadEdmxFromString(OldEdmxModel), GetEdmxModel());

            IEnumerable<MigrationOperation> dbMigrationOperations = GetModelTransformations(IsRevert)
                .SelectMany(t => t.GetDbMigrationOperations(operationBuilder));
                        

            DbMigrationsConfiguration configuration = CreateInstance<DbMigrationsConfiguration>(Configuration.EfMigrationsConfigurationType);

            //TODO: idealni by bylo aby byl nas generator migraci i generator sql rovnou v configuracnim filu v projektu
            configuration.CodeGenerator = new ExtendedCSharpMigrationCodeGenerator(dbMigrationOperations);
            configuration.SetSqlGenerator("System.Data.SqlClient", new ExtendedSqlServerMigrationSqlGenerator());

            MigrationScaffolder scaffolder = new MigrationScaffolder(configuration);

            string dbMigrationName = ModelMigration.Name;
            if(IsRevert)
            {
                dbMigrationName = "Revert" + dbMigrationName;
            }

            var scaffoldedMigration = scaffolder.Scaffold(dbMigrationName, ignoreChanges: true);

            Return(scaffoldedMigration);
        }

        private XDocument LoadEdmxFromString(string edmx)
        {
            return XDocument.Parse(edmx);
        }
    }
}
