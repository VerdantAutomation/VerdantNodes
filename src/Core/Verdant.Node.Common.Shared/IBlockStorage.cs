using System.IO;

namespace Verdant.Node.Common
{
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
