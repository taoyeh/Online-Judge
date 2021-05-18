using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Online_Judge.Models;
using Online_Judge.Models.Auth;
using Online_Judge.Models.Judge;
using Online_Judge.Models.ViewModels.Judge;

namespace Online_Judge.Controllers
{
    public class JudgeController : BaseController
    {
        private readonly MyContext _context;
        public JudgeController(MyContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }



        [HttpGet]
        public IActionResult Submit(long? id)
        {
            if (id != null)
            {
                var problem = _context.Problems.FirstOrDefault(p => p.Id == id);
                if (problem != null)
                {
                    return View(new SubmitViewModel { Id=id.GetValueOrDefault() ,OrderNumber = problem.OrderNumber });
                }
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Submit(SubmitViewModel model)
        {

            var user = HttpContext.Session.Get<UserProfileModel>("CurrentUser");
            if (ModelState.IsValid)
            {
                var problem =  _context.Problems.FirstOrDefault(x => x.Id == model.Id);
                var currentuser = _context.UserProfileModels.FirstOrDefault(x => x.Id==user.Id);
                currentuser.TotalSubmit++;
                _context.UserProfileModels.Update(currentuser);
                var track = new Track
                {
                    AuthorId = user.Id.ToString(),
                    CreateTime = DateTime.Now,
                    ProblemId = model.Id,
                    CodeEncoded = Base64Encode(model.Code),
                    Status = JudgeStatus.Pending,
                    Language = model.Language,
                    SubmitterId=user.Id.ToString()
                };

                if (problem != null)
                {
                    List<Point> points = new List<Point>();

                    var problemDirectory = Path.Combine(Directory.GetCurrentDirectory(), "JudgeDataStorage", problem.Id.ToString(), "data");
                    var subDirectories = Directory.EnumerateDirectories(problemDirectory);
                    for (int i = 0; i < subDirectories.Count(); i++)
                    {
                        points.Add(new Point
                        {
                            Id = i,
                            Status = PointStatus.Pending
                        });
                    }

                    var passRate = problem.GetPassRate();
                    passRate.Submit += 1;
                    problem.SetPassRate(passRate);
                    track.SetPointStatus(points);
                    _context.Problems.Update(problem);

                    _context.Tracks.Add(track);
                    await _context.SaveChangesAsync();

                    var trackNew = await _context.Tracks.FirstOrDefaultAsync(t => t.CreateTime == track.CreateTime  && t.AuthorId== user.Id.ToString());

         

                    return RedirectToAction(nameof(Status), new { trackNew.Id });
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Status(long id)
        {
            var track = await _context.Tracks.FirstOrDefaultAsync(x => x.Id == id);
            if (track != null)
            {
                return View(track);
            }
            return NotFound();
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}
