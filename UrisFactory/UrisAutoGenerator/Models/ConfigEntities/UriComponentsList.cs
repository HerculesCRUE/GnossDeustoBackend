using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace UrisFactory.Models.ConfigEntities
{
    ///<summary>
    ///almacena los nombres 
    ///</summary>
    public static class UriComponentsList
    {
        public const string Base = "base";
        public const string Character = "character";
        public const string ResourceClass = "resourceClass";
        public const string Identifier = "identifier";

        private static ImmutableList<String> defaultParameters = new List<string> { Base, Character, ResourceClass, Identifier }.ToImmutableList();

        public static ImmutableList<String> DefaultParameters
        {
            get
            {
                return defaultParameters;
            }
        }
    }
}
