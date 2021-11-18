using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Umwelt.Faculty.Csv.Algorithm1
{
    /// <summary>
    /// 入出力の種類（単一ファイルか複数ファイルか）を認識し、入出力パスを解決し、返します。<br />
    /// 必要があれば、ファイルの移動を行います。<br />
    /// このファイルがどんな方針のもとに処理をしているかは、プロジェクトのWikiを参照して下さい。<br />
    /// https://github.com/Tryeting/Umwelt.Faculty.Template/wiki
    /// </summary>
    static class Initializer
    {
        /// <summary>
        /// 入力にファイルを要求し、ファイルを出力する場合の入出力パスを取得します。
        /// </summary>
        public static (string inputPath, string outputPath) InFileOutFile(IConfiguration configuration)
        {
            var incar = configuration.GetSection("INCAR");

            var inputPath = configuration["INPUT_PATH"];
            var outputPath = configuration["OUTPUT_PATH"];

            var incarOutputPath = incar["OutputPath"];
            if (string.IsNullOrWhiteSpace(incarOutputPath))
            {
                Console.WriteLine("出力パスが無効か、入力されませんでした。");
                Console.WriteLine("単一ファイル形式で出力します。");

                if (Directory.Exists(inputPath))
                {
                    Console.WriteLine("入力が複数ファイル形式でした。");
                    Console.WriteLine("入力ファイルパスを解決します。");

                    inputPath = CombinePath(inputPath, incar["InputPath"]);
                    Console.WriteLine($"入力ファイルパス: \"{inputPath}\"");
                    if (!File.Exists(inputPath)) throw new FacultyException("指定したファイルは存在しませんでした。");
                }
            }
            else
            {
                Console.WriteLine("出力パスが入力されています。");
                Console.WriteLine("複数ファイル形式で出力します。");

                if (Directory.Exists(inputPath))
                {
                    Console.WriteLine("入力が複数ファイル形式でした。");
                    Console.WriteLine("入力フォルダを出力に移動します。");

                    if (inputPath != outputPath) Directory.Move(inputPath, outputPath);

                    Console.WriteLine("入力ファイルパスを解決します。");

                    inputPath = CombinePath(outputPath, incar["InputPath"]);
                    Console.WriteLine($"入力ファイルパス: \"{inputPath}\"");
                    if (!File.Exists(inputPath)) throw new FacultyException("指定したファイルは存在しませんでした。");
                }
                else
                {
                    Console.WriteLine("入力が単一ファイル形式でした。");
                    Console.WriteLine("入力ファイルを出力フォルダの中に移動します。");

                    Directory.CreateDirectory(outputPath);
                    var tempInputPath = Path.Combine(outputPath, "input");
                    File.Move(inputPath, tempInputPath);
                    inputPath = tempInputPath;
                }

                Console.WriteLine("出力ファイルパスを解決します。");
                outputPath = CombinePath(outputPath, incarOutputPath);

                if (inputPath == outputPath)
                {
                    Console.WriteLine($"上書きモードです。");
                    var taskPath = configuration["TASK_PATH"];
                    inputPath = Path.Combine(taskPath, "temp");
                    if (Directory.Exists(inputPath)) Directory.Delete(inputPath, true);
                    File.Move(outputPath, inputPath, true);
                }

                Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
            }

            return (inputPath, outputPath);
        }

        /// <summary>
        /// 入力にディレクトリを要求し、ファイルを出力する場合の入出力パスを取得します。
        /// </summary>
        public static (string inputPath, string outputPath) InDirectoryOutFile(IConfiguration configuration)
        {
            var incar = configuration.GetSection("INCAR");

            var inputPath = configuration["INPUT_PATH"];
            var outputPath = configuration["OUTPUT_PATH"];

            if (!Directory.Exists(inputPath)) throw new FacultyException("入力が複数ファイル形式ではありませんでした。");

            var incarOutputPath = incar["OutputPath"];
            if (string.IsNullOrWhiteSpace(incarOutputPath))
            {
                Console.WriteLine("出力パスが無効か、入力されませんでした。");
                Console.WriteLine("単一ファイル形式で出力します。");

                Console.WriteLine("入力ディレクトリパスを解決します。");

                inputPath = CombinePath(inputPath, incar["InputPath"]);
                Console.WriteLine($"入力ディレクトリパス: \"{inputPath}\"");
                if (!Directory.Exists(inputPath)) throw new FacultyException("指定したディレクトリは存在しませんでした。");
            }
            else
            {
                Console.WriteLine("出力パスが入力されています。");
                Console.WriteLine("複数ファイル形式で出力します。");

                Console.WriteLine("入力フォルダを出力に移動します。");

                if (inputPath != outputPath) Directory.Move(inputPath, outputPath);

                Console.WriteLine("入力ディレクトリパスを解決します。");

                inputPath = CombinePath(outputPath, incar["InputPath"]);
                Console.WriteLine($"入力ディレクトリパス: \"{inputPath}\"");
                if (!Directory.Exists(inputPath)) throw new FacultyException("指定したディレクトリは存在しませんでした。");

                Console.WriteLine("出力ファイルパスを解決します。");
                outputPath = CombinePath(outputPath, incarOutputPath);

                if (inputPath == outputPath)
                {
                    Console.WriteLine($"上書きモードです。");
                    var taskPath = configuration["TASK_PATH"];
                    inputPath = Path.Combine(taskPath, "temp");
                    if (Directory.Exists(inputPath)) Directory.Delete(inputPath, true);
                    Directory.Move(outputPath, inputPath);
                }

                Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
            }

            return (inputPath, outputPath);
        }

        /// <summary>
        /// 入力にファイルを要求し、ディレクトリを出力する場合の入出力パスを取得します。
        /// </summary>
        public static (string inputPath, string outputPath) InFileOutDirectory(IConfiguration configuration)
        {
            var incar = configuration.GetSection("INCAR");

            var inputPath = configuration["INPUT_PATH"];
            var outputPath = configuration["OUTPUT_PATH"];

            var incarOutputPath = incar["OutputPath"];
            if (string.IsNullOrWhiteSpace(incarOutputPath))
            {
                Console.WriteLine("出力パスが無効か、入力されませんでした。");
                Console.WriteLine("単一ファイル形式で出力します。");

                if (Directory.Exists(inputPath))
                {
                    Console.WriteLine("入力が複数ファイル形式でした。");
                    Console.WriteLine("入力ファイルパスを解決します。");

                    inputPath = CombinePath(inputPath, incar["InputPath"]);
                    Console.WriteLine($"入力ファイルパス: \"{inputPath}\"");
                    if (!File.Exists(inputPath)) throw new FacultyException("指定したファイルは存在しませんでした。");
                }

                Directory.CreateDirectory(outputPath);
            }
            else
            {
                Console.WriteLine("出力パスが入力されています。");
                Console.WriteLine("複数ファイル形式で出力します。");

                if (Directory.Exists(inputPath))
                {
                    Console.WriteLine("入力が複数ファイル形式でした。");
                    Console.WriteLine("入力フォルダを出力に移動します。");

                    if (inputPath != outputPath) Directory.Move(inputPath, outputPath);

                    Console.WriteLine("入力ファイルパスを解決します。");

                    inputPath = CombinePath(outputPath, incar["InputPath"]);
                    Console.WriteLine($"入力ファイルパス: \"{inputPath}\"");
                    if (!File.Exists(inputPath)) throw new FacultyException("指定したファイルは存在しませんでした。");
                }
                else
                {
                    Console.WriteLine("入力が単一ファイル形式でした。");
                    Console.WriteLine("入力ファイルを出力フォルダの中に移動します。");

                    Directory.CreateDirectory(outputPath);
                    var tempInputPath = Path.Combine(outputPath, "input");
                    File.Move(inputPath, tempInputPath);
                    inputPath = tempInputPath;
                }

                Console.WriteLine("出力ディレクトリパスを解決します。");
                outputPath = CombinePath(outputPath, incarOutputPath);

                if (inputPath == outputPath)
                {
                    Console.WriteLine($"上書きモードです。");
                    var taskPath = configuration["TASK_PATH"];
                    inputPath = Path.Combine(taskPath, "temp");
                    if (Directory.Exists(inputPath)) Directory.Delete(inputPath, true);
                    File.Move(outputPath, inputPath, true);
                }

                Directory.CreateDirectory(outputPath);
            }

            return (inputPath, outputPath);
        }

        /// <summary>
        /// 入力にディレクトリを要求し、ディレクトリを出力する場合の入出力パスを取得します。
        /// </summary>
        public static (string inputPath, string outputPath) InDirectoryOutDirectory(IConfiguration configuration)
        {
            var incar = configuration.GetSection("INCAR");

            var inputPath = configuration["INPUT_PATH"];
            var outputPath = configuration["OUTPUT_PATH"];

            if (!Directory.Exists(inputPath)) throw new FacultyException("入力が複数ファイル形式ではありませんでした。");

            var incarOutputPath = incar["OutputPath"];
            if (string.IsNullOrWhiteSpace(incarOutputPath))
            {
                Console.WriteLine("出力パスが無効か、入力されませんでした。");
                Console.WriteLine("単一ファイル形式で出力します。");

                Console.WriteLine("入力ディレクトリパスを解決します。");

                inputPath = CombinePath(inputPath, incar["InputPath"]);
                Console.WriteLine($"入力ディレクトリパス: \"{inputPath}\"");
                if (!Directory.Exists(inputPath)) throw new FacultyException("指定したディレクトリは存在しませんでした。");

                Directory.CreateDirectory(outputPath);
            }
            else
            {
                Console.WriteLine("出力パスが入力されています。");
                Console.WriteLine("複数ファイル形式で出力します。");

                Console.WriteLine("入力フォルダを出力に移動します。");

                if (inputPath != outputPath) Directory.Move(inputPath, outputPath);

                Console.WriteLine("入力ディレクトリパスを解決します。");

                inputPath = CombinePath(outputPath, incar["InputPath"]);
                Console.WriteLine($"入力ディレクトリパス: \"{inputPath}\"");
                if (!Directory.Exists(inputPath)) throw new FacultyException("指定したディレクトリは存在しませんでした。");

                Console.WriteLine("出力ディレクトリパスを解決します。");
                outputPath = CombinePath(outputPath, incarOutputPath);

                if (inputPath == outputPath)
                {
                    Console.WriteLine($"上書きモードです。");
                    var taskPath = configuration["TASK_PATH"];
                    inputPath = Path.Combine(taskPath, "temp");
                    if (Directory.Exists(inputPath)) Directory.Delete(inputPath, true);
                    Directory.Move(outputPath, inputPath);
                }

                Directory.CreateDirectory(outputPath);
            }

            return (inputPath, outputPath);
        }

        private static string CombinePath(string basePath, string relativePath)
        {
            basePath = Path.GetFullPath(basePath);
            if (string.IsNullOrEmpty(relativePath)) return basePath;
            var path = Path.GetFullPath(Path.Combine(basePath, relativePath));
            if (path.StartsWith(basePath)) return path;
            throw new FacultyException($"設定されたパスが不正です。\r\n{relativePath}");
        }
    }
}
