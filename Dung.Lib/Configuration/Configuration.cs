using System.Collections.Generic;

namespace Dung.Lib.Configuration
{
    public struct Configuration
    {
        public string? Name { get; set; }
        public List<string>? Include { get; set; }
        public List<string>? Exclude { get; set; }
    }
}