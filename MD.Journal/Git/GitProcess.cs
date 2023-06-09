﻿using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace MD.Journal.Git
{
    internal sealed class GitProcess
    {
        private const string GitCommand = "git.exe";
        private readonly string path;

        public GitProcess(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException($"'{nameof(path)}' cannot be null or whitespace.", nameof(path));
            }

            if (!IsGitRepository(path))
            {
                throw new InvalidOperationException("path is not a git repository");
            }

            this.path = path;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (string stdout, string stderr) Stage()
        {
            return RunCommand("add .", this.path);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (string stdout, string stderr) Commit()
        {
            return RunCommand($"commit -m \"doc: new journal entry\"", this.path);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (string stdout, string stderr) Push()
        {
            return RunCommand("push", this.path);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsGitRepository(string path)
        {
            var (stdout, stderr) = RunCommand("rev-parse --git-dir", path);
            return !stdout.Contains("fatal") && !stderr.Contains("fatal");
        }

        private static (string stdout, string stderr) RunCommand(string args, string path)
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = GitCommand,
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Normal,
                    WorkingDirectory = path,
                }
            };

            _ = process.Start();

            var stdout = new StringBuilder();
            while (!process.StandardOutput.EndOfStream)
            {
                var message = process.StandardOutput.ReadLine();
                _ = stdout.AppendLine(message);
                //this.logger.LogInformation("{Caller}/{StdOut}", caller, message);
            }

            var stderr = new StringBuilder();
            while (!process.StandardError.EndOfStream)
            {
                var message = process.StandardError.ReadLine();
                _ = stderr.AppendLine(message);
                //this.logger.LogError("{StdErr}", message);
            }

            return (stdout.ToString(), stderr.ToString());
        }
    }
}
