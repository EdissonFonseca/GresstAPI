using System;
using System.Collections.Generic;
using System.Text;

namespace Gresst.Application.Queries
{
    public class PartyFilter
    {
        public PartyRelationType? Role { get; init; }
        public bool? IsActive { get; init; }
        public string? Search { get; init; }
    }
}
