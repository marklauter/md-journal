using System.Diagnostics;

namespace MD.Journal
{
    public sealed class Git
    {
        private const string GitCommand = "git.exe";
        private readonly string path;

        public Git(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException($"'{nameof(path)}' cannot be null or whitespace.", nameof(path));
            }

            // todo: verify that path points to a git repo
            this.path = path;
        }

        public void Stage()
        {
            this.RunCommand("add .");
        }

        public void Commit()
        {
            this.RunCommand($"commit -m \"new journal entry\"");
        }

        public void Push()
        {
            this.RunCommand("push");
        }

        //, [CallerMemberName]string caller = ""
        private void RunCommand(string args)
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = GitCommand,
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    CreateNoWindow = false,
                    WindowStyle = ProcessWindowStyle.Normal,
                    WorkingDirectory = path,
                }
            };

            _ = process.Start();

            //while (!process.StandardOutput.EndOfStream)
            //{
            //    var message = process.StandardOutput.ReadLine();
            //    this.logger.LogInformation("{Caller}/{StdOut}", caller, message);
            //}

            //while (!process.StandardError.EndOfStream)
            //{
            //    var message = process.StandardError.ReadLine();
            //    this.logger.LogError("{StdErr}", message);
            //}
        }
    }
}
