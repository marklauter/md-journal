using MD.Journal.IO.Indexes;

namespace MD.Journal.IO.Tests.Indexes
{
    public class PropertyIdTests
    {
        [Fact]
        public void PropertyId_WhenValueIsNull_ThrowsArgumentException()
        {
            var value = @"this is a string ~!@#$%^&*()_+;'[]\";
            var id = (PropertyId)value;
            Assert.Equal(id.ToString(), $"{id}");
            Assert.Equal(value, (string)id);
        }
    }
}
