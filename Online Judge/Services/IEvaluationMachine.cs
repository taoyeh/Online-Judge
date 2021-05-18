using Online_Judge.Models.Judge;
using Online_Judge.Models.ProblemSet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Online_Judge.Services
{
    public interface IEvaluationMachine
    {
        string CreateSourceFile(string code, Track track);

        PointStatus RunTest(TestData data, string compiledProgramPath, Track track, Problem problem);

        string CompileProgram(Track trackIn, out Track track);
    }
}
