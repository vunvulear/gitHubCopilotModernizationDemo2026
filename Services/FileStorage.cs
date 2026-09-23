using System.IO;
using System.Web;

namespace ContosoUniversity.Services
{
    public class FileStorage : IFileStorage
    {
        private readonly System.Func<string, string> _mapPath;

        public FileStorage(System.Func<string, string> mapPath)
        {
            _mapPath = mapPath;
        }

        public string MapPath(string virtualPath)
        {
            return _mapPath(virtualPath);
        }

        public void EnsureDirectory(string physicalPath)
        {
            if (!Directory.Exists(physicalPath))
                Directory.CreateDirectory(physicalPath);
        }

        public void SavePostedFile(HttpPostedFileBase postedFile, string physicalPath)
        {
            // Use SaveAs to preserve posted file semantics
            postedFile.SaveAs(physicalPath);
        }
    }
}
