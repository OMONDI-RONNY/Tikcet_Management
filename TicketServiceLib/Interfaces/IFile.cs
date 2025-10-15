using System;
using Microsoft.AspNetCore.Http;

namespace TicketServiceLib.Interfaces;

public interface IFile
{
    /// <summary>
    /// Saves file in a directory
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    Task SaveFileAsync(IFormFile file);
    /// <summary>
    /// Deletes file from a directory
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    void DeleteFile(string fileName);
}
