using System;
using System.Collections.Generic;
using System.Text;

namespace Gresst.Domain.Entities
{
    public class PartyRelation
    {
        public required string fromPartyId { get; set; }
        public required string toPartyId { get; set; }
        public PartyRelationType relationType { get; set; }
        public DateTime? validFrom { get; set; }
        public DateTime? validTo { get; set; }
    }
}
