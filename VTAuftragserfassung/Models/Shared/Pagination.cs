using System.Collections.Immutable;

namespace VTAuftragserfassung.Models.Shared
{
    public class Pagination
    {
        #region Private Fields

        private readonly IReadOnlyList<int> linesPerPageOptions = [5, 10, 20, 30, 40, 50];
        private int linesPerPage = 20;

        #endregion Private Fields

        #region Public Properties

        public int LinesPerPage
        {
            get { return linesPerPage; }
            set => linesPerPage = LinesPerPageOptions.Contains(value) ? value : 20;
        }

        public IReadOnlyList<int> LinesPerPageOptions
        { get { return linesPerPageOptions.OrderBy(x => x).ToImmutableList(); } }

        public int Offset
        {
            get
            {
                return (Page - 1) * linesPerPage;
            }
        }

        public int Page { get; set; } = 1;

        #endregion Public Properties
    }
}