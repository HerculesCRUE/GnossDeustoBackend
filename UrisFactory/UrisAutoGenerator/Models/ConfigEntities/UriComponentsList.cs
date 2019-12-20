using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace UrisFactory.Models.ConfigEntities
{
    public static class UriComponentsList
    {
        public const string Base = "base";
        public const string Character = "character";
        public const string ResourceClass = "resourceClass";
        public const string Identifier = "identifier";

        public static ImmutableList<String> DefaultParameters = new List<string> { Base, Character, ResourceClass,Identifier }.ToImmutableList();
    }
}
