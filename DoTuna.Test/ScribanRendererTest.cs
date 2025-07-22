using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using DoTuna;
using DoTuna.Features.Export;
using DoTuna.Core;

namespace DoTuna.Test
{
    public class ScribanRendererTest
    {
        [Fact]
        public async Task RenderIndexPageAsync_ReturnsHtml()
        {
            var threads = new List<JsonIndexDocument>
            {
                new JsonIndexDocument {
                    threadId = 1UL,
                    title = "테스트 제목",
                    username = "테스터",
                    createdAt = DateTime.Now,
                    updatedAt = DateTime.Now,
                    size = 10
                }
            };
            var fileNameMap = new ThreadFileMap(threads, "{id}");
            var renderer = new ScribanRenderer(
                fileNameMap,
                new ImageProvider(
                    new FileHelper("sourcePath"),
                    new FileHelper("resultPath")
                )
            );

            var html = await renderer.RenderIndexPageAsync(threads.ConvertAll(doc => HtmlIndexDocument.FromJson(doc, fileNameMap)));

            Assert.Contains("<html", html);
            Assert.Contains("테스트 제목", html);
            Assert.Contains("테스터", html);
        }

        [Fact]
        public async Task RenderThreadPageAsync_ReturnsHtml()
        {
            var thread = new JsonThreadDocument
            {
                boardId = "b",
                threadId = 1UL,
                title = "스레드 제목",
                username = "작성자",
                createdAt = new DateTime(2024, 6, 8, 12, 0, 0),
                updatedAt = new DateTime(2024, 6, 8, 13, 0, 0),
                size = 5,
                responses = new List<Response>
                {
                    new Response {
                        threadId = 1UL,
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
                    threadId = 1UL,
                    title = "스레드 제목",
                    username = "작성자",
                    createdAt = thread.createdAt,
                    updatedAt = thread.updatedAt,
                    size = 5
                }
            };
            var fileNameMap = new ThreadFileMap(threads, "{id}");
            var renderer = new ScribanRenderer(
                fileNameMap,
                new ImageProvider(
                    new FileHelper("sourcePath"),
                    new FileHelper("resultPath")
                )
            );

            var html = await renderer.RenderThreadPageAsync(thread);

            Assert.Contains("<html", html);
            Assert.Contains("스레드 제목", html);
            Assert.Contains("작성자", html);
            Assert.Contains("댓글 내용", html);
        }
        [Fact]
        public async Task RenderThreadPageAsync_WithAttachment_RendersImageTag()
        {
            Setting.Instance.SingleHTML = false; // 또는 true

            var thread = new JsonThreadDocument
            {
                boardId = "b",
                threadId = 2UL,
                title = "첨부 테스트",
                username = "이미지작성자",
                createdAt = new DateTime(2024, 6, 8, 14, 0, 0),
                updatedAt = new DateTime(2024, 6, 8, 15, 0, 0),
                size = 1,
                responses = new List<Response>
                {
                    new Response {
                        threadId = 2UL,
                        sequence = 1,
                        username = "이미지댓글러",
                        userId = "user2",
                        createdAt = new DateTime(2024, 6, 8, 14, 1, 0),
                        content = "이미지 첨부",
                        attachment = "test.png"
                    }
                }
            };

            var threads = new List<JsonIndexDocument> {
                new JsonIndexDocument {
                    threadId = 2UL,
                    title = "첨부 테스트",
                    username = "이미지작성자",
                    createdAt = thread.createdAt,
                    updatedAt = thread.updatedAt,
                    size = 1
                }
            };

            var fileNameMap = new ThreadFileMap(threads, "{id}");

            var fakeSourceHelper = new FakeFileHelper();
            fakeSourceHelper.BasePath = "/source";
            fakeSourceHelper.AddFile("data/test.png", new byte[] { 1, 2, 3 });  // 파일 내용 셋업

            var fakeResultHelper = new FakeFileHelper();
            fakeResultHelper.BasePath = "/result";

            var renderer = new ScribanRenderer(
                fileNameMap,
                new ImageProvider(fakeSourceHelper, fakeResultHelper)
            );

            var html = await renderer.RenderThreadPageAsync(thread);

            Assert.Contains("<html", html);
            Assert.Contains("첨부 테스트", html);
            Assert.Contains("이미지작성자", html);
            Assert.Contains("이미지 첨부", html);
            Assert.Contains("test.png", html);
            Assert.Contains("<img", html, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("data/", html, StringComparison.OrdinalIgnoreCase);
        }
    }
}
