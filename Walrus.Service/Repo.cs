using System.Collections.Generic;

namespace Walrus.Service
{
    public class Repo
    {
        public string FullPath { get; set; }

        public string Name {  get; set; }

        public string CurrentBranch { get; set; }

        public List<string> LocalBranches { get; set; }

        public List<string> RemoteBranches {  get; set; }
    }
}
