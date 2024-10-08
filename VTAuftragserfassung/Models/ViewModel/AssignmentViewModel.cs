﻿using VTAuftragserfassung.Models.DBO;

namespace VTAuftragserfassung.Models.ViewModel
{
    public class AssignmentViewModel
    {
        public List<PositionViewModel>? PositionenVM { get; set; }
        public Auftrag? Auftrag { get; set; }
        public Kunde? Kunde { get; set; }
        public Gesellschafter? Gesellschafter { get; set; }
    }
}