using System.Collections.Immutable;

namespace VTAuftragserfassung.Models.Shared
{
    public class Pagination
    {
        #region Private Fields

        private readonly IReadOnlyList<int> _linesPerPageOptions = [5, 10, 20, 30, 40, 50];
        private int _linesPerPage = 20;

        #endregion Private Fields

        #region Public Properties

        public int LinesPerPage
        {
            get { return _linesPerPage; }
            set => _linesPerPage = LinesPerPageOptions.Contains(value) ? value : 20;
        }

        public IReadOnlyList<int> LinesPerPageOptions
        { get { return _linesPerPageOptions.OrderBy(x => x).ToImmutableList(); } }

        public int Offset => (Page - 1) * _linesPerPage;

        public int Page { get; set; } = 1;

        #endregion Public Properties
    }
}