﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;

namespace mvc_evolution.PowerShell.Extensions
{
    internal static class ProjectExtensions
    {

        public static string GetProjectDir(this Project project)
        {
            return project.GetPropertyValue<string>("FullPath");
        }


        private static T GetPropertyValue<T>(this Project project, string propertyName)
        {

            var property = project.Properties.Item(propertyName);

            if (property == null)
            {
                return default(T);
            }

            return (T)property.Value;
        }

    }
}
