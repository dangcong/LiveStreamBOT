using FFmpegArgs;
using FFmpegArgs.Cores;
using FFmpegArgs.Cores.Enums;
using FFmpegArgs.Cores.Maps;
using FFmpegArgs.Inputs;
using FFmpegArgs.Outputs;

namespace BaseSource.Shared.Helpers
{
    public class RenderData
    {
        //để sau thay thế hoặc thêm
        public string VideoName { get; set; }
        public string? AudioName { get; set; }
        public Uri StreamOutput { get; set; }
    }
    public class PackageConfig
    {
        public List<string> OutputOptions { get; } = new List<string>();
        public string OutputOption { get { return string.Join(" ", OutputOptions); } }

        public List<string> GlobalOptions { get; } = new List<string>();
        public string GlobalOption { get { return string.Join(" ", GlobalOptions); } }

        public List<string> VideoInputOptions { get; } = new List<string>();
        public string VideoInputOption { get { return string.Join(" ", VideoInputOptions); } }

        public List<string> AudioInputOptions { get; } = new List<string>();
        public string AudioInputOption { get { return string.Join(" ", AudioInputOptions); } }
    }




    public interface IRenderArgumentGenerateHelper
    {
        FFmpegArg Generate(RenderData renderData, PackageConfig packageConfig);
    }
    public class RenderArgumentGenerateHelper : IRenderArgumentGenerateHelper
    {
        public RenderArgumentGenerateHelper()
        {

        }

        public FFmpegArg Generate(RenderData renderData, PackageConfig packageConfig)
        {
            if (renderData is null)
                throw new ArgumentNullException(nameof(renderData));
            if (string.IsNullOrWhiteSpace(renderData.VideoName))
                throw new ArgumentNullException($"{nameof(renderData)}.{nameof(renderData.VideoName)}");
            if (renderData.StreamOutput is null)
                throw new ArgumentNullException($"{nameof(renderData)}.{nameof(renderData.StreamOutput)}");

            FFmpegArg ffmpegArg = new FFmpegArg();

            VideoFileInput videoFileInput = new VideoFileInput(renderData.VideoName);
            if(!string.IsNullOrWhiteSpace(packageConfig.VideoInputOption))
                videoFileInput.SetFlag(packageConfig.VideoInputOption.Trim());
            var video_map = ffmpegArg.AddVideoInput(videoFileInput);

            AudioMap? audioMap = null;
            if (!string.IsNullOrWhiteSpace(renderData.AudioName))
            {
                AudioFileInput audioFileInput = new AudioFileInput(renderData.AudioName);
                if (!string.IsNullOrWhiteSpace(packageConfig.AudioInputOption))
                    audioFileInput.SetFlag(packageConfig.AudioInputOption.Trim());
                audioMap = ffmpegArg.AddAudiosInput(audioFileInput).First();
            }

            ImageMap imageMap = video_map.ImageMaps.First();
            if (audioMap is null) audioMap = video_map.AudioMaps.First();

            VideoUrlOutput videoFileOutput = new VideoUrlOutput(renderData.StreamOutput, MuxingFileFormat.flv, imageMap, audioMap);
            if (!string.IsNullOrWhiteSpace(packageConfig?.OutputOption))
                videoFileOutput.SetFlag(packageConfig.OutputOption.Trim());

            ffmpegArg.AddOutput(videoFileOutput);
            if (!string.IsNullOrWhiteSpace(packageConfig?.GlobalOption))
                ffmpegArg.SetFlag(packageConfig.GlobalOption.Trim());

            return ffmpegArg;
        }
    }
}
