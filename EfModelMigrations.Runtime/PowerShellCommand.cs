﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EfModelMigrations.PowerShellDispatcher;
using EnvDTE;
using System.Reflection;

namespace EfModelMigrations.Runtime
{
    internal abstract class PowerShellCommand
    {
        private readonly AppDomain domain;
        private readonly Dispatcher dispatcher;
        private readonly string efDllPath;

        private string[] parameters;

        protected PowerShellCommand(string[] parameters)
        {
            this.parameters = parameters;

            this.domain = AppDomain.CurrentDomain;
            this.dispatcher = (Dispatcher)domain.GetData("dispatcher");

            this.efDllPath = (string)domain.GetData("efDllPath");
        }


        public string[] Parameters
        {
            get { return parameters; }
        }

        public Project Project
        {
            get { return (Project)domain.GetData("project"); }
        }

        public Project StartUpProject
        {
            get { return (Project)domain.GetData("startUpProject"); }
        }

        public Project ContextProject
        {
            get { return (Project)domain.GetData("contextProject"); }
        }

        protected AppDomain Domain
        {
            get { return domain; }
        }

        protected Assembly EFAssembly
        {
            get
            {
                return LoadAssemblyFromFile(efDllPath);
            }
        }



        public void Execute()
        {
            //DebugCheck.NotNull(command);

            Init();

            try
            {
                ExecuteCore();
            }
            catch (Exception ex)
            {
                Throw(ex);
            }
        }

        protected abstract void ExecuteCore();

        public virtual void WriteLine(string message)
        {
            //DebugCheck.NotEmpty(message);

            dispatcher.WriteLine(message);
        }

        public virtual void WriteWarning(string message)
        {
            //DebugCheck.NotEmpty(message);

            dispatcher.WriteWarning(message);
        }

        public void WriteVerbose(string message)
        {
            //DebugCheck.NotEmpty(message);

            dispatcher.WriteVerbose(message);
        }

        private void Init()
        {
            domain.SetData("wasError", false);
            domain.SetData("error.Message", null);
            domain.SetData("error.TypeName", null);
            domain.SetData("error.StackTrace", null);
        }

        private void Throw(Exception ex)
        {
            //DebugCheck.NotNull(ex);

            domain.SetData("wasError", true);
            domain.SetData("error.Message", ex.Message);
            domain.SetData("error.TypeName", ex.GetType().FullName);
            domain.SetData("error.StackTrace", ex.ToString());
        }


        protected Assembly LoadAssemblyFromFile(string path)
        {
            //TODO: Rethrow with our exception if FileNotFound
            return Assembly.LoadFile(path);
        }
    }
    
}
