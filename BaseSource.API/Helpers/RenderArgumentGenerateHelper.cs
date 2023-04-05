using FFmpegArgs;
using FFmpegArgs.Cores;
using FFmpegArgs.Cores.Enums;
using FFmpegArgs.Cores.Maps;
using FFmpegArgs.Inputs;
using FFmpegArgs.Outputs;

namespace BaseSource.API.Helpers
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
        //để sau thay thế hoặc thêm
        // dùng dictionary tốt nhất nếu được ở db
        public string OutputOption { get; set; }
        public string GlobalOption { get; set; }
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

            var video_map = ffmpegArg.AddVideoInput(new VideoFileInput(renderData.VideoName));

            AudioMap? audioMap = null;
            if (!string.IsNullOrWhiteSpace(renderData.AudioName))
            {
                audioMap = ffmpegArg.AddAudiosInput(new AudioFileInput(renderData.AudioName)).First();
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
