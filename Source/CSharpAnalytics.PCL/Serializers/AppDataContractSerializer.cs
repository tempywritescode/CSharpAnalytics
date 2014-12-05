using PCLStorage;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CSharpAnalytics.PCL.Serializers
{
    /// <summary>
    /// Provides an easy way to serialize and deserialize simple classes to a user AppData folder in
    /// Windows Forms applications.
    /// </summary>
    internal static class AppDataContractSerializer
    {
        /// <summary>
        /// Restore an object from local folder storage.
        /// </summary>
        /// <param name="filename">Optional filename to use, name of the class if not provided.</param>
        /// <param name="deleteBadData">Optional boolean on whether delete the existing file if deserialization fails, defaults to false.</param>
        /// <returns>Task that holds the deserialized object once complete.</returns>
        public static async Task<T> Restore<T>(string filename = null, bool deleteBadData = false)
        {
            var serializer = new DataContractSerializer(typeof(T), new[] { typeof(DateTimeOffset) });
            T result = default(T);

            try
            {
                var file = await GetFilePath<T>(filename, true);
                SerializationException ex = null;
                try
                {
                    using (var inputStream = await file.OpenAsync(FileAccess.Read))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await inputStream.CopyToAsync(memoryStream);
                            await inputStream.FlushAsync();
                            memoryStream.Seek(0, SeekOrigin.Begin);

                            if (memoryStream.Length == 0) {
                                return default(T);
                            }

                            result = (T)serializer.ReadObject(memoryStream);
                        }
                    }
                }
                catch (SerializationException e)
                {
                    ex = e;
                }
                if (ex != null) {
                    if (deleteBadData)
                        await file.DeleteAsync();

                    throw ex;
                }

                return result;
            }
            catch (FileNotFoundException)
            {
                return default(T);
            }
        }

        /// <summary>
        /// Save an object to local folder storage asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="value"/> to save.</typeparam>
        /// <param name="value">Object to save to local storage.</param>
        /// <param name="filename">Optional filename to save to, defaults to the name of the class.</param>
        /// <returns>Task that completes once the object is saved.</returns>
        public static async Task Save<T>(T value, string filename = null)
        {
            var serializer = new DataContractSerializer(typeof(T), new[] { typeof(DateTimeOffset) });

            var file = await GetFilePath<T>(filename, false);

            using (var memoryStream = new MemoryStream())
            {
                serializer.WriteObject(memoryStream, value);

                using (var fileStream = await file.OpenAsync(FileAccess.ReadAndWrite))
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    await memoryStream.CopyToAsync(fileStream);
                    await fileStream.FlushAsync();
                }
            }
        }

        /// <summary>
        /// Gets the file path of the given type and file name.
        /// </summary>
        /// <typeparam name="T">The type to get path.</typeparam>
        /// <param name="filename">The file name to get path.</param>
        /// <returns>The full path to the file.</returns>
        private static async Task<IFile> GetFilePath<T>(string filename, bool ToRead)
        {
            var folder = await FileSystem.Current.LocalStorage.CreateFolderAsync("analytics_local", CreationCollisionOption.OpenIfExists);

            if (ToRead) {
                return await folder.GetFileAsync(filename);
            } else {
                return await folder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            }
        }
    }
}