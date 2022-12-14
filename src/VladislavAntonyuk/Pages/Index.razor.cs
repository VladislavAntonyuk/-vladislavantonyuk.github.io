﻿using Microsoft.AspNetCore.Components;
using VladislavAntonyuk.Application.UseCases;
using VladislavAntonyuk.Models;
using VladislavAntonyuk.Services;

namespace VladislavAntonyuk.Pages;

public partial class Index : VladislavAntonyukBaseComponent
{
	[Parameter]
	[SupplyParameterFromQuery]
	public int? Page { get; set; }

	[Parameter]
	[SupplyParameterFromQuery]
	public string? CategoryName { get; set; }

	[Inject]
	public required IUrlCreator UrlCreator { get; set; }

	[Inject]
	public required IArticlesService ArticlesService { get; set; }

    private async Task<PaginatedList<Article>> LoadArticles(int page, string? searchParameter)
    {
        var articles = await ArticlesService.GetArticles(CategoryName, searchParameter);
        
        var result = articles.Skip(page - 1).Take(10).ToList();
       
        return new PaginatedList<Article>(result, articles.Count, page - 1, 10);
	}
}