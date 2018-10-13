namespace NArrange.Tests.Core
{
	using NArrange.Core;
	using NArrange.Core.CodeElements;
	using NArrange.Core.Configuration;
	using NUnit.Framework;
	using System;
	using System.Collections.Generic;
	using System.IO;

	/// <summary>
	/// Base tests for ICodeWriter impelementations.
	/// </summary>
	/// <typeparam name="TCodeWriter">The type of the code writer.</typeparam>
	public abstract class CodeWriterTests<TCodeWriter>
		where TCodeWriter : ICodeElementWriter, new()
	{
		#region Methods

		/// <summary>
		/// Tests writing an element with an unknown tab style.
		/// </summary>
		[Test]
		public virtual void TabStyleUnknownTest()
		{
			TypeElement classElement = new TypeElement();
			classElement.Name = "TestClass";
			classElement.Type = TypeElementType.Class;
			classElement.Access = CodeAccess.Public;

			MethodElement methodElement = new MethodElement();
			methodElement.Name = "DoSomething";
			methodElement.Access = CodeAccess.Public;
			methodElement.Type = "Object";

			classElement.AddChild(methodElement);

			List<ICodeElement> codeElements = new List<ICodeElement>();

			StringWriter writer;
			codeElements.Add(classElement);

			CodeConfiguration configuration = new CodeConfiguration();
			TCodeWriter codeWriter = new TCodeWriter();
			codeWriter.Configuration = configuration;

			//
			// Unknown tab style
			//
			configuration.Formatting.Tabs.SpacesPerTab = 4;
			configuration.Formatting.Tabs.TabStyle = (TabStyle) int.MinValue;

			writer = new StringWriter();
			Assert.Throws(typeof(InvalidOperationException), () => codeWriter.Write(codeElements.AsReadOnly(), writer));
		}

		/// <summary>
		/// Tests calling Write with a null element collection.
		/// </summary>
		[Test]
		public void WriteNullElementsTest()
		{
			TCodeWriter codeWriter = new TCodeWriter();
			Assert.Throws(typeof(ArgumentNullException), () => codeWriter.Write(null, new StringWriter()));
		}

		/// <summary>
		/// Tests calling Write with a null writer.
		/// </summary>
		[Test]
		public void WriteNullWriterTest()
		{
			TCodeWriter codeWriter = new TCodeWriter();
			Assert.Throws(typeof(ArgumentNullException), () => codeWriter.Write(new List<ICodeElement>().AsReadOnly(), null));
		}

		/// <summary>
		/// Tests writing an ungrecognized Type element.
		/// </summary>
		[Test]
		public void WriteUnrecognizedTypeTest()
		{
			List<ICodeElement> codeElements = new List<ICodeElement>();

			TypeElement classElement = new TypeElement();
			classElement.Access = CodeAccess.Public;
			classElement.Type = (TypeElementType) int.MinValue;
			classElement.Name = "TestType";

			StringWriter writer = new StringWriter();
			codeElements.Add(classElement);

			TCodeWriter codeWriter = new TCodeWriter();
			Assert.Throws(typeof(ArgumentOutOfRangeException), () => codeWriter.Write(codeElements.AsReadOnly(), writer));
		}

		#endregion
	}
}