using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CRM.ApiHub.Application.UseCases.Documents;
using CRM.ApiHub.Domain.Entities;
using CRM.ApiHub.Domain.Repositories;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace CRM.WebFrontend.Tests;

public class UploadDocumentTests : IDisposable
{
    private readonly string _tempStoragePath;

    public UploadDocumentTests()
    {
        _tempStoragePath = Path.Combine(Directory.GetCurrentDirectory(), "TestStorage_" + Guid.NewGuid().ToString("N"));
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempStoragePath))
        {
            Directory.Delete(_tempStoragePath, true);
        }
    }

    [Fact]
    public async Task ExecuteAsync_ShouldSaveFileToDiskAndSaveToRepository()
    {
        // Arrange
        var fakeRepository = new FakeOrderDocumentRepository();
        
        var inMemorySettings = new System.Collections.Generic.Dictionary<string, string?> {
            {"DocumentSettings:StoragePath", _tempStoragePath}
        };
        IConfiguration config = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var useCase = new UploadOrderDocumentUseCase(fakeRepository, config);

        var idOrder = 12345L;
        var documentType = "DNI";
        var originalFileName = "my_dni_front.png";
        var mimeType = "image/png";
        var fileSizeKb = 42;
        var fileContent = "Simulated PNG file content";
        var uploadedBy = 99L;

        using var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));

        // Act
        var result = await useCase.ExecuteAsync(
            idOrder,
            documentType,
            originalFileName,
            mimeType,
            fileSizeKb,
            fileStream,
            uploadedBy,
            CancellationToken.None
        );

        // Assert
        Assert.NotNull(result);
        Assert.Equal(idOrder, result.IdOrder);
        Assert.Equal(documentType, result.DocumentType);
        Assert.Equal(originalFileName, result.FileName);
        Assert.Equal(mimeType, result.MimeType);
        Assert.Equal(fileSizeKb, result.FileSizeKb);
        Assert.Equal("PENDING", result.VerificationStatus);
        Assert.Equal(uploadedBy, result.UploadedBy);
        Assert.True(result.IdDocument > 0);

        // Verify file got saved on disk
        Assert.True(File.Exists(result.FilePath));
        var savedContent = await File.ReadAllTextAsync(result.FilePath);
        Assert.Equal(fileContent, savedContent);

        // Verify repository received the save
        Assert.NotNull(fakeRepository.SavedDocument);
        Assert.Equal(result.FilePath, fakeRepository.SavedDocument.FilePath);
    }

    private class FakeOrderDocumentRepository : IOrderDocumentRepository
    {
        public OrderDocument? SavedDocument { get; private set; }
        private long _nextId = 1;

        public Task<IEnumerable<OrderDocument>> GetByOrderAsync(long idOrder, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<long> UploadAsync(OrderDocument document, CancellationToken ct = default)
        {
            document.IdDocument = _nextId++;
            SavedDocument = document;
            return Task.FromResult(document.IdDocument);
        }

        public Task<bool> UpdateVerificationAsync(long idDoc, string status, string? notes, long verifiedBy, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<OrderDocument?> GetByIdAsync(long idDoc, CancellationToken ct = default)
        {
            if (SavedDocument != null && SavedDocument.IdDocument == idDoc)
            {
                return Task.FromResult<OrderDocument?>(SavedDocument);
            }
            return Task.FromResult<OrderDocument?>(null);
        }
    }
}
