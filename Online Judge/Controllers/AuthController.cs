using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Online_Judge.Models;
using Online_Judge.Models.Auth;
using Online_Judge.Models.Label;
using Online_Judge.Models.ViewModels.Auth;
using Online_Judge.Utility;

namespace Online_Judge.Controllers
{
    public class AuthController : Controller
    {

        private readonly MyContext _context;
        public AuthController(MyContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// 登录页面
        /// </summary>
        /// <param name="loginModel"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Login(LoginModel loginModel)
        {
            UserProfileModel CurrentUser = _context.UserProfileModels.FirstOrDefault(x => x.Email == loginModel.Email);
            if(CurrentUser==null)
            {
                TempData["Message"] = "查无账号，请先注册";
                return RedirectToAction("Login", "Auth");
            }
            if (CurrentUser.Password==loginModel.Password)
            {
                HttpContext.Session.Set("CurrentUser", CurrentUser);
                //var user = HttpContext.Session.Get<UserProfileModel>("CurrentUser");

                CurrentUser.LastLoginTime = DateTime.Now;
                _context.UserProfileModels.Update(CurrentUser);
                _context.SaveChanges();

                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("CurrentUser");
            return RedirectToAction("Index", "Home");
        }
        public string Islogin()
        {
            var use = HttpContext.Session.Get<UserProfileModel>("CurrentUser");
            if (use == null) return null;
            else return use.UserName;
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterModel registerModel)
        {
            var userProfileModel = new UserProfileModel
            {
                Email = registerModel.Email,
                Password = registerModel.Password,
                CreateTime = DateTime.UtcNow,
                UserName = registerModel.UserName,
                Role = 1,
                TotalSubmit = 0,
                TotalAccepted = 0
            };
            _context.UserProfileModels.Add(userProfileModel);
            _context.SaveChanges();
            return RedirectToAction("Login", "Auth");
        }

        /// <summary>
        /// 个人信息界面
        /// </summary>
        /// <returns></returns>
        public IActionResult My()
        {
            var user = HttpContext.Session.Get<UserProfileModel>("CurrentUser");
            if (user == null)
            {
                TempData["Message"] = "请先登录";
                return RedirectToAction("Login", "Auth");

            }
            // 已经解决的题目
            List<string> havesoledproblem = new List<string>();
            foreach (var item in _context.Tracks.Where(x=>x.SubmitterId==user.Id.ToString() && x.Status== Models.Judge.JudgeStatus.Accept).ToList())
            {
                var problemnumber = _context.Problems.FirstOrDefault(x => x.Id == item.ProblemId).OrderNumber;
                if (havesoledproblem.Find(x => x.Contains(problemnumber)) == null)
                    havesoledproblem.Add(problemnumber);
                else continue;
            }

            // 用户拥有的标签和未拥有的标签
            List<LabelProfile> havelabel = new List<LabelProfile>();
            foreach (var item in _context.UserLabels.Where(x=>x.UserId==user.Id.ToString() && x.Weight!=0).ToList())
            {
                var userlabel = _context.LabelProfiles.FirstOrDefault(x => x.Id == long.Parse(item.LabelId));
                havelabel.Add(userlabel);
            }
            List<LabelProfile> nothavelabel = new List<LabelProfile>();
            foreach (var item in _context.LabelProfiles.ToList())
            {
                if (havelabel.Find(x => x == item) == null) nothavelabel.Add(item);
                else continue;
            }

            var model = new MyIndexModel
            {
                User = user,
                HaveSolvedProblem = havesoledproblem,
                HaveLabel= havelabel,
                NotHaveLabel= nothavelabel
            };
            return View(model);
        }
        
        [HttpPost]
        public JsonResult ChartData()
        {
            var user = HttpContext.Session.Get<UserProfileModel>("CurrentUser");
            Dictionary<string, object> res = new Dictionary<string, object>();
            foreach( var userLabels in _context.UserLabels.Where(x=>x.UserId== user.Id.ToString()) )
            {
                res.Add(_context.LabelProfiles.FirstOrDefault(x => x.Id == long.Parse(userLabels.LabelId)).Name, userLabels.Weight);
            }
            return Json (res);
        }

        [HttpPost]
        public ActionResult DeleteLabel(int LabelId)
        {
            var user = HttpContext.Session.Get<UserProfileModel>("CurrentUser");
            UserLabel userlabel = _context.UserLabels.FirstOrDefault(x=>x.LabelId==LabelId.ToString() && x.UserId== user.Id.ToString()) ;
            userlabel.Weight = 0;
            _context.SaveChanges();
            return RedirectToAction(nameof(My));
        }

        [HttpPost]
        public ActionResult AddLabel(int LabelId)
        {
            var user = HttpContext.Session.Get<UserProfileModel>("CurrentUser");
            UserLabel userlabel = _context.UserLabels.FirstOrDefault(x => x.LabelId == LabelId.ToString() && x.UserId == user.Id.ToString());
            if (userlabel == null)
            {
                UserLabel newuserLabel = new UserLabel
                {
                    LabelId = LabelId.ToString() ,
                    UserId =  user.Id.ToString(),
                    Weight = 1
                };
                _context.UserLabels.Add(newuserLabel);

            }
            else userlabel.Weight = 1;
            _context.SaveChanges();
            return RedirectToAction(nameof(My));
        }



    }
    



    /// <summary>
    /// 添加Session用法,Set是存，Get取
    /// </summary>
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) :
                                  JsonConvert.DeserializeObject<T>(value);
        }
    }
   
}
