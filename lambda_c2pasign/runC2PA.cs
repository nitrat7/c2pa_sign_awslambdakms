using System.Diagnostics;

namespace c2panalyze
{

    public class processC2PA
    {

        string _filetoAnalyze = "";

        public processC2PA(string filetoAnalyze)
        {
            _filetoAnalyze = filetoAnalyze;
        }

        public void runSign(string outputFile)
        {
            if (_filetoAnalyze != "")
            {
                Process c2parunner = new Process();

                c2parunner.StartInfo.FileName = Path.Combine(Directory.GetCurrentDirectory(), "c2pa", "c2patool");
                c2parunner.StartInfo.Arguments = _filetoAnalyze + " -m " + Path.Combine(Directory.GetCurrentDirectory(), "certs","manifest.json") + " --signer-path " + Path.Combine(Directory.GetCurrentDirectory(), "c2pa", "kms_signer") + " -o " + outputFile;

                Console.WriteLine("runC2PA " + c2parunner.StartInfo.Arguments);

                c2parunner.StartInfo.CreateNoWindow = true;
                c2parunner.StartInfo.WorkingDirectory = Path.Combine(Directory.GetCurrentDirectory(), "c2pa");
                c2parunner.StartInfo.UseShellExecute = false;
                c2parunner.StartInfo.RedirectStandardError = true;
                c2parunner.StartInfo.RedirectStandardOutput = true;
                c2parunner.Start();

                if (!c2parunner.WaitForExit(60 * 1000))
                {
                    try
                    {
                        Console.WriteLine("runC2PA: 'msg': 'c2patool process timed out'");
                        c2parunner.Kill();

                    }
                    catch { }
                }

                string s_runc2pa_out1 = "";
                string s_runc2pa_err1 = "";

                try
                {
                    s_runc2pa_out1 = c2parunner.StandardOutput.ReadToEnd().Trim();
                    s_runc2pa_err1 = c2parunner.StandardError.ReadToEnd().Trim();
                    c2parunner.WaitForExit();
                }
                catch
                { }

                Console.WriteLine("runC2PA: 'msg': 'c2patool process finished s_runc2pa_out1'" + s_runc2pa_out1);
                Console.WriteLine("runC2PA: 'msg': 'c2patool process finished s_runc2pa_err1'" + s_runc2pa_err1);
                try 
                {
                    Console.WriteLine("runC2PA: 'msg': 'KMS Err " + File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "c2pa", "error_kms.err")));
                }
                catch { }
            }
        }
    }
}
