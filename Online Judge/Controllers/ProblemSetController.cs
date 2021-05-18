using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.AspNetCore.Mvc;
using Online_Judge.Models;
using Online_Judge.Models.Auth;
using Online_Judge.Models.Label;
using Online_Judge.Models.ProblemSet;
using Online_Judge.Models.ViewModels.Problemset;

namespace Online_Judge.Controllers
{
    public class ProblemSetController : Controller
    {
        private readonly MyContext _context;
        public ProblemSetController(MyContext context)
        {
            _context = context;
        }
        // 题目每页多少题
        public int PageSize = 20;
        /// <summary>
        /// 展示所有的题目
        /// </summary>
        /// <returns></returns>
        public IActionResult Index(int page=0)
        {
            var user = HttpContext.Session.Get<UserProfileModel>("CurrentUser");
            // 将题目和他的标签查询出来
            List<ProblemAndHisLabel> problemAndHisLabels = new List<ProblemAndHisLabel>();
            
            foreach(var item in _context.Problems.Where(x=>x.IsDelete==false).Skip(page * PageSize).Take(PageSize).ToList())
            {
                ProblemAndHisLabel problemAndHisLabel = new ProblemAndHisLabel();
                problemAndHisLabel.problem = item;

                if (user != null)
                {
                    var issolve = _context.Tracks.FirstOrDefault(x => x.ProblemId == item.Id && x.SubmitterId == user.Id.ToString());
                    if (issolve == null) problemAndHisLabel.IsSolved = false;
                    else problemAndHisLabel.IsSolved = true;
                }
                else problemAndHisLabel.IsSolved = false;


                // 去查每一个题目的标签
                List <LabelProfile> labelProfiles = new List<LabelProfile>();
                foreach (var ProblemLabelitem in _context.ProblemLabels.Where(x => x.ProblemId == item.Id.ToString()).ToList())  
                {
                    LabelProfile labelProfile =_context.LabelProfiles.FirstOrDefault(x => x.Id ==  long.Parse(ProblemLabelitem.LabelId) ) ;
                    labelProfiles.Add(labelProfile);
                }
                problemAndHisLabel.labelProfiles= labelProfiles;
                problemAndHisLabels.Add(problemAndHisLabel);
            }

            //获得所有标签
            List<LabelProfile> AllLables = _context.LabelProfiles.ToList();


            IndexViewModel model = new IndexViewModel
            {
                ProblemModels = problemAndHisLabels,
                AllLables= AllLables,
                Page = page
            };
            return View(model);
        }

        public IActionResult Recommend(int page = 0)
        {
            var user = HttpContext.Session.Get<UserProfileModel>("CurrentUser");
            if(user==null)
            {
               TempData["Message"] = "请先登录";
               return RedirectToAction("Login", "Auth");
               
            }
            List<RecommendProblem> recommendProblems = new List<RecommendProblem>();
            foreach (var item in _context.Problems.Where(x => x.IsDelete == false).ToList())
            {
                int weight = 0;
                foreach(var problemlabel in _context.ProblemLabels.Where(x=>x.ProblemId== item.Id.ToString()).ToList())
                {
                    foreach(var userlabel in _context.UserLabels.Where(x=>x.UserId== user.Id.ToString()).ToList() )
                    {
                        if (userlabel.LabelId == problemlabel.LabelId) weight += userlabel.Weight * problemlabel.Weight;
                    }
                }
                if (_context.Tracks.FirstOrDefault(x => x.ProblemId == item.Id && x.Status == Models.Judge.JudgeStatus.Accept) != null)
                {
                    weight = -1;
                }
                RecommendProblem recommendProblem = new RecommendProblem
                { 
                   problem=item,
                   Weight=weight
                };
                recommendProblems.Add(recommendProblem);
            }
            recommendProblems.Sort((l, r) =>r.Weight.CompareTo(l.Weight));

            List<ProblemAndHisLabel> problemAndHisLabels = new List<ProblemAndHisLabel>();

            foreach (var item in recommendProblems.Skip(page * PageSize).Take(PageSize).ToList())
            {
                ProblemAndHisLabel problemAndHisLabel = new ProblemAndHisLabel();
                problemAndHisLabel.problem = item.problem;

                if (user != null)
                {
                    var issolve = _context.Tracks.FirstOrDefault(x => x.ProblemId == item.problem.Id && x.SubmitterId == user.Id.ToString());
                    if (issolve == null) problemAndHisLabel.IsSolved = false;
                    else problemAndHisLabel.IsSolved = true;
                }
                else problemAndHisLabel.IsSolved = false;


                // 去查每一个题目的标签
                List<LabelProfile> labelProfiles = new List<LabelProfile>();
                foreach (var ProblemLabelitem in _context.ProblemLabels.Where(x => x.ProblemId == item.problem.Id.ToString()).ToList())
                {
                    LabelProfile labelProfile = _context.LabelProfiles.FirstOrDefault(x => x.Id == long.Parse(ProblemLabelitem.LabelId));
                    labelProfiles.Add(labelProfile);
                }
                problemAndHisLabel.labelProfiles = labelProfiles;
                problemAndHisLabels.Add(problemAndHisLabel);
            }

            //获得所有标签
            List<LabelProfile> AllLables = _context.LabelProfiles.ToList();
            IndexViewModel model = new IndexViewModel
            {
                ProblemModels = problemAndHisLabels,
                AllLables = AllLables,
                Page = page
            };
            return View(model);
        }


