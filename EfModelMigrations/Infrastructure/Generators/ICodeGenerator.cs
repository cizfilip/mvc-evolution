﻿using EfModelMigrations.Infrastructure.CodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.Generators
{
    public interface ICodeGenerator
    {
        string GenerateEmptyClass(string name, string @namespace,
            CodeModelVisibility visibility, string baseType,
            IEnumerable<string> implementedInterfaces);
        string GenerateProperty(PropertyCodeModel propertyModel);
        string GenerateDbSetProperty(string classNameForAddProperty);

        string GetFileExtensions();
    }
}
