﻿using WorklogManagement.API.Interfaces;
using WorklogManagement.API.Models.Filter;

namespace WorklogManagement.API.Implements
{
    public class TicketStatusLogQuery : TicketStatusLogFilter, IQuery
    {
        public string Sort { get; set; } = "Id";
        public uint PageSize { get; set; } = 0;
        public uint PageIndex { get; set; } = 0;
        public string? Expand { get; set; }
    }
}
