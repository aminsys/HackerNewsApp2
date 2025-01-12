using HackerNewsApp2.Model;

namespace HackerNewsApp2Test
{
    [TestClass]
    public class HNAlgoliaModelTest
    {
        [TestMethod]
        public void ModelValidation_ReturnsSuccess()
        {
            var model = new HNAlgoliaModel
            {
                Id = 1,
                Title = "Test Title",
                Type = "Test Type",
                Author = "logfromblammo",
                Points = 1,
                Dead = false,
                Deleted = false,
                Created_At = DateTime.Now,
                Parent = 1,
                Poll = 1,
                Parts = 1,
                Url = "http://test.com",
                Children = new HNAlgoliaModel[1],
                Descendants = 1,
                Text = "No, the equivalent would be as soon as he raises the issue, no one can type even one more line of code, not even a character, and their editor would be completely blocked.<p>That’s what these companies need. The entire business comes to a screeching halt.<p>And if they really needed more motivation, production servers could be shut down if the issue was not resolved in 24 hours. That will really hold their feet to the fire."
            };

            Assert.Equals(1, model.Id);

        }
    }
}