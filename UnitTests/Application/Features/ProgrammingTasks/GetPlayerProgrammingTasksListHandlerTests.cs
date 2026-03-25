
using Ardalis.Specification;
using AutoMapper;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Application.Features.ProgrammingTasks.Filters;
using CodeBattleArena.Application.Features.ProgrammingTasks.Queries.GetPlayerProgrammingTasksList;
using CodeBattleArena.Domain.ProgrammingTasks;
using FluentAssertions;
using Moq;
using UnitTests.Common.DataBuilders;
using UnitTests.Common.TestData;
using Xunit;

namespace UnitTests.Application.Features.ProgrammingTasks
{
    public class GetPlayerProgrammingTasksListHandlerTests
    {
        private readonly Mock<IRepository<ProgrammingTask>> _repoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetPlayerProgrammingTasksListHandler _handler;

        public GetPlayerProgrammingTasksListHandlerTests()
        {
            // Готовим "подделки" зависимостей
            _repoMock = new Mock<IRepository<ProgrammingTask>>();
            _mapperMock = new Mock<IMapper>();

            // Создаем хендлер, подсовывая ему наши подделки
            _handler = new GetPlayerProgrammingTasksListHandler(_repoMock.Object, _mapperMock.Object);
        }

        [Theory]
        [ClassData(typeof(BasePaginationTestData))]
        public async Task Handle_ShouldReturnCorrectPagination(int page, int size, int total, int expected)
        {
            // --- ARRANGE ---
            var filter = new ProgrammingTaskFilter { PageNumber = page, PageSize = size };
            var query = new GetPlayerProgrammingTasksListQuery(Guid.NewGuid(), filter);

            var tasks = ProgrammingTaskBuilder.CreateList(expected);
            var dtos = tasks.Select(t => new ProgrammingTaskDto { Name = t.Name }).ToList();

            // Настраиваем мок репозитория
            _repoMock.Setup(x => x.GetListBySpecAsync(It.IsAny<ISpecification<ProgrammingTask>>(), default))
                     .ReturnsAsync(tasks);

            // Настраиваем мок количества
            _repoMock.Setup(x => x.CountAsync(It.IsAny<ISpecification<ProgrammingTask>>(), default))
                     .ReturnsAsync(total);

            // Настраиваем маппер
            _mapperMock.Setup(x => x.Map<List<ProgrammingTaskDto>>(It.IsAny<List<ProgrammingTask>>()))
                       .Returns(dtos);

            // --- ACT ---
            var result = await _handler.Handle(query, CancellationToken.None);

            // --- ASSERT (используя FluentAssertions) ---
            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Should().HaveCount(dtos.Count);
            result.Value.TotalCount.Should().Be(total);

            // Проверка математики пагинации
            var expectedPages = (int)Math.Ceiling(total / (double)size);
            result.Value.TotalPages.Should().Be(expectedPages);

            // Проверяем, что репозиторий ДЕЙСТВИТЕЛЬНО вызывался
            _repoMock.Verify(x => x.GetListBySpecAsync(It.IsAny<ISpecification<ProgrammingTask>>(), default), Times.Once);
        }
    }
}
