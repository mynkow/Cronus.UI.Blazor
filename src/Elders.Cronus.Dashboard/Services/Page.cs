using Elders.Cronus.Dashboard.Models;

namespace Elders.Cronus.Dashboard.Services
{
    public class Table<TableData, PaginationToken>
    {
        public Table()
        {
            Pages = new HashSet<Page<TableData>>();
        }

        public Table(ISet<Page<TableData>> pages, PaginationToken lastToken)
        {
            LastToken = lastToken;

            if (pages is null)
                Pages = new HashSet<Page<TableData>>();
            Pages = pages;
        }

        public ISet<Page<TableData>> Pages { get; set; }

        public PaginationToken LastToken { get; set; }

        public int Take { get; set; }

        /// <summary>
        /// We need this because if there are no more records returned from cassandra, null pagination token will be returned and we start to load again from the beginning
        /// </summary>
        public bool ThereIsMoreDataToLoad { get; set; } = true;

        public bool IsEmpty => Pages.Any() == false;

        public int AllSavedItems => Pages.SelectMany(x => x.Items).Count();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowsPerPage"></param>
        /// <returns>True if we should rearange how we save the items in the collection</returns>
        public bool ShouldChangePaging(int rowsPerPage) => IsEmpty == false && Take != rowsPerPage;

        public async Task<Page<TableData>> GetOrAddItems(int page, int take, Func<int, Task<TableResult<TableData, PaginationToken>>> getItems)
        {
            var pagedData = GetPageResult(page);
            if (pagedData.Items.Any() == false && ThereIsMoreDataToLoad)
            {
                TableResult<TableData, PaginationToken> newItems = await getItems(take);

                if (newItems.Items.Any() == false)
                    return new Page<TableData>();

                else if (newItems.Token is null)
                    ThereIsMoreDataToLoad = false;

                else
                {
                    Take = take;
                    ChangeToken(newItems.Token);
                }

                pagedData = AddPageWithData(newItems.Items, page);
            }
            return pagedData;
        }

        public async Task<Page<TableData>> GetItemsAsync(int page, int take, Func<int, Task<TableResult<TableData, PaginationToken>>> getItems)
        {
            TableResult<TableData, PaginationToken> newItems = await getItems(take);
            if (newItems.Items.Count < take)
                ThereIsMoreDataToLoad = false;

            ChangeToken(newItems.Token);
            return new Page<TableData>(newItems.Items, page, newItems.Items.Count);
        }

        public Task ReorderItemsAccordingToPageSize(int pageSize, Page<TableData> additionalData = null)
        {
            List<TableData> flatItems = Pages.SelectMany(x => x.Items).ToList();

            if (additionalData is not null)
                flatItems.AddRange(additionalData.Items);

            Pages = new HashSet<Page<TableData>>();

            int pageNumber = 0;
            int taken = 0;
            int maxItems = flatItems.Count;
            bool itemsAreRemaining = true;

            while (itemsAreRemaining)
            {
                ISet<TableData> items = flatItems.Skip(taken).Take(pageSize).ToHashSet();
                Pages.Add(new Page<TableData>(items.ToHashSet(), pageNumber, items.Count));

                taken += items.Count;
                pageNumber++;

                if (taken >= maxItems)
                    itemsAreRemaining = false;
            }
            return Task.CompletedTask;
        }

        public Page<TableData> GetPageResult(int page)
        {
            Page<TableData> result = Pages.Where(x => x.CurrentPage.Equals(page)).FirstOrDefault();

            if (result is null)
                result = new Page<TableData>();
            return result;
        }

        public Page<TableData> AddPageWithData(ISet<TableData> events, int currentPage)
        {
            var page = new Page<TableData>(events, currentPage, events.Count);
            Pages.Add(page);

            return page;
        }

        public bool ShouldLoadMore(int pageSize, out int dataToTake)
        {
            dataToTake = 0;
            int allItems = AllSavedItems;
            int requested = pageSize * Pages.Count;

            bool shouldLoadMore = allItems < requested;
            if (shouldLoadMore)
                dataToTake = requested - allItems;

            return shouldLoadMore && ThereIsMoreDataToLoad;
        }

        private void ChangeToken(PaginationToken newLastToken) => LastToken = newLastToken;
    }
}

public class Page<TableData> : IEquatable<Page<TableData>>
{
    public Page(ISet<TableData> events, int currentPage, int rowData)
    {
        CurrentPage = currentPage;
        Items = events;
        RowData = rowData;
    }

    public Page()
    {
        Items = new HashSet<TableData>();
    }

    /// <summary>
    /// Corresponding page in table
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Collection of items in the table
    /// </summary>
    public ISet<TableData> Items { get; set; }

    public int RowData { get; set; }

    public bool Equals(Page<TableData> other)
    {
        if (other is null)
            return false;
        else
            return other.CurrentPage.Equals(CurrentPage);
    }
}
