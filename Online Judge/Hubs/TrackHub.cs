using Microsoft.EntityFrameworkCore;
using Online_Judge.Models;
using Online_Judge.Models.Judge;
using Online_Judge.Models.ProblemSet;
using Online_Judge.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using System;
using Online_Judge.Models.Auth;
using Microsoft.AspNetCore.Http;

namespace Online_Judge.Hubs
{
    public class TrackHub : Hub
    {
        private readonly MyContext _context;

        private readonly IEvaluationMachine _evaluationMachine;
        public TrackHub(MyContext context, IEvaluationMachine evaluationMachine)
        {
            _context = context;
            _evaluationMachine = evaluationMachine;
        }
        public async Task GetTrack(string trackId)
        {
            var track = await _context.Tracks.FirstOrDefaultAsync(t => t.Id == long.Parse(trackId));
            await Clients.Caller.SendAsync("updateStatus", Base64Encode(JsonSerializer.Serialize(track)));

            //设置初试状态
            if (track.Status != JudgeStatus.Pending)
            {
                await Clients.Caller.SendAsync("updateStatus", Base64Encode(JsonSerializer.Serialize(track)));
                await Clients.Caller.SendAsync("finish");
                return;
            }

            //找题目
            var problem = await _context.Problems.FirstOrDefaultAsync(q => q.Id == track.ProblemId);
            if (problem != null)
            {


                // 将测试数据加载到内存
                var testdata = new List<TestData>();
                var problemDirectory = Path.Combine(Directory.GetCurrentDirectory(), "JudgeDataStorage", problem.Id.ToString(), "data");
                Directory.GetDirectories(problemDirectory).ToList().ForEach(async element =>
                {
                    var currentPath = Path.Combine(problemDirectory, element);

                    TestData data = new TestData
                    {
                        Input = await File.ReadAllTextAsync(Path.Combine(currentPath, "data.in")),
                        Output = await File.ReadAllTextAsync(Path.Combine(currentPath, "data.out"))
                    };
                    testdata.Add(data);
                });

                // 编译源代码
                string sourceFilePath = _evaluationMachine.CreateSourceFile(track.CodeEncoded, track);
                string programPath = _evaluationMachine.CompileProgram(track, out Track trackOut);
                track = trackOut;

                // 将编译日志推送到客户端
                await Clients.Caller.SendAsync("updateStatus", Base64Encode(JsonSerializer.Serialize(track)));

                // 如果编译失败，可执行文件将不存在
                if (!File.Exists(programPath))
                {
                    track.Status = JudgeStatus.CompileError;
                    var pointStatus = track.GetPointStatus();
                    for (int i = 0; i < pointStatus.Count; i++)
                    {
                        pointStatus[i].Status = PointStatus.InternalError;
                    }
                    track.SetPointStatus(pointStatus);
                    _context.Tracks.Update(track);
                    await _context.SaveChangesAsync();
                    await Clients.Caller.SendAsync("updateStatus", Base64Encode(JsonSerializer.Serialize(track)));
                    await Clients.Caller.SendAsync("finish");
                    return;
                }

                var status = track.GetPointStatus();
                status.ForEach(element =>
                {
                    element.Status = PointStatus.Judging;
                });

                track.SetPointStatus(status);

                await Clients.Caller.SendAsync("updateStatus", Base64Encode(JsonSerializer.Serialize(track)));

                var tasks = new List<Task<PointStatus>>();
                for (int i = 0; i < testdata.Count; i++)
                {
                    var task = new Task<PointStatus>(() =>
                    {
                        var section = i - 1;
                        var data = testdata[section];
                        var result = _evaluationMachine.RunTest(data, programPath, track, problem);

                        return result;

                    });

                    task.Start();
                    tasks.Add(task);
                }

                var result = await Task.WhenAll(tasks);
                for (int i = 0; i < result.Length; i++)
                {
                    status[i].Status = result[i];
                }

                track.SetPointStatus(status);

                if (track.GetPointStatus().FirstOrDefault(x => x.Status != PointStatus.Accepted) != null)
                {
                    track.Status = JudgeStatus.WrongAnswer;
                }
                else
                {
                    track.Status = JudgeStatus.Accept;
                    var passRate = problem.GetPassRate();
                    passRate.Pass += 1;
                    problem.SetPassRate(passRate);
                    _context.Update(problem);

                    //通过题目之后要给用户加标签
                    var user = await _context.UserProfileModels.FirstOrDefaultAsync(q => q.Id == long.Parse(track.SubmitterId) );
                    user.TotalAccepted++;
                    _context.UserProfileModels.Update(user);
                    foreach (var ProblemLabelitem in _context.ProblemLabels.Where(x => x.ProblemId == problem.Id.ToString()).ToList())
                    {
                        var userLabel = await _context.UserLabels.FirstOrDefaultAsync(x => x.LabelId == ProblemLabelitem.LabelId);
                        if(userLabel == null)
                        {
                            UserLabel userlabel = new UserLabel
                            {
                                UserId = user.Id.ToString(),
                                LabelId = ProblemLabelitem.LabelId,
                                Weight = 1
                            };
                            _context.UserLabels.Add(userlabel);
                        }
                        else
                        {
                            userLabel.Weight++;
                            _context.UserLabels.Update(userLabel);
                        }
                    }



                }

                _context.Tracks.Update(track);
                await _context.SaveChangesAsync();

                await Clients.Caller.SendAsync("updateStatus", Base64Encode(JsonSerializer.Serialize(track)));

            }

            await Clients.Caller.SendAsync("finish");
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }

}
