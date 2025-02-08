using PolicyService.Infrastructure.Persistence;
using PolicyService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace PolicyService.Application.Services
{
    public interface IStatementService
    {
        Task AddStatement(int status, int statementType, DateTime activeTime, int sector, int receiverType, string title, string content);
        Task<List<Statement>> GetStatement();
    }
    public class StatementService : IStatementService
    {
        //TODO: IMPLEMETAR RESPOSTA PARA A API
        private readonly PolicyServiceDbContext _context;

        public StatementService(PolicyServiceDbContext context)
        {
            _context = context;
        }

        public async Task AddStatement(int status, int statementType, DateTime activeTime, int sector, int receiverType, string title, string content)
        {
            var statement = new Statement { Status = status, StatementType = statementType, ActiveTime = activeTime, SectorId = sector, ReceiverType = receiverType, Title = title, Content = content };
            _context.Statement.Add(statement);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Statement>> GetStatement()
        {
            return await _context.Statement.ToListAsync();
        }
    }
}
