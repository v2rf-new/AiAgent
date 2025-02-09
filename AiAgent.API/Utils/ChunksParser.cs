using System.Collections.ObjectModel;
using System.Text;

namespace AiAgent.API.Utils
{
    public static class ChunksParser
    {
        private static int _breaksOfLine = 2;

        public static async Task<ReadOnlyCollection<string>> SegmentFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is null or empty", nameof(file));
            }

            if (file.ContentType != "text/plain")
            {
                throw new ArgumentException("File must be a text/plain file", nameof(file));
            }

            List<string> segments = [];
            StringBuilder currentSegment = new();
            int emptyLineCount = 0;

            using (var streamReader = new StreamReader(file.OpenReadStream()))
            {
                string? line;

                while ((line = await streamReader.ReadLineAsync()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        emptyLineCount++;
                        if (emptyLineCount == _breaksOfLine)
                        {
                            segments.Add(currentSegment.ToString());
                            currentSegment.Clear();
                            emptyLineCount = 0;
                        }
                    }
                    else
                    {
                        emptyLineCount = 0;
                        currentSegment.AppendLine(line);
                    }
                }

                if (currentSegment.Length > 0)
                {
                    segments.Add(currentSegment.ToString());
                }
            }

            return segments.AsReadOnly();
        }
        [Obsolete()]
        public static ReadOnlyCollection<string> SegmentFile(string filePath)
        {
            List<string> segments = [];
            StringBuilder currentSegment = new();
            int emptyLineCount = 0;


            if (File.Exists(filePath))
            {
                using (StreamReader streamReader = new(filePath))
                {
                    string? line;

                    while ((line = streamReader.ReadLine()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            emptyLineCount++;
                            if (emptyLineCount == _breaksOfLine)
                            {
                                segments.Add(currentSegment.ToString());
                                currentSegment.Clear();
                                emptyLineCount = 0;
                            }
                        }
                        else
                        {
                            emptyLineCount = 0;
                            currentSegment.AppendLine(line);
                        }
                    }

                    if (currentSegment.Length > 0)
                    {
                        segments.Add(currentSegment.ToString());
                    }
                }
            }
            else
            {
                throw new Exception("File provided for SegmentFile static method does not exist.");
            }

            return segments.AsReadOnly();
        }
    }
}
