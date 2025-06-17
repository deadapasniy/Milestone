using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using Milestone.Data;
using Milestone.Models;

namespace Milestone.Controllers
{
    [Authorize(Roles = "Leader,Member")]
    public class ProjectTaskController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectTaskController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Task
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ProjectTasks.Include(p => p.AssignedUser).Include(p => p.Project);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Task/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectTask = await _context.ProjectTasks
                .Include(p => p.AssignedUser)
                .Include(p => p.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (projectTask == null)
            {
                return NotFound();
            }

            return View(projectTask);
        }

        // GET: Task/Create
        public async Task<IActionResult> Create()
        {
            var members = await _userManager.GetUsersInRoleAsync("Member");

            ViewData["AssignedUserId"] = new SelectList(members, "Id", "UserName");

            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "OwnerId");
            return View();
        }

        // POST: Task/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Deadline,Priority,IsCompleted,ProjectId,AssignedUserId")] ProjectTask projectTask)
        {
            if (ModelState.IsValid)
            {
                _context.Add(projectTask);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            var members = await _userManager.GetUsersInRoleAsync("Member");

            ViewData["AssignedUserId"] = new SelectList(members, "Id", "UserName", projectTask.AssignedUserId);

            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "OwnerId", projectTask?.ProjectId);
            return View(projectTask);
        }

        // GET: Task/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectTask = await _context.ProjectTasks.FindAsync(id);
            if (projectTask == null)
            {
                return NotFound();
            }

            var members = await _userManager.GetUsersInRoleAsync("Member");
            ViewData["AssignedUserId"] = new SelectList(members, "Id", "UserName", projectTask.AssignedUserId);

            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "OwnerId", projectTask?.ProjectId);
            return View(projectTask);
        }

        // POST: Task/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Deadline,Priority,IsCompleted,ProjectId,AssignedUserId")] ProjectTask projectTask)
        {
            if (id != projectTask.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(projectTask);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectTaskExists(projectTask.Id))
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
            var members = await _userManager.GetUsersInRoleAsync("Member");
            ViewData["AssignedUserId"] = new SelectList(members, "Id", "UserName", projectTask.AssignedUserId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "OwnerId", projectTask?.ProjectId);
            return View(projectTask);
        }

        // GET: Task/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectTask = await _context.ProjectTasks
                .Include(p => p.AssignedUser)
                .Include(p => p.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (projectTask == null)
            {
                return NotFound();
            }

            return View(projectTask);
        }

        // POST: Task/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var projectTask = await _context.ProjectTasks.FindAsync(id);
            if (projectTask != null)
            {
                _context.ProjectTasks.Remove(projectTask);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectTaskExists(int id)
        {
            return _context.ProjectTasks.Any(e => e.Id == id);
        }
    }
}
