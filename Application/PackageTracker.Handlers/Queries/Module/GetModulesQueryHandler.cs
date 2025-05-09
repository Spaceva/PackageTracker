﻿using PackageTracker.Domain.Modules;
using PackageTracker.Messages.Queries;
using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Handlers.Queries;

internal class GetModulesQueryHandler(IModuleManager moduleManager) : IRequestHandler<GetModulesQuery, GetModulesQueryResponse>
{
    public async Task<GetModulesQueryResponse> Handle(GetModulesQuery request, CancellationToken cancellationToken) => new GetModulesQueryResponse { Modules = await moduleManager.GetAllAsync(cancellationToken) };
}
