namespace mvc.BLL.DTOs;

public sealed record PosterUpload(Stream Content, string FileName, string ContentType, long Length);
