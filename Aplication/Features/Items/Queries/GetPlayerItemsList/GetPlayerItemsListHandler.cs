
using AutoMapper;
using CodeBattleArena.Application.Common;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Application.Features.Items.Specifications;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.PlayerItems;
using MediatR;

namespace CodeBattleArena.Application.Features.Items.Queries.GetPlayerItemsList
{
    public class GetPlayerItemsListHandler 
    : IRequestHandler<GetPlayerItemsListQuery, Result<PaginatedResult<PlayerItemDto>>>
    {
        private readonly IRepository<PlayerItem> _playerItemRepository;
        private readonly IMapper _mapper;
        public GetPlayerItemsListHandler(IRepository<PlayerItem> playerItemRepository, IMapper mapper)
        {
            _playerItemRepository = playerItemRepository;
            _mapper = mapper;
        }
        public async Task<Result<PaginatedResult<PlayerItemDto>>> Handle(GetPlayerItemsListQuery request, CancellationToken cancellationToken)
        {
            var spec = new PlayerItemsListSpec(request.PlayerId, request.Filter);
            var playerItems = await _playerItemRepository.GetListBySpecAsync(spec, cancellationToken);
            var totalCount = await _playerItemRepository.CountAsync(spec, cancellationToken);

            var dtos = _mapper.Map<List<PlayerItemDto>>(playerItems);

            var result = new PaginatedResult<PlayerItemDto>(
                dtos,
                totalCount,
                request.Filter?.PageNumber ?? 1,
                request.Filter?.PageSize ?? 15);

            return Result<PaginatedResult<PlayerItemDto>>.Success(result);
        }
    }
}
