// Licensed to Elasticsearch B.V under one or more agreements.
// Elasticsearch B.V licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information

namespace Elastic.ApiExplorer.Operations;

public class OperationViewModel : ApiViewModel
{
	public required ApiOperation Operation { get; init; }

}
