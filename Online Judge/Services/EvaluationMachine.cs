using Online_Judge.Models.Judge;
using Online_Judge.Models.ProblemSet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Online_Judge.Services
{
    public class EvaluationMachine : IEvaluationMachine
    {
        public string CreateSourceFile(string code, Track track)
        {
            var path = Directory.GetCurrentDirectory();
            path = Path.Combine(path, "EvaluationMachine");

            var programSourceFilePath = Path.Combine(path, track.Id.ToString());
            if (!Directory.Exists(programSourceFilePath))
            {
                Directory.CreateDirectory(programSourceFilePath);
            }

            programSourceFilePath = Path.Combine(programSourceFilePath, "source.c");
            if (!File.Exists(programSourceFilePath))
            {
                File.Create(programSourceFilePath).Close();
            }
            else
            {
                File.Delete(programSourceFilePath);
                File.Create(programSourceFilePath).Close();
            }

            using (var tw = new StreamWriter(programSourceFilePath, true))
            {
                tw.WriteLine(Base64Decode(code));
                tw.Close();
            }

            return programSourceFilePath;
        }

        public string CompileProgram(Track trackIn, out Track track)
        {
            track = trackIn;

            var path = Directory.GetCurrentDirectory();
            path = Path.Combine(path, "EvaluationMachine");
            path = Path.Combine(path, track.Id.ToString());
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            try
            {
                using Process compilerProcess = new Process();
                compilerProcess.StartInfo.UseShellExecute = false;
                compilerProcess.StartInfo.CreateNoWindow = true;
                compilerProcess.StartInfo.RedirectStandardInput = true;
                compilerProcess.StartInfo.RedirectStandardOutput = true;
                compilerProcess.StartInfo.WorkingDirectory = path;
                compilerProcess.StartInfo.FileName = "cmd.exe";

              

                compilerProcess.Start();

                if (track.Language == Models.Judge.SupportProgrammingLanguage.C)
                {
                    compilerProcess.StandardInput.WriteLine("gcc source.c");
                }
                else if (track.Language == Models.Judge.SupportProgrammingLanguage.Cpp)
                {
                    compilerProcess.StandardInput.WriteLine("g++ source.c");
                }

                compilerProcess.StandardInput.WriteLine("exit");
                string compileOutput = compilerProcess.StandardOutput.ReadToEnd();
                compilerProcess.WaitForExit();


            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Path.Combine(path, "a.exe");

        }

        public PointStatus RunTest(TestData data, string compiledProgramPath, Track track, Problem problem)
        {
            var path = Directory.GetCurrentDirectory();
            path = Path.Combine(path, "EvaluationMachine");
            path = Path.Combine(path, track.Id.ToString());
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            try
            {
                using Process programProcess = new Process();
                programProcess.StartInfo.UseShellExecute = false;
                programProcess.StartInfo.CreateNoWindow = true;
                programProcess.StartInfo.RedirectStandardInput = true;
                programProcess.StartInfo.RedirectStandardOutput = true;
                programProcess.StartInfo.WorkingDirectory = path;
                programProcess.StartInfo.FileName = compiledProgramPath;
                programProcess.Start();
                programProcess.StandardInput.WriteLine(data.Input);
                string programOutput = programProcess.StandardOutput.ReadToEnd();
                Thread.Sleep(Convert.ToInt32(problem.GetJudgeProfile().TimeLimit * 1000) + 1000);

                if (!programProcess.HasExited)
                {
                    programProcess.Kill();
                }
                programProcess.WaitForExit();

                var runningTime = ((programProcess.ExitTime - programProcess.StartTime).TotalMilliseconds) / 1000;
                if (runningTime > problem.GetJudgeProfile().TimeLimit)
                {
                    programProcess.Kill();
                    return PointStatus.TimeLimitExceeded;
                }

                if (data.Output.Trim().TrimEnd('\n').Trim().Replace("\r", "").Equals(programOutput.Trim().TrimEnd('\n').Trim().Replace("\r", "")))
                {
                    return PointStatus.Accepted;
                }
                else
                {
                    return PointStatus.WrongAnswer;
                }
            }
            catch
            {
                return PointStatus.InternalError;
            }
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
