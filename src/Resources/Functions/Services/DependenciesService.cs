using System.IO;
using System.IO.Compression;
using System.Net.Http;

namespace PakMaster.Resources.Functions.Services
{
    public static class DependenciesService
    {
        // Method to check if dependencies exist in the specified subdirectory
        public static bool CheckIfDependencyExists(string subDirectory, string exeName)
        {
            string targetDirectory = Path.Combine(Directory.GetCurrentDirectory(), "bin", subDirectory);
            string targetFilePath = Path.Combine(targetDirectory, exeName);

            return File.Exists(targetFilePath);
        }

        public static async Task DependenciesManagerAsync(string fileUrl, string subDirectoryName)
        {
            try
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string targetFolderPath = Path.Combine(baseDirectory, "bin", subDirectoryName);

                if (!Directory.Exists(targetFolderPath))
                {
                    Directory.CreateDirectory(targetFolderPath);
                }

                string fileExtension = Path.GetExtension(fileUrl).ToLower();

                if (fileExtension == ".zip")
                {
                    string tempZipFilePath = Path.Combine(baseDirectory, "temp_69420.zip");

                    using (HttpClient client = new HttpClient())
                    {
                        byte[] fileBytes = await client.GetByteArrayAsync(fileUrl);
                        await File.WriteAllBytesAsync(tempZipFilePath, fileBytes);
                    }

                    if (File.Exists(tempZipFilePath))
                    {
                        ZipFile.ExtractToDirectory(tempZipFilePath, targetFolderPath);
                        File.Delete(tempZipFilePath);
                        Console.WriteLine($"Zip file extracted to: {targetFolderPath}");
                    }
                    else
                    {
                        Console.WriteLine("Error: Downloaded zip file not found.");
                    }
                }
                else
                {
                    string fileName = Path.GetFileName(fileUrl);
                    string targetFilePath = Path.Combine(targetFolderPath, fileName);

                    using (HttpClient client = new HttpClient())
                    {
                        byte[] fileBytes = await client.GetByteArrayAsync(fileUrl);
                        await File.WriteAllBytesAsync(targetFilePath, fileBytes);
                    }

                    Console.WriteLine($"File downloaded to: {targetFilePath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while downloading the file: {ex.Message}");
            }
        }
    }
}
