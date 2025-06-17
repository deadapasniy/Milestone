using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Milestone.Data;
using Milestone.Models;

namespace Milestone.Controllers
{
    [Authorize(Roles = "Member")]
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReportController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Report
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Reports.Include(r => r.SubmittedBy).Include(r => r.Task);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Report/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var report = await _context.Reports
                .Include(r => r.SubmittedBy)
                .Include(r => r.Task)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (report == null)
            {
                return NotFound();
            }

            return View(report);
        }

        // GET: Report/Create
        public IActionResult Create()
        {
            ViewData["TaskId"] = new SelectList(_context.ProjectTasks, "Id", "Title");

            var userId = _userManager.GetUserId(User);
            ViewData["SubmittedById"] = new SelectList(new[] {
                new { Id = userId, Email = _userManager.GetUserName(User) }
            }, "Id", "Email", userId);

            return View();
        }

        // POST: Report/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Content,DateSubmitted,TaskId,SubmittedById")] Report report)
        {
            report.DateSubmitted = DateTime.Now;

            if (!ModelState.IsValid)
            {
                // Перевір помилки валідації
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    // Логуй або дивись помилки
                    Console.WriteLine(error.ErrorMessage);
                }
            }


            if (ModelState.IsValid)
            {
                _context.Add(report);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TaskId"] = new SelectList(_context.ProjectTasks, "Id", "Title", report.TaskId);
            var userId = _userManager.GetUserId(User);
            ViewData["SubmittedById"] = new SelectList(new[] {
                new { Id = userId, Email = _userManager.GetUserName(User) }
            }, "Id", "Email", userId);
            return View(report);
        }

        // GET: Report/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var report = await _context.Reports.FindAsync(id);
            if (report == null)
            {
                return NotFound();
            }
            ViewData["SubmittedById"] = new SelectList(_context.Users, "Id", "Id", report.SubmittedById);
            ViewData["TaskId"] = new SelectList(_context.ProjectTasks, "Id", "Title", report.TaskId);
            return View(report);
        }

        // POST: Report/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Content,DateSubmitted,TaskId,SubmittedById")] Report report)
        {
            if (id != report.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(report);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReportExists(report.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["SubmittedById"] = new SelectList(_context.Users, "Id", "Id", report.SubmittedById);
            ViewData["TaskId"] = new SelectList(_context.ProjectTasks, "Id", "Title", report.TaskId);
            return View(report);
        }

        // GET: Report/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var report = await _context.Reports
                .Include(r => r.SubmittedBy)
                .Include(r => r.Task)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (report == null)
            {
                return NotFound();
            }

            return View(report);
        }

        // POST: Report/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var report = await _context.Reports.FindAsync(id);
            if (report != null)
            {
                _context.Reports.Remove(report);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReportExists(int id)
        {
            return _context.Reports.Any(e => e.Id == id);
        }
    }
}
