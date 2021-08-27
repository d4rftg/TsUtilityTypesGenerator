using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace TsUtilityTypesGenerator.Templates
{
    public partial class PickTemplate
    {
        public ITypeSymbol PickBaseType { get; set; } = null!;
        public IEnumerable<string> PickTargetProperties { get; set; } = null!;
        public string DestTypeName { get; set; } = null!;
        public string DestTypeNamespace { get; set; } = null!;
    }
}