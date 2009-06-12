// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using NUnit.Framework;

namespace ICSharpCode.XamlBinding.Tests
{
	[TestFixture]
	public class UtilsTests
	{
		[Test]
		public void XmlNamespacesForOffsetSimple()
		{
			string xaml = File.ReadAllText("Test1.xaml");
			int offset = xaml.IndexOf("CheckBox") + "CheckBox ".Length;
			
			var expectedResult = new Dictionary<string, string> {
				{"xmlns", "http://schemas.microsoft.com/netfx/2007/xaml/presentation"},
				{"xmlns:x", "http://schemas.microsoft.com/winfx/2006/xaml"}
			};
			
			var result = Utils.GetXmlNamespacesForOffset(xaml, offset);
			
			foreach (var p in result)
				Debug.Print(p.Key + " " + p.Value);
			
			Assert.AreEqual(expectedResult, result, "Is not equal");
		}
		
		[Test]
		public void XmlNamespacesForOffsetSimple2()
		{
			string xaml = File.ReadAllText("Test2.xaml");
			int offset = xaml.IndexOf("CheckBox") + "CheckBox ".Length;
			
			var expectedResult = new Dictionary<string, string> {
				{"xmlns", "http://schemas.microsoft.com/netfx/2007/xaml/presentation"},
				{"xmlns:x", "http://schemas.microsoft.com/winfx/2006/xaml"},
				{"xmlns:y", "clr-namespace:ICSharpCode.Profiler.Controls;assembly=ICSharpCode.Profiler.Controls"}
			};
			
			var result = Utils.GetXmlNamespacesForOffset(xaml, offset);
			
			foreach (var p in result)
				Debug.Print(p.Key + " " + p.Value);
			
			Assert.AreEqual(expectedResult, result, "Is not equal");
		}
		
		[Test]
		public void XmlNamespacesForOffsetComplex()
		{
			string xaml = File.ReadAllText("Test3.xaml");
			int offset = xaml.IndexOf("CheckBox") + "CheckBox ".Length;
			
			var expectedResult = new Dictionary<string, string> {
				{"xmlns", "http://schemas.microsoft.com/netfx/2007/xaml/presentation"},
				{"xmlns:x", "clr-namespace:ICSharpCode.Profiler.Controls;assembly=ICSharpCode.Profiler.Controls"}
			};
			
			var result = Utils.GetXmlNamespacesForOffset(xaml, offset);
			
			foreach (var p in result)
				Debug.Print(p.Key + " " + p.Value);
			
			Assert.AreEqual(expectedResult, result, "Is not equal");
		}
		
		[Test]
		public void DiffTestSimple()
		{
			string xaml = "<Test val1=\"Test\" />";
			int offset = "<Test val1=\"Te".Length;
			int expectedResult = offset - "<Test val1=\"".Length;
			
			int actualResult = Utils.GetOffsetFromValueStart(xaml, offset);
			
			Assert.AreEqual(expectedResult, actualResult);
		}
		
		[Test]
		public void DiffTestSimple2()
		{
			string xaml = "<Test val1=\"Test\" />";
			int offset = "<Test val1=\"".Length;
			int expectedResult = offset - "<Test val1=\"".Length;
			
			int actualResult = Utils.GetOffsetFromValueStart(xaml, offset);
			
			Assert.AreEqual(expectedResult, actualResult);
		}
		
		[Test]
		public void ExistingAttributesTest()
		{
			string xaml = "<UserControl\n" +
				"\txmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"\n" +
				"\txmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"\n" +
				"\txmlns:toolkit=\"clr-namespace:System.Windows.Controls;assembly=PresentationFramework\"\n" +
				"\txmlns:chartingToolkit=\"clr-namespace:XamlTest\" ";
			
			var list = Utils.GetListOfExistingAttributeNames(xaml, xaml.Length);
			var expected = new List<string> { "xmlns", "xmlns:x", "xmlns:toolkit", "xmlns:chartingToolkit" };
			
			Assert.AreEqual(expected.Count, list.Length, "Wrong count!");
			Assert.AreEqual(list, expected, "Wrong elements!");
		}
		
		[Test]
		public void ExistingAttributesWithInvalidSyntaxTest()
		{
			string xaml = "<UserControl\n" +
				"\txmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"\n" +
				"\txmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"\n" +
				"\txmlns:toolkit=\"clr-namespace:System.Windows.Controls;assembly=PresentationFramework\"\n" +
				"\txmlns:chartingToolkit=\"clr-namespace:XamlTest\" asd ";
			
			var list = Utils.GetListOfExistingAttributeNames(xaml, xaml.Length);
			var expected = new List<string> { "xmlns", "xmlns:x", "xmlns:toolkit", "xmlns:chartingToolkit" };
			
			Assert.AreEqual(expected.Count, list.Length, "Wrong count!");
			Assert.AreEqual(list, expected, "Wrong elements!");
		}
	}
}
