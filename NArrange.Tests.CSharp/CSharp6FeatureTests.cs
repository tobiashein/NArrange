﻿using FluentAssertions;
using NArrange.Core;
using NArrange.Core.CodeElements;
using NArrange.CSharp;
using NArrange.Tests.Core;
using NUnit.Framework;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.IO;

namespace NArrange.Tests.CSharp
{
	/// <summary>
	/// Class to assert that all C# 6.0 features work in NArrange.
	/// </summary>
	[TestFixture]
	public class CSharp6FeatureTests
	{
		#region Methods

		/// <summary>
		/// Asserts that auto property initializers work. They are new in C# 6 and old NArrange parser broke when encountering them.
		/// </summary>
		[Test]
		public void AssertAutoPropertyInitializersAreCorrectlyFormatted()
		{
			TryCompileArrangeAndRecompile("AutoPropertyInitializers.cs");
		}

		/// <summary>
		/// Tests the auto property initializer.
		/// </summary>
		[Test]
		public void TestAutoPropertyInitializersParseCorrectly()
		{
			CSharpTestFile testFile = CSharpTestUtilities.GetAutoPropertyInitializersFile();
			using (TextReader reader = testFile.GetReader())
			{
				CSharpParser parser = new CSharpParser();
				ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);
				elements.Should().HaveCount(1, "because there is only one namespace");
				elements[0].Children.Should().HaveCount(1, "because there is 1 class in the namespace");
				elements[0].Children[0].Children.Should().HaveCount(4, "because there are 3 subclasses in the class");
				var customer1 = elements[0].Children[0].Children[0];
				var customer2 = elements[0].Children[0].Children[1];
				var customer3 = elements[0].Children[0].Children[2];
				var customer4 = elements[0].Children[0].Children[3];
				customer1.Children.Should().HaveCount(2, "because there are only 2 properties");
				customer1.Children[0].ElementType.Should().Be(ElementType.Property);
				customer1.Children[1].ElementType.Should().Be(ElementType.Property);
				customer2.Children.Should().HaveCount(2, "because there are only 2 properties");
				customer2.Children[0].ElementType.Should().Be(ElementType.Property);
				customer2.Children[1].ElementType.Should().Be(ElementType.Property);
				customer3.Children.Should().HaveCount(2, "because there is only 1 property and 1 constructor");
				customer3.Children[0].ElementType.Should().Be(ElementType.Property);
				customer3.Children[1].ElementType.Should().Be(ElementType.Constructor);
				customer4.Children.Should().HaveCount(2, "because there are only 2 properties");
				customer4.Children[0].ElementType.Should().Be(ElementType.Property);
				customer4.Children[1].ElementType.Should().Be(ElementType.Property);
			}
		}

		/// <summary>
		/// Aranges the given file and saves changes back to the file.
		/// </summary>
		/// <param name="tempFile"></param>
		/// <returns></returns>
		private static bool Arrange(string tempFile)
		{
			TestLogger logger = new TestLogger();
			FileArranger fileArranger = new FileArranger(null, logger);

			return fileArranger.Arrange(tempFile, null);
		}

		private static bool Compile(string tempFile)
		{
			var results = CSharpTestFile.Compile(File.ReadAllText(tempFile), true);
			if (results.Errors.Count > 0)
			{
				CompilerError error = null;

				error = TestUtilities.GetCompilerError(results);

				if (error != null)
				{
					string messageFormat =
						"Test source code should not produce compiler errors. " +
						"Error: {0} - {1}, line {2}, column {3} ";
					Assert.Fail(
						messageFormat,
						error.ErrorText,
						tempFile,
						error.Line,
						error.Column);
				}
				return false;
			}
			return true;
		}

		private static string CopyToTempFile(string resourceName)
		{
			var tmp = GetTempCSharpFile();
			File.WriteAllText(tmp, GetTestFileContents(resourceName));
			return tmp;
		}

		private static string GetTempCSharpFile()
		{
			var f = Path.GetTempFileName();
			File.Delete(f);
			return f + ".cs";
		}

		/// <summary>
		/// Gets the test file contents.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		/// <returns>The test file contents.</returns>
		private static string GetTestFileContents(string fileName)
		{
			string contents = null;

			using (Stream stream = CSharpTestFile.GetTestFileStream(fileName))
			{
				Assert.IsNotNull(stream, "Test stream could not be retrieved.");

				StreamReader reader = new StreamReader(stream);
				contents = reader.ReadToEnd();
			}

			return contents;
		}

		/// <summary>
		/// Will compile the resource file to make sure it is valid, then arrange it and compile it again.
		/// It should be modified after compile  (added regions, etc.) but still compile
		/// </summary>
		/// <param name="resourceName"></param>
		private static void TryCompileArrangeAndRecompile(string resourceName)
		{
			resourceName = "CSharp_6_0_Features." + resourceName;
			var tempFile = CopyToTempFile(resourceName);
			try
			{
				Compile(tempFile).Should().BeTrue();
				var currentContent = File.ReadAllText(tempFile);
				Arrange(tempFile).Should().BeTrue();
				var newContent = File.ReadAllText(tempFile);
				currentContent.Should()
					.NotBe(newContent, "because regions where missing and NArrange always adds them for us.");
				Compile(tempFile).Should().BeTrue();
			}
			finally
			{
				File.Delete(tempFile);
			}
		}

		#endregion Methods
	}
}