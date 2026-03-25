
using AutoMapper;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Application.Features.Items.Specifications;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Items;
using MediatR;

namespace CodeBattleArena.Application.Features.Items.Queries.GetItem
{
    public class GetItemHandler : IRequestHandler<GetItemQuery, Result<ItemDto>>
    {
        private readonly IRepository<Item> _itemRepository;
        private readonly IMapper _mapper;
        public GetItemHandler(IRepository<Item> itemRepository, IMapper mapper)
        {
            _itemRepository = itemRepository;
            _mapper = mapper;
        }
        public async Task<Result<ItemDto>> Handle(GetItemQuery request, CancellationToken cancellationToken)
        {
            var spec = new ItemReadOnlySpec(request.Id);
            var item = await _itemRepository.GetBySpecAsync(spec, cancellationToken);

            return Result<ItemDto>.Success(_mapper.Map<ItemDto>(item));
        }
    }
}
