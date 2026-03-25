
using AutoMapper;
using CodeBattleArena.Application.Common;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Application.Features.Items.Specifications;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Items;
using MediatR;

namespace CodeBattleArena.Application.Features.Items.Queries.GetItemsList
{
    public class GetItemsListHandler 
    : IRequestHandler<GetItemsListQuery, Result<PaginatedResult<ItemDto>>>
    {
        private readonly IRepository<Item> _itemRepository;
        private readonly IMapper _mapper;
        public GetItemsListHandler(IRepository<Item> itemRepository, IMapper mapper)
        {
            _itemRepository = itemRepository;
            _mapper = mapper;
        }
        public async Task<Result<PaginatedResult<ItemDto>>> Handle(
        GetItemsListQuery request, CancellationToken ct)
        {
            var spec = new ItemsListSpec(request.Filter);

            var items = await _itemRepository.GetListBySpecAsync(spec, ct);

            var totalCount = await _itemRepository.CountAsync(spec, ct);

            var dtos = _mapper.Map<List<ItemDto>>(items);

            var result = new PaginatedResult<ItemDto>(
                dtos,
                totalCount,
                request.Filter?.PageNumber ?? 1,
                request.Filter?.PageSize ?? 15);

            return Result<PaginatedResult<ItemDto>>.Success(result);
        }
    }
}
