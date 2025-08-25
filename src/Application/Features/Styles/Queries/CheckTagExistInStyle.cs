using Application.Abstractions;
using Application.Abstractions.IRepository;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Styles.Queries;

public class CheckTagExistInStyle
{
    public sealed record Query(string Style, string Tag) : IQuery<bool>;

    public sealed class Handler(IStyleRepository styleRepository) : IQueryHandler<Query, bool>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<bool>> Handle(Query query, CancellationToken cancellationToken)
        {
            return await _styleRepository.CheckTagExistsInStyleAsync(query.Style, query.Tag);
        }
    }
}
