using CodeBattleArena.Server.IRepositories;
using CodeBattleArena.Server.Models;

namespace CodeBattleArena.Server.Services.Cache.DecorateDBService
{
    public class CachedLangProgrammingRepository : ILangProgrammingRepository
    {
        private readonly ILangProgrammingRepository _inner;
        private readonly ICacheService _cache;
        private const string CacheKeyPrefix = "lang:";
        private const string CacheKeyList = "list";

        public CachedLangProgrammingRepository(ILangProgrammingRepository inner, ICacheService cache)
        {
            _inner = inner;
            _cache = cache;
        }
        public async Task RemoveCacheListAsync() => await _cache.RemoveAsync(CacheKeyPrefix + CacheKeyList);

        public async Task<LangProgramming> GetLangProgrammingAsync(int id, CancellationToken ct)
        {
            var lang = await _cache.GetAsync<LangProgramming>(CacheKeyPrefix + id);
            if (lang != null)
                return lang;

            lang = await _inner.GetLangProgrammingAsync(id, ct);
            if (lang != null)
                await _cache.SetAsync(CacheKeyPrefix + lang.IdLang, lang);

            return lang;
        }
        public async Task<List<LangProgramming>> GetLangProgrammingListAsync(CancellationToken ct)
        {
            var langs = await _cache.GetAsync<List<LangProgramming>>(CacheKeyPrefix + CacheKeyList);
            if (langs != null)
                return langs;

            langs = await _inner.GetLangProgrammingListAsync(ct);
            if (langs != null)
                await _cache.SetAsync(CacheKeyPrefix + CacheKeyList, langs);

            return langs;
        }
        public async Task AddLangProgrammingAsync(LangProgramming langProgramming, CancellationToken ct)
        {
            await _inner.AddLangProgrammingAsync(langProgramming, ct);
            await RemoveCacheListAsync();
        }
        public async Task DeleteLangProgrammingAsync(int id, CancellationToken ct)
        {
            await _inner.DeleteLangProgrammingAsync(id, ct);
            await _cache.RemoveAsync(CacheKeyPrefix + id);
            await RemoveCacheListAsync();
        }
    }
}
