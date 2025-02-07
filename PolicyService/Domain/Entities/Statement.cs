using PolicyService;

namespace PolicyService.Domain.Entities
{
    public class Statement
    {
        public int StatementId { get; set; }
        public int Status { get; set; }
        public int StatementType { get; set; }
        public DateTime ActiveTime { get; set; }
        public int SectorId { get; set; }
        public int ReceiverType { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}