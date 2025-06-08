using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using DoTuna;

namespace DoTuna.Test
{
    public class ScribanRendererTest
    {
        [Fact]
        public async Task RenderIndexPageAsync_ReturnsHtml()
        {
            // Arrange
            var threads = new List<JsonIndexDocument>
            {
                new JsonIndexDocument {
                    threadId = 1,
                    title = "테스트 제목",
                    username = "테스터",
                    createdAt = DateTime.Now,
                    updatedAt = DateTime.Now,
                    size = 10
                }
            };
            var fileNameMap = new ThreadFileNameMap(threads, "{id}");
            var renderer = new ScribanRenderer(fileNameMap);

            // Act
            var html = await renderer.RenderIndexPageAsync(threads);

            // Assert
            Assert.Contains("<html", html);
            Assert.Contains("테스트 제목", html);
            Assert.Contains("테스터", html);
        }

        [Fact]
        public async Task RenderThreadPageAsync_ReturnsHtml()
        {
            // Arrange
            var thread = new JsonThreadDocument
            {
                boardId = "b",
                threadId = 1,
                title = "스레드 제목",
                username = "작성자",
                createdAt = new DateTime(2024, 6, 8, 12, 0, 0),
                updatedAt = new DateTime(2024, 6, 8, 13, 0, 0),
                size = 5,
                responses = new List<Response>
                {
                    new Response {
                        threadId = 1,
                        sequence = 1,
                        username = "댓글러",
                        userId = "user1",
                        createdAt = new DateTime(2024, 6, 8, 12, 1, 0),
                        content = "댓글 내용",
                        attachment = ""
                    }
                }
            };
            var threads = new List<JsonIndexDocument> {
                new JsonIndexDocument {
                    threadId = 1,
                    title = "스레드 제목",
                    username = "작성자",
                    createdAt = thread.createdAt,
                    updatedAt = thread.updatedAt,
                    size = 5
                }
            };
            var fileNameMap = new ThreadFileNameMap(threads, "{id}");
            var renderer = new ScribanRenderer(fileNameMap);

            // Act
            var html = await renderer.RenderThreadPageAsync(thread);

            // Assert
            Assert.Contains("<html", html);
            Assert.Contains("스레드 제목", html);
            Assert.Contains("작성자", html);
            Assert.Contains("댓글 내용", html);
        }
    }
}
