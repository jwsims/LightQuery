﻿using System.Linq;
using System.Threading.Tasks;
using LightQuery.Client;
using LightQuery.Shared;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace LightQuery.EntityFrameworkCore
{
    public class AsyncLightQueryAttribute :  ActionFilterAttribute
    {
        public AsyncLightQueryAttribute(bool forcePagination = false, int defaultPageSize = QueryParser.DEFAULT_PAGE_SIZE)
        {
            _forcePagination = forcePagination;
            _defaultPageSize = defaultPageSize;
        }

        private readonly bool _forcePagination;
        private readonly int _defaultPageSize;

        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var queryContainer = ContextProcessor.GetQueryContainer(context, _defaultPageSize);
            if (queryContainer.ObjectResult == null)
            {
                return;
            }
            if (_forcePagination || queryContainer.QueryOptions.QueryRequestsPagination)
            {
                queryContainer.ObjectResult.Value = await GetPaginationResult(queryContainer);
            }
            else
            {
                queryContainer.ObjectResult.Value = await queryContainer.Queryable.Cast<object>().ToListAsync();
            }
            await next.Invoke();
        }

        private async Task<PaginationResult<object>> GetPaginationResult(QueryContainer queryContainer)
        {
            var queryOptions = queryContainer.QueryOptions;
            var queryable = queryContainer.Queryable;
            return new PaginationResult<object>
            {
                Page = queryOptions.Page,
                PageSize = queryOptions.PageSize,
                TotalCount = await queryable.Cast<object>().CountAsync(),
                Data = await queryable.Cast<object>().Skip((queryOptions.Page - 1) * queryOptions.PageSize).Take(queryOptions.PageSize).ToListAsync()
            };
        }
    }
}
