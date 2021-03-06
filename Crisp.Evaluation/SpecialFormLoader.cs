﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Crisp.Interfaces.Evaluation;
using Crisp.Interfaces.Shared;
using Crisp.Interfaces.Types;
using Crisp.Types;

namespace Crisp.Evaluation
{
    public class SpecialFormLoader : ISpecialFormLoader
    {
        private readonly ISpecialFormDirectoryPathProvider _specialFormDirectoryPathProvider;

        /// <summary>
        /// Gets whether or not a type is a special form type for loading.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns></returns>
        private static bool IsSpecialFormType(Type type)
        {
            // Type must be public, concrete and subclass SpecialForm.
            return type.IsPublic
                && !type.IsAbstract
                && type.BaseType == typeof(SpecialForm);
        }

        public virtual Dictionary<string, ISymbolicExpression> GetBindings()
        {
            // Prepare list of definitions to pass back.
            var definitions = new Dictionary<string, ISymbolicExpression>();

            // Loop through each file in target directory.
            foreach (var file in Directory.GetFiles(_specialFormDirectoryPathProvider.Get(), "*.dll"))
            {
                // Load assembly and find special form types.
                var assembly = Assembly.LoadFrom(file);
                var types = assembly.GetTypes().Where(IsSpecialFormType);

                // Loop through each special form and add definition.
                foreach (var type in types)
                {
                    var function = (SpecialForm)Activator.CreateInstance(assembly.GetType(type.ToString()));
                    foreach (var name in function.Names)
                    {
                        definitions.Add(name, function);
                    }
                }
            }

            return definitions;
        }

        /// <summary>
        /// Initializes a new instance of a special form loading service.
        /// </summary>
        /// <param name="specialFormDirectoryPathProvider">The special form directory path provider service.</param>
        public SpecialFormLoader(ISpecialFormDirectoryPathProvider specialFormDirectoryPathProvider)
        {
            _specialFormDirectoryPathProvider = specialFormDirectoryPathProvider;
        }
    }
}