        [HttpGet]
        public IActionResult New()
        {
            var user = HttpContext.Session.Get<UserProfileModel>("CurrentUser");
            if (user == null)
            {
                TempData["Message"] = "请先登录";
                return RedirectToAction("Login", "Auth");

            }
            NewViewModel model = new NewViewModel();
            model.Labels =_context.LabelProfiles.ToList();
            return View(model);
        }

        /// <summary>
        /// 新增题目
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]

        public IActionResult New(NewViewModel model,int[] ProblemLabels, int[] RecommendedValue)
        {

            var user = HttpContext.Session.Get<UserProfileModel>("CurrentUser");
            if (user == null)
            {
                TempData["Message"] = "请先登录";
                return RedirectToAction("Login", "Auth");

            }
            //创建新的Problem
            var problem = new Problem
            {
                OrderNumber =  (1000 + _context.Problems.LongCount()).ToString() ,
                Title = model.Title,
                Description = model.Description,
                AuthorId = user.Id.ToString(),
                PublishTime = DateTime.UtcNow,
                IsDelete=false
            };

            var exampleData = new TestData
            {
                Input = model.ExampleData.Input,
                Output = model.ExampleData.Output
            };

            problem.SetExampleData(exampleData);

            var judgeProfile = new JudgeProfile
            {
                MemoryLimit = model.MemoryLimit,
                TimeLimit = model.TimeLimit,
            };

            problem.SetJudgeProfile(judgeProfile);

            problem.SetPassRate(new PassRate
            {
                Submit = 0,
                Pass = 0
            });

            _context.Problems.Add(problem);
            _context.SaveChanges();


            //增加题目标签

            long ProblemId = _context.Problems.OrderByDescending(x => x.Id).FirstOrDefault().Id;

            int count = 0;
            foreach (var item in _context.LabelProfiles.ToList())
            {
                if (ProblemLabels.Length == count) break;
                if(ProblemLabels[count]==item.Id)
                {
                    ProblemLabel problemLabel = new ProblemLabel();
                    problemLabel.LabelId = item.Id.ToString();
                    problemLabel.ProblemId = ProblemId.ToString();
                    problemLabel.Weight = RecommendedValue[count];
                    _context.ProblemLabels.Add(problemLabel);
                    _context.SaveChanges();
                    count++;
                }
            }



            //创建文件路径
            var judgeDataStorageDirectory = Path.Combine(Directory.GetCurrentDirectory(), "JudgeDataStorage");
            if (!Directory.Exists(judgeDataStorageDirectory))
            {
                Directory.CreateDirectory(judgeDataStorageDirectory);
            }

            //复制zip到指定路径
            var problemDirectory = Path.Combine(judgeDataStorageDirectory, _context.Problems.LongCount().ToString());
            Directory.CreateDirectory(problemDirectory);
            var zipFilePath = Path.Combine(problemDirectory, "judge.zip");

            var stream = new FileStream(zipFilePath, FileMode.Create);
            model.TestDatas.CopyTo(stream);
            stream.Close();

            //解压zip
            var targetDirectory = Path.Combine(problemDirectory, "data");
            var fastZip = new FastZip();
            fastZip.ExtractZip(zipFilePath, targetDirectory, null);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Detail(long? id)
        {
            var problem = _context.Problems.FirstOrDefault(p => p.Id == id);

            return View(problem);
        }

        public IActionResult ChangeProblem(int page = 0)
        {

            var user = HttpContext.Session.Get<UserProfileModel>("CurrentUser");
            if (user == null)
            {
                TempData["Message"] = "请先登录";
                return RedirectToAction("Login", "Auth");

            }
            // 将题目和他的标签查询出来
            List<ProblemAndHisLabel> problemAndHisLabels = new List<ProblemAndHisLabel>();

            foreach (var item in _context.Problems.Where(x => x.IsDelete == false).Skip(page * PageSize).Take(PageSize).ToList())
            {
                ProblemAndHisLabel problemAndHisLabel = new ProblemAndHisLabel();
                problemAndHisLabel.problem = item;
                // 去查每一个题目的标签
                List<LabelProfile> labelProfiles = new List<LabelProfile>();
                foreach (var ProblemLabelitem in _context.ProblemLabels.Where(x => x.ProblemId == item.Id.ToString()).ToList())
                {
                    LabelProfile labelProfile = _context.LabelProfiles.FirstOrDefault(x => x.Id == long.Parse(ProblemLabelitem.LabelId));
                    labelProfiles.Add(labelProfile);
                }
                problemAndHisLabel.labelProfiles = labelProfiles;
                problemAndHisLabels.Add(problemAndHisLabel);
            }

            //获得所有标签
            List<LabelProfile> AllLables = _context.LabelProfiles.ToList();


            IndexViewModel model = new IndexViewModel
            {
                ProblemModels = problemAndHisLabels,
                AllLables = AllLables,
                Page = page
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult Delete(int ProblemId)
        {
            Problem problem = _context.Problems.FirstOrDefault(x => x.Id == ProblemId);
            problem.IsDelete = true;
            _context.SaveChanges();
            return RedirectToAction(nameof(ChangeProblem));
        }

        public IActionResult Records(int page = 0)
        {
            var model = new List<RecordsModel>();
            var tracks = _context.Tracks.ToList();
            foreach (var track in tracks)
            {
                var user = _context.UserProfileModels.FirstOrDefault(x => x.Id == long.Parse(track.AuthorId));
                model.Add(new RecordsModel(track, user.UserName));
            }

            return View(model);
        }
       

    }
}
