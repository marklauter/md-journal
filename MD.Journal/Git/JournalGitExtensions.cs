using System.Text;

namespace MD.Journal.Git
{
    public static class JournalGitExtensions
    {
        public static (string stdout, string stderr) Commit(this Journals.Journal journal)
        {
            var git = new GitProcess(journal.Path);

            var stage = git.Stage();
            var commit = git.Commit();
            var push = git.Push();

            var stdout = new StringBuilder()
                .Append(stage.stdout)
                .Append(commit.stdout)
                .Append(push.stdout);

            var stderr = new StringBuilder()
                .Append(stage.stderr)
                .Append(commit.stderr)
                .Append(push.stderr);

            return (stdout.ToString(), stderr.ToString());
        }
    }
}


