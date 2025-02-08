using PolicyService.Infrastructure.Persistence;
using PolicyService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace PolicyService.Application.Services
{
    public interface IStatementService
    {
        Task<List<Statement>> GetAllStatements();
        Task<Statement> GetStatementById(int statementId);
        Task AddStatement(int status, int statementType, DateTime activeTime, int sector, int receiverType, string title, string content);
        Task DeleteStatement(int statementId);
        Task UpdateStatement(Statement statement);
    }
    
    public class StatementService : IStatementService
    {
        private readonly PolicyServiceDbContext _context;

        public StatementService(PolicyServiceDbContext context)
        {
            _context = context;
        }

        //TODO: IMPLEMETAR RESPOSTA PARA A API
        public async Task<List<Statement>> GetAllStatements()
        {
            return await _context.Statement.ToListAsync();
        }

        public async Task<Statement> GetStatementById(int statementId)
        {
            var statement = await _context.Statement.FindAsync(statementId);

            if (statement == null)
            {
                throw new ArgumentException("Statement não encontrado.");
            }

            return statement;
        }

        public async Task AddStatement(int status, int statementType, DateTime activeTime, int sector, int receiverType, string title, string content)
        {
            var statement = new Statement { Status = status, StatementType = statementType, ActiveTime = activeTime, SectorId = sector, ReceiverType = receiverType, Title = title, Content = content };
            _context.Statement.Add(statement);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteStatement(int statementId)
        {
            var statement = await _context.Statement.FindAsync(statementId);
            if (statement == null)
            {
                throw new ArgumentException("Statement não encontrado.");
            }
            _context.Statement.Remove(statement);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStatement(Statement statement)
        {
            var existingStatement = await _context.Statement.FindAsync(statement.StatementId);

            if (existingStatement == null)
            {
                throw new ArgumentException("Statement não encontrado.");
            }

            existingStatement.Status = statement.Status;
            existingStatement.StatementType = statement.StatementType;
            existingStatement.ActiveTime = statement.ActiveTime;
            existingStatement.SectorId = statement.SectorId;
            existingStatement.ReceiverType = statement.ReceiverType;
            existingStatement.Title = statement.Title;
            existingStatement.Content = statement.Content;

            await _context.SaveChangesAsync();
        }
    }
}
