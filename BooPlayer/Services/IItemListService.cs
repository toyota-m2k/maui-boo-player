using BooPlayer.Models;

namespace BooPlayer.Services;

internal interface IItemListService {
    Task<ItemList> GetItemListAsync(IHostEntry host);
}
