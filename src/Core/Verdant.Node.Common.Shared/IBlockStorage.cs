using System.IO;

namespace Verdant.Node.Common
{
    public enum FileMode
    {
        CreateNew = 1,
        Create = 2,
        Open = 3,
        OpenOrCreate = 4,
        Truncate = 5,
        Append = 6
    }

    public interface IBlockStorage
    {
        bool IsFormatted { get; }

        void Format();

        void Mount();

        Stream Open(string fileName, FileMode fileMode);

        string[] GetFiles();

        void Delete(string fileName);
    }
}
