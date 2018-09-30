namespace NArrange.Tests.Core
{
	using NArrange.Core;
	using NUnit.Framework;
	using System;

	/// <summary>
	/// Test fixture for the ProjectManager class.
	/// </summary>
	[TestFixture]
	public class ProjectManagerTests
	{
		#region Methods

		/// <summary>
		/// Tests creating a new ProjectManager with a null configuration.
		/// </summary>
		[Test]
		public void NullConfigurationTest()
		{
			Assert.Throws(typeof(ArgumentNullException), () => { ProjectManager projectManager = new ProjectManager(null); });
		}

		#endregion
	}
}