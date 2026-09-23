using System.Web;

namespace ContosoUniversity.Services
{
    public interface IFileStorage
    {
        // Map virtual path to physical path
        string MapPath(string virtualPath);

        // Ensure directory exists
        void EnsureDirectory(string physicalPath);

        // Save posted file to physical path
        void SavePostedFile(HttpPostedFileBase postedFile, string physicalPath);
    }
}
