using System;
using System.Threading.Tasks;
using Xunit;

namespace DoTuna.Test
{

    public class ImageProviderTest
    {
        [Fact]
        public async Task Href_ReturnsBase64_WhenSingleHTMLTrue_AndFileExists()
        {
            // Arrange
            var fileName = "image.jpg";
            Setting.Instance.SingleHTML = true;

            var fakeHelper = new FakeFileHelper();
            fakeHelper.BasePath = "/source";
            fakeHelper.AddFile("data/image.jpg", new byte[] { 0x01, 0x02, 0x03 });

            var provider = new ImageProvider(fakeHelper, new FakeFileHelper());

            // Act
            var result = await provider.Href(fileName);

            // Assert
            Assert.StartsWith("data:image/jpeg;base64,", result);
        }

        [Fact]
        public async Task Href_ReturnsRelativePath_WhenSingleHTMLFalse()
        {
            // Arrange
            var fileName = "pic.png";
            Setting.Instance.SingleHTML = false;

            var fakeHelper = new FakeFileHelper();
            var provider = new ImageProvider(fakeHelper, fakeHelper);

            // Act
            var result = await provider.Href(fileName);

            // Assert
            Assert.Equal("data/" + Uri.EscapeDataString(fileName), result);
        }

        [Fact]
        public async Task Href_ReturnsEscapedFileName_WhenFileDoesNotExist()
        {
            // Arrange
            var fileName = "nofile.gif";
            Setting.Instance.SingleHTML = true;

            var fakeHelper = new FakeFileHelper(); // 파일 없음
            var provider = new ImageProvider(fakeHelper, fakeHelper);

            // Act
            var result = await provider.Href(fileName);

            // Assert
            Assert.Equal(Uri.EscapeDataString(fileName), result);
        }
    }
}
