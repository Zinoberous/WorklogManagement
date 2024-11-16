namespace WorklogManagement.Service.Common;

public static class Configuration
{
    internal static string AttachmentsBaseDir { get; private set; } = Path.Combine(".", "Attachments");

    public static void SetAttachmentsBaseDir(string attachmentsBaseDir)
    {
        AttachmentsBaseDir = attachmentsBaseDir;
    }
}
