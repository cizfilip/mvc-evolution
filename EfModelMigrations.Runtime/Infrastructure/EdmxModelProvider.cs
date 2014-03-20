﻿using EfModelMigrations.Runtime.Infrastructure.Runners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Runtime.Infrastructure
{
    internal sealed class EdmxModelProvider : MarshalByRefObject
    {
        private Func<NewAppDomainExecutor> executorFactory;

        public EdmxModelProvider(Func<NewAppDomainExecutor> executorFactory)
        {
            this.executorFactory = executorFactory;
        }

        public string GetEdmxModel()
        {
            using (var executor = executorFactory())
            {
                return executor.ExecuteRunner<string>(new GetEdmxRunner());
            }
        }
    }
}
