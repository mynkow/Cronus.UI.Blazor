using Elders.Cronus.Dashboard.Models;
using MudBlazor;

namespace Elders.Cronus.Dashboard.Services
{
    public abstract class ServerTableManagerBase<TableData, PaginationToken>
    {
        protected Table<TableData, PaginationToken> Table = new Table<TableData, PaginationToken>();

        public PaginationToken Token => Table.LastToken;

        public virtual async Task<TableData<TableData>> ServerReload(TableState state, string id, Func<int, Task<TableResult<TableData, PaginationToken>>> getItems)
        {
            if (string.IsNullOrEmpty(id))
                return new TableData<TableData>();

            if (Table.ShouldChangePaging(state.PageSize))
                return await HandleNewPaging(state, getItems).ConfigureAwait(false);

            else
            {
                int take = state.PageSize;
                Page<TableData> result = await Table.GetOrAddItems(state.Page, state.PageSize, getItems);
                return new TableData<TableData>() { Items = result.Items, TotalItems = Table.AllSavedItems + 1 };
            }
        }

        public virtual async Task<TableData<TableData>> HandleNewPaging(TableState state, Func<int, Task<TableResult<TableData, PaginationToken>>> getItems)
        {
            Table.Take = state.PageSize;

            if (Table.IsEmpty)
                return new TableData<TableData>();

            Page<TableData> additionalItems = default;
            if (Table.ShouldLoadMore(state.PageSize, out int take))
                additionalItems = await Table.GetItemsAsync(state.Page, take, getItems);

            await Table.ReorderItemsAccordingToPageSize(state.PageSize, additionalItems);

            Page<TableData> itemsForPage = Table.GetPageResult(state.Page);
            return new TableData<TableData>() { Items = itemsForPage.Items, TotalItems = itemsForPage.Items.Count + 1 };
        }
    }
}
