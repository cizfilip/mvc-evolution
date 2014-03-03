﻿using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure.Generators.Templates;
using EfModelMigrations.Operations.Mapping;
using EfModelMigrations.Operations.Mapping.Model;
using EfModelMigrations.Transformations;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.Generators
{
    public class CSharpMappingInformationsGenerator : IMappingInformationsGenerator
    {
        private static readonly string Indent = "    ";
        private static readonly string ModelBuilderParameterName = "modelBuilder";
        
        public virtual string GetPrefixForOnModelCreatingUse(string entityName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ModelBuilderParameterName)
                .Append(GetSyntaxToken(CSharpTokenType.DotOperator))
                .Append("Entity")
                .Append(GetSyntaxToken(CSharpTokenType.GenericDefStart))
                .Append(entityName)
                .Append(GetSyntaxToken(CSharpTokenType.GenericDefEnd))
                .Append(GetSyntaxToken(CSharpTokenType.ParameterListStart))
                .Append(GetSyntaxToken(CSharpTokenType.ParameterListEnd));
            AppendIndentedNewLine(sb);
            sb.Append(GetSyntaxToken(CSharpTokenType.DotOperator));
            return sb.ToString();
        }

        public virtual GeneratedFluetApiCall GenerateFluentApiCall(EfFluentApiCallChain callChain)
        {
            var generatedMethodCalls = callChain.FluentApiCalls.Select(m => GenerateOneFluentApiCall(m));

            var sb = new StringBuilder();
            AppendIndentedNewLine(sb);
            sb.Append(GetSyntaxToken(CSharpTokenType.DotOperator));
            string methodCallSeparator = sb.ToString();

            var result = string.Join(methodCallSeparator, generatedMethodCalls) + GetSyntaxToken(CSharpTokenType.StatementSeparator);

            return new GeneratedFluetApiCall()
            {
                Content = result,
                TargetType = callChain.EntityType
            };
        }

        protected virtual string GenerateOneFluentApiCall(EfFluetApiCall fluentApiCall)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(GenerateMethodName(fluentApiCall.Method))
                    .Append(GetSyntaxToken(CSharpTokenType.ParameterListStart));

            string parameterSeparator = ParameterSeparatorWithWhitespace();
            foreach (var param in fluentApiCall.Parameters)
            {
                GenerateParameter(param, sb);
                sb.Append(parameterSeparator);
            }
            sb.Length = sb.Length - parameterSeparator.Length;

            sb.Append(GetSyntaxToken(CSharpTokenType.ParameterListEnd));

            return sb.ToString();
        }

        protected virtual void GenerateParameter(IEfFluentApiMethodParameter parameter, StringBuilder sb)
        {
            try
            {
                dynamic param = parameter;

                GenerateParameter(param, sb);
            }
            catch (RuntimeBinderException e)
            {
                //TODO: string do resourcu
                throw new ModelMigrationsException(string.Format("Cannot generate mapping information, because parameter of type {0} is not supported", parameter.GetType().Name), e);
            }
        }

        protected virtual void GenerateParameter(PropertySelectorParameter parameter, StringBuilder sb)
        {
            var lambdaParameterName = GetLambdaParameterName(parameter.ClassName);
            sb.Append(lambdaParameterName)
                .Append(GetLambdaOperator());

            if(parameter.PropertyNames.Length == 1)
            {
                sb.Append(lambdaParameterName)
                    .Append(GetSyntaxToken(CSharpTokenType.DotOperator))
                    .Append(parameter.PropertyNames[0]);
            }
            else
            {
                sb.Append("new")
                    .Append(GetWhiteSpace())
                    .Append(GetSyntaxToken(CSharpTokenType.BlockStart))
                    .Append(GetWhiteSpace());

                string parameterSeparator = ParameterSeparatorWithWhitespace();
                foreach (var property in parameter.PropertyNames)
                {
                    sb.Append(lambdaParameterName)
                        .Append(GetSyntaxToken(CSharpTokenType.DotOperator))
                        .Append(parameter)
                        .Append(parameterSeparator);
                }
                sb.Length = sb.Length - parameterSeparator.Length;

                sb.Append(GetWhiteSpace())
                    .Append(GetSyntaxToken(CSharpTokenType.BlockEnd));
            }
        }

        protected virtual void GenerateParameter(ValueParameter parameter, StringBuilder sb)
        {
            sb.Append(parameter.Value.ToString());
        }

        protected virtual void GenerateParameter(StringParameter parameter, StringBuilder sb)
        {
            sb.Append("\"")
                .Append(parameter.Value)
                .Append("\"");
        }

        protected virtual void GenerateParameter(MapMethodParameter parameter, StringBuilder sb)
        {
            var lambdaParameterName = GetLambdaParameterNameForMapMethod();
            sb.Append(lambdaParameterName)
                .Append(GetLambdaOperator());

            int mapMethodCallsCount = parameter.MapCalls.Count;

            if(mapMethodCallsCount == 0)
            {
                sb.Append(GetSyntaxToken(CSharpTokenType.BlockStart))
                    .Append(GetWhiteSpace())
                    .Append(GetSyntaxToken(CSharpTokenType.BlockEnd));
                
            }
            else if (mapMethodCallsCount == 1)
            {
                sb.Append(lambdaParameterName)
                    .Append(GetSyntaxToken(CSharpTokenType.DotOperator))
                    .Append(GenerateOneFluentApiCall(parameter.MapCalls[0]));
            }
            else
            {
                AppendIndentedNewLine(sb);
                sb.Append(GetSyntaxToken(CSharpTokenType.BlockStart));
                foreach (var mapMethodCall in parameter.MapCalls)
                {
                    AppendIndentedNewLine(sb, 2);
                    sb.Append(lambdaParameterName)
                        .Append(GetSyntaxToken(CSharpTokenType.DotOperator))
                        .Append(GenerateOneFluentApiCall(mapMethodCall))
                        .Append(GetSyntaxToken(CSharpTokenType.StatementSeparator));
                }
                AppendIndentedNewLine(sb);
                sb.Append(GetSyntaxToken(CSharpTokenType.BlockEnd));
            }
        }


        protected virtual string GetLambdaParameterName(string className)
        {
            return className.Take(1).Single().ToString().ToLower();
        }

        protected virtual string GetLambdaParameterNameForMapMethod()
        {
            return "m";
        }

        protected virtual string GenerateMethodName(EfFluentApiMethods method)
        {
            return Enum.GetName(typeof(EfFluentApiMethods), method);
        } 

        protected virtual string GetSyntaxToken(CSharpTokenType type)
        {
            switch (type)
            {
                case CSharpTokenType.DotOperator:
                    return ".";
                case CSharpTokenType.StatementSeparator:
                    return ";";
                case CSharpTokenType.BlockStart:
                    return "{";
                case CSharpTokenType.BlockEnd:
                    return "}";
                case CSharpTokenType.LambdaOperator:
                    return "=>";
                case CSharpTokenType.GenericDefStart:
                    return "<";
                case CSharpTokenType.GenericDefEnd:
                    return ">";
                case CSharpTokenType.ParameterListStart:
                    return "(";
                case CSharpTokenType.ParameterListEnd:
                    return ")";
                case CSharpTokenType.ParameterSeparator:
                    return ",";
                default:
                    throw new InvalidOperationException("Invalid CSharpTokenType."); //TODO: string do resourcu
            }
        }

        protected virtual void AppendIndentedNewLine(StringBuilder sb, int indent = 1)
        {
            sb.AppendLine();
            for (int i = 0; i < indent; i++)
            {
                sb.Append(Indent);
            }
        }   

        protected virtual string GetWhiteSpace()
        {
            return " ";
        }

        private string GetLambdaOperator()
        {
            return GetWhiteSpace() + GetSyntaxToken(CSharpTokenType.LambdaOperator) + GetWhiteSpace();
        }

        private string ParameterSeparatorWithWhitespace()
        {
            return GetSyntaxToken(CSharpTokenType.ParameterSeparator) + GetWhiteSpace();
        }
    }


    public enum CSharpTokenType
    {
        DotOperator,
        StatementSeparator,
        BlockStart,
        BlockEnd,
        LambdaOperator,
        GenericDefStart,
        GenericDefEnd,
        ParameterListStart,
        ParameterListEnd,
        ParameterSeparator,
    }
}
