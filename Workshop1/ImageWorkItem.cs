namespace Workshop1;

using Core;

public record ImageWorkItem(string Id, string ImagePath, ImageSize TargetSize) : IWorkItem
{
    public string Description => $"Resize {Path.GetFileName(ImagePath)} to {TargetSize}";
}

public enum ImageSize
{
    Thumbnail,  // 150x150
    Medium,     // 800x600
    Large       // 1920x1080
}
