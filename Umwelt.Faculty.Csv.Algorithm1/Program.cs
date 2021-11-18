using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umwelt.Faculty.FacultyBase;

namespace Umwelt.Faculty.Csv.Algorithm1
{
    /* まずやること：
     * 1. githubに登録します。なによりも先に。なによりも先に！！！！！！！！！！！！！！！！！！！！
     * 2. githubに登録した後に、Umwelt.Faculty.FacultyBaseプロジェクトを追加します。
     * 3. Facultyクラスの中に実際の処理を書きます。
     */

    class Program
    {
        static async Task Main(string[] args)
        {
            SetOut();
            Console.WriteLine("設定値の読み込みを開始します。");
            var configuration = FacultyUtils.BuildConfiguration(args);

            try
            {
                Console.WriteLine("依存関係を構成します。");
                var services = new ServiceCollection();
                services.AddSingleton(configuration);
                services.AddSingleton<Faculty>();

                using var provider = services.BuildServiceProvider();
                var faculty = provider.GetRequiredService<Faculty>();

                Console.WriteLine("処理を実行します。");
                await faculty.ExecuteAsync();
            }
            catch (FacultyException exp)
            {
                var message = exp.Message;
                Console.WriteLine(message);
                var errorPath = configuration.GetValue<string>("ERROR_PATH");
                if (!string.IsNullOrEmpty(errorPath)) await File.WriteAllTextAsync(errorPath, message);
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.ToString());
                var errorPath = configuration.GetValue<string>("ERROR_PATH");
                if (!string.IsNullOrEmpty(errorPath)) await File.WriteAllTextAsync(errorPath, "不明なエラーが発生しました。");
            }
        }

        private static void SetOut()
        {
            var writer = new FacultyTextWriter(Console.Out);
            Console.SetOut(writer);
        }
    }

    class FacultyTextWriter : TextWriter
    {
        private readonly Stopwatch _stopwatch;

        public TextWriter BaseWriter { get; }

        public override Encoding Encoding => BaseWriter.Encoding;

        public FacultyTextWriter(TextWriter baseWriter)
        {
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
            BaseWriter = baseWriter;
        }

        public override void WriteLine(string? value)
        {
            if (value is not null) value = $"{_stopwatch.Elapsed:mm\\:ss\\.f} {value}";
            BaseWriter.WriteLine(value);
        }
    }

    /// <summary>
    /// アルゴリズムで発生した例外です。<br />
    /// メッセージが、そのままアルゴリズムのエラーとして報告されます。
    /// </summary>
    class FacultyException : Exception
    {
        private List<string>? _messages;

        public override string Message => _messages is null ? "" : string.Join("\r\n", _messages);

        public FacultyException() { }

        public FacultyException(string message) => _messages = new List<string> { message };

        public void AddMessage(string message)
        {
            if (_messages is null) _messages = new List<string>();
            _messages.Add(message);
        }

        public bool Any() => _messages is not null && _messages.Any();
    }
}
