﻿namespace blackcat.Models.viewModels;

public class Pager
{
    public int TotalItems { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    
    public int TotalPages { get; set; }
    public int StartPage { get; set; }
    public int EndPage { get; set; }
    
    public Pager(){

    }

    public Pager(int totalItems, int page, int pageSize = 5)
    {
        int totalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);
        int currentPage = page;

        int startPage = currentPage - 2;
        int endPage = currentPage + 2;

        if (startPage <= 0)
        {
            endPage += (1 - startPage);
            startPage = 1;
        }

        if (endPage > totalPages)
        {
            endPage = totalPages;
            if (endPage > 5)
            {
                startPage = endPage - 4;
            }
        }

        TotalItems = totalItems;
        CurrentPage = currentPage;
        PageSize = pageSize;
        TotalPages = totalPages;
        StartPage = startPage < 1 ? 1 : startPage;
        EndPage = endPage;
    }
}
