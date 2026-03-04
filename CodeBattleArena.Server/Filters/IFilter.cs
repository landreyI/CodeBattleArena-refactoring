namespace CodeBattleArena.Server.Filters
{
    public interface IFilter<T>
    {
        IQueryable<T> ApplyTo(IQueryable<T> query);
        bool IsEmpty();
    }
}
