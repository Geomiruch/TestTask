using TestTask.Data;
using TestTask.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TestTask.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext context;
        IWebHostEnvironment _appEnvironment;
        public HomeController(ApplicationDbContext context, IWebHostEnvironment appEnvironment)
        {
            this.context = context;
            _appEnvironment = appEnvironment;
        }

        public bool IsSimilar(AnnouncementModel announcement)
        {
            string[] mass1 = announcement.Title.Split(new char[] { ' ' });
            string[] mass2 = announcement.Description.Split(new char[] { ' ' });
            foreach(string s1 in mass1)
            {
                foreach(string s2 in mass2)
                {
                    if (s1 == s2)
                        return true;
                }
            }
            return false;
        }


        public async Task<IActionResult> Index()
        {
            return View(await context.Announcements.ToListAsync());
        }
        [HttpPost]
        public async Task<IActionResult> Filter()
        {
            var announcements = await context.Announcements.ToListAsync();
            var list = new List<AnnouncementModel> { };
            int count = 0;
            foreach(var a in announcements)
            {
                bool b = IsSimilar(a);
                if(b)
                {
                    list.Add(a);
                    count++;
                }
                if (count == 3)
                    break;
            }
            
            return View("Index", list);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(AnnouncementModel announcement)
        {
            announcement.Date = DateTime.Now.ToString();
            context.Announcements.Add(announcement);
            await context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id != null)
            {
                AnnouncementModel announcement = await context.Announcements.FirstOrDefaultAsync(p => p.Id == id);
                if (announcement != null)
                {
                    return View(announcement);
                }
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Edit(AnnouncementModel announcement)
        {
            announcement.Date = DateTime.Now.ToString();
            context.Announcements.Update(announcement);
            await context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id != null)
            {
                AnnouncementModel announcement = await context.Announcements.FirstOrDefaultAsync(p => p.Id == id);
                if (announcement != null)
                    return View(announcement);
            }
            return NotFound();
        }
        [HttpGet]
        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id != null)
            {
                AnnouncementModel announcement = await context.Announcements.FirstOrDefaultAsync(p => p.Id == id);
                if (announcement != null)
                    return View(announcement);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                AnnouncementModel announcement = await context.Announcements.FirstOrDefaultAsync(p => p.Id == id);
                if (announcement != null)
                {
                    context.Announcements.Remove(announcement);
                    await context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return NotFound();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
