using System.Linq;
using System;
using System.Collections.Generic;
using DoTuna.Util;

namespace DoTuna.Core
{
    public class ThreadFile : IEquatable<ThreadFile>
    {
        public ulong ThreadId { get; }
        public string[] FileNameSegments { get; }
        public string FileName => string.Join("/", FileNameSegments);
        public string PathName => string.Join("/", FileNameSegments.SkipLastOne());

        public ThreadFile(JsonIndexDocument doc, string pattern)
        {
            ThreadId = doc.threadId;
            FileNameSegments = (GetTemplateName(doc, pattern) + ".html").Split('/');
        }

        public ThreadFile(ulong threadId, string[] fileNameSegments)
        {
            ThreadId = threadId;
            FileNameSegments = fileNameSegments ?? throw new ArgumentNullException(nameof(fileNameSegments));
        }

        public string GetRelativePathTo(ThreadFile target)
        {
            var from = this.FileNameSegments;
            var to = target.FileNameSegments;
            if (from == to) return target.FileName;
            int len = Math.Min(from.Length, to.Length);
            int common = 0;
            while (common < len && from[common] == to[common]) common++;
            var upCount = Math.Max(0, from.Length - common - 1);
            var up = Enumerable.Repeat("..", upCount);
            var down = to.Skip(common);
            var rel = up.Concat(down);
            return string.Join("/", rel);
        }

        public string GetRelativePathToRoot()
        {
            if (FileNameSegments.Length <= 1) return "";
            return string.Join("/", Enumerable.Repeat("..", FileNameSegments.Length - 1));
        }

        public static string GetTemplateName(JsonIndexDocument doc, string template)
        {
            if (string.IsNullOrEmpty(template)) return string.Empty;
            var values = new Dictionary<string, string>
            {
                { "id", doc.threadId.ToString() },
                { "title", doc.title },
                { "name", doc.username },
                { "created", doc.createdAt.ToString("yyyy-MM-dd") },
                { "updated", doc.updatedAt.ToString("yyyy-MM-dd") },
                { "size", doc.size.ToString() }
            };

            return TemplateFormatter
                .Format(template, values)
                .ReplaceInvalidFileNameChars()
                .Truncate(200);
        }






        public override bool Equals(object obj)
        {
            if (obj is ThreadFile o)
            {
                return Equals(o);
            }
            else return false;
        }

        public bool Equals(ThreadFile other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (ThreadId != other.ThreadId) return false;
            if (FileNameSegments.Length != other.FileNameSegments.Length) return false;
            for (int i = 0; i < FileNameSegments.Length; i++)
            {
                if (!string.Equals(FileNameSegments[i], other.FileNameSegments[i], StringComparison.Ordinal))
                    return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = ThreadId.GetHashCode();
                foreach (var segment in FileNameSegments)
                {
                    hash = (hash * 397) ^ (segment != null ? segment.GetHashCode() : 0);
                }
                return hash;
            }
        }

        public static bool operator ==(ThreadFile left, ThreadFile right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        public static bool operator !=(ThreadFile left, ThreadFile right)
        {
            return !(left == right);
        }
    }
}
