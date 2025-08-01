// Licensed to Elasticsearch B.V under one or more agreements.
// Elasticsearch B.V licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information

using System.Runtime.Serialization;

namespace Elastic.Markdown.Slices;

public enum LayoutName
{
	[EnumMember(Value = "landing-page")] LandingPage,
	[EnumMember(Value = "not-found")] NotFound,
	[EnumMember(Value = "archive")] Archive
}
