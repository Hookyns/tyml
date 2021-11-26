using System;
using System.IO;
using Xunit;

namespace RJDev.Tyml.Core.Tests
{
	public class WorkingDirTest : TestBase
	{
		[Fact]
		public void RelativeWorkingDirTest()
		{
			Assert.Throws<ArgumentException>(() =>
			{
				new TymlContextBuilder().UseWorkingDirectory("../foo").Build();
			});
		}
		
		[Fact]
		public void AbsoluteWorkingDirTest()
		{
			try
			{
				new TymlContextBuilder().UseWorkingDirectory("C:/work/dir/absolute").Build();
				Assert.True(true);
			}
			catch (Exception)
			{
				Assert.True(false);
			}
		}
		
		[Fact]
		public void WorkingDirNormalizeTest()
		{
			try
			{
				TymlContext context = new TymlContextBuilder().UseWorkingDirectory(Path.Combine("C:\\\\some/folder", "..", "work", "./dir")).Build();
				Assert.Equal("C:\\some\\work\\dir", context.WorkingDirectory);
			}
			catch (Exception)
			{
				Assert.True(false);
			}
		}
	}
}