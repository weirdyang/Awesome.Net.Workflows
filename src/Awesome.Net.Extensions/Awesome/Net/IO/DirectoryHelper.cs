using System;
using System.IO;

namespace Awesome.Net.IO
{
    /// <summary>
    /// A helper class for Directory operations.
    /// </summary>
    public static class DirectoryHelper
    {
        public static void CreateIfNotExists(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public static bool IsSubDirectoryOf(string parentDirectoryPath, string childDirectoryPath)
        {
            if (parentDirectoryPath == null)
            {
                throw new ArgumentNullException(nameof(parentDirectoryPath));
            }

            if (childDirectoryPath == null)
            {
                throw new ArgumentNullException(nameof(childDirectoryPath));
            }

            return IsSubDirectoryOf(
                new DirectoryInfo(parentDirectoryPath),
                new DirectoryInfo(childDirectoryPath)
            );
        }

        public static bool IsSubDirectoryOf(DirectoryInfo parentDirectory, DirectoryInfo childDirectory)
        {
            if (parentDirectory == null) throw new ArgumentNullException(nameof(parentDirectory));
            if (childDirectory == null) throw new ArgumentNullException(nameof(childDirectory));

            if (parentDirectory.FullName == childDirectory.FullName)
            {
                return true;
            }

            var parentOfChild = childDirectory.Parent;
            if (parentOfChild == null)
            {
                return false;
            }

            return IsSubDirectoryOf(parentDirectory, parentOfChild);
        }
    }
}
