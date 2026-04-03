
using AutoMapper;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Application.Features.Items.Specifications;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.PlayerItems;
using MediatR;

namespace CodeBattleArena.Application.Features.Items.Queries.GetPlayerActiveItemsList
{
    public class GetPlayerActiveItemsListHandler : IRequestHandler<GetPlayerActiveItemsListQuery, Result<IReadOnlyList<ItemDto>>>
    {
        private readonly IRepository<PlayerItem> _playerItemRepository;
        private readonly IMapper _mapper;
        public GetPlayerActiveItemsListHandler(IRepository<PlayerItem> playerItemRepository, IMapper mapper)
        {
            _playerItemRepository = playerItemRepository;
            _mapper = mapper;
        }

        public async Task<Result<IReadOnlyList<ItemDto>>> Handle(GetPlayerActiveItemsListQuery request, CancellationToken cancellationToken)
        {
            var spec = new PlayerActiveItemsListSpec(request.PlayerId);
            var playerActiveItems = await _playerItemRepository.GetListBySpecAsync(spec, cancellationToken);

            var playerActiveItemsDto = _mapper.Map<IReadOnlyList<ItemDto>>(playerActiveItems);

            return Result<IReadOnlyList<ItemDto>>.Success(playerActiveItemsDto);
        }
    }
}
