using CMS2.Data;
using CMS2.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;

public class EmailHistoryModel : PageModel
{
    private readonly AppDbContext _context;

    public EmailHistoryModel(AppDbContext context)
    {
        _context = context;
    }

    public Dictionary<DateTime, List<EmailLog>> GroupedLogsByDate { get; set; }

    public void OnGet()
    {
        GroupedLogsByDate = _context.EmailLogs
            .OrderByDescending(e => e.SentAt)
            .AsEnumerable()
            .GroupBy(e => e.SentAt.Date)
            .ToDictionary(g => g.Key, g => g.ToList());
    }
}
