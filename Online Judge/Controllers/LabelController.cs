using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Online_Judge.Models;
using Online_Judge.Models.Auth;
using Online_Judge.Models.Label;
using Online_Judge.Models.ViewModels.Label;

namespace Online_Judge.Controllers
{
    public class LabelController : AdminController
    {

        private readonly MyContext _context;
        public LabelController(MyContext context)
        {
            _context = context;
        }
        // 每页有多少标签 
        public int PageSize = 20;
        public IActionResult Index(int page = 0)
        {
            
            IndexViewModel model = new IndexViewModel
            {
                LabelProfileModels = _context.LabelProfiles.Where(x=>x.IsDelete==false).Skip(page * PageSize).Take(PageSize).ToList(),
                Page = page
            };
            return View(model);
        }

        //增
        [HttpGet]
        public IActionResult New()
        {
            return View();
        }

        [HttpPost]
        public IActionResult New(NewViewModel model)
        {
            LabelProfile label = new LabelProfile();
            label.Introduction = model.Introduction;
            label.Name = model.Name;
            label.IsDelete = false;
            _context.LabelProfiles.Add(label);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


        //删除
        [HttpPost]
        public ActionResult Delete(int LabelId)
        {
            LabelProfile label = _context.LabelProfiles.FirstOrDefault(x => x.Id == LabelId);
            label.IsDelete = true;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        //改
        [HttpGet]
        public IActionResult Change(long? id)
        {
            if(id==null) return RedirectToAction(nameof(Index));
            LabelProfile label = _context.LabelProfiles.FirstOrDefault(x => x.Id == id);
            return View(label);
        }


        [HttpPost]
        public IActionResult Change(LabelProfile labelProfile)
        {
            LabelProfile label = _context.LabelProfiles.FirstOrDefault(x => x.Id == labelProfile.Id);
            label.Introduction = labelProfile.Introduction;
            label.Name = labelProfile.Name;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

    }
}
