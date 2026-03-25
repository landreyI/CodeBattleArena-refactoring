
using AutoMapper;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Application.Features.Items.Specifications;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.PlayerItems;
using MediatR;

namespace CodeBattleArena.Application.Features.Items.Queries.GetPlayerItem
{
    public class GetPlayerItemHandler : IRequestHandler<GetPlayerItemQuery, Result<PlayerItemDto>>
    {
        private readonly IRepository<PlayerItem> _playerItemsRepository;
        private readonly IMapper _mapper;
        public GetPlayerItemHandler(IRepository<PlayerItem> playerItemsRepository, IMapper mapper)
        {
            _playerItemsRepository = playerItemsRepository;
            _mapper = mapper;
        }

        public async Task<Result<PlayerItemDto>> Handle(GetPlayerItemQuery request, CancellationToken cancellationToken)
        {
            var spec = new PlayerItemReadOnlySpec(request.PlayerId, request.ItemId);
            var playerItem = await _playerItemsRepository.GetBySpecAsync(spec, cancellationToken);

            return Result<PlayerItemDto>.Success(_mapper.Map<PlayerItemDto>(playerItem));
        }
    }
}
