﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Transformations;
using EfModelMigrations.Runtime.Infrastructure.ModelChanges;
using EnvDTE;
using System.Data.Entity.Migrations;
using System.Data.Entity;

namespace EfModelMigrations.Runtime.Infrastructure.Runners.Migrators
{
    [Serializable]
    internal abstract class MigratorBaseRunner : BaseRunner
    {
        public string ModelMigrationId { get; set; }
        public Project ModelProject { get; set; }

        private ModelMigration modelMigration;
        protected ModelMigration ModelMigration
        {
            get
            {
                if (modelMigration == null)
                {
                    var locator = new ModelMigrationsLocator(Configuration);
                    var modelMigrationType = locator.FindModelMigration(ModelMigrationId);
                    modelMigration = CreateInstance<ModelMigration>(modelMigrationType);
                }
                return modelMigration;
            }
        }

        private DbMigrationsConfiguration dbConfiguration;
        protected DbMigrationsConfiguration DbConfiguration
        {
            get
            {
                if (dbConfiguration == null)
                {
                    dbConfiguration = CreateInstance<DbMigrationsConfiguration>(Configuration.EfMigrationsConfigurationType);
                }
                return dbConfiguration;
            }
        }

        private DbContext dbContext;
        protected DbContext DbContext
        {
            get
            {
                if (dbContext == null)
                {
                    dbContext = CreateInstance<DbContext>(DbConfiguration.ContextType);
                }
                return dbContext;
            }
        }

        protected IEnumerable<ModelTransformation> GetModelTransformations(bool isRevert)
        {
            ModelMigration.Reset();
            if (isRevert)
            {
                ModelMigration.Down();
            }
            else
            {
                ModelMigration.Up();
            }

            return ModelMigration.Transformations;
        }

        protected IClassModelProvider GetClassModelProvider()
        {
            return new VsClassModelProvider(ModelProject, Configuration.ModelNamespace);
        }

        public override abstract void Run();
        

        
    }
}
