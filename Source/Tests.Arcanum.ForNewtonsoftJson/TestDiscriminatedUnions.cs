// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;

using Arcanum.DataContracts;

using FluentAssertions;
using FluentAssertions.Json;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Xunit;

namespace Tests.Arcanum.ForNewtonsoftJson
{
	public sealed class TestDiscriminatedUnions : ArcanumJsonContractResolverTest
	{
		private abstract class DiscriminatedUnionExample
		{
			[JsonObject(MemberSerialization.OptIn)]
			public sealed class JsonObjectCase : DiscriminatedUnionExample
			{
				[JsonProperty, JsonRequired]
				public String prop { get; set; } = "undefined";
			}

			[JsonArray(AllowNullItems = false)]
			public sealed class JsonArrayCase : DiscriminatedUnionExample, ICollection<String>
			{
				private ICollection<String> _impl { get; } = new List<String>();

				/// <inheritdoc />
				public Int32 Count => _impl.Count;

				/// <inheritdoc />
				public Boolean IsReadOnly => _impl.IsReadOnly;

				/// <inheritdoc />
				public IEnumerator<String> GetEnumerator () => _impl.GetEnumerator();

				/// <inheritdoc />
				IEnumerator IEnumerable.GetEnumerator () => ((IEnumerable)_impl).GetEnumerator();

				/// <inheritdoc />
				public void Add (String item) => _impl.Add(item);

				/// <inheritdoc />
				public void Clear () => _impl.Clear();

				/// <inheritdoc />
				public Boolean Contains (String item) => _impl.Contains(item);

				/// <inheritdoc />
				public void CopyTo (String[] array, Int32 arrayIndex) => _impl.CopyTo(array, arrayIndex);

				/// <inheritdoc />
				public Boolean Remove (String item) => _impl.Remove(item);
			}

			[DataCase("CaseCustomName"), JsonObject(MemberSerialization.OptIn)]
			public sealed class CaseWithCustomName : DiscriminatedUnionExample
			{
				[JsonProperty, JsonRequired]
				public String prop { get; set; } = "undefined";
			}

			public abstract class MediumCase : DiscriminatedUnionExample
			{
				public sealed class LeafCase : MediumCase
				{
					[JsonProperty, JsonRequired]
					public String prop { get; set; } = "undefined";
				}

				public abstract class DeeperMediumCase : MediumCase
				{
					public sealed class DeeperLeafCase : DeeperMediumCase
					{
						[JsonProperty, JsonRequired]
						public String prop { get; set; } = "undefined";
					}
				}
			}
		}

		[Fact]
		public void JsonObjectCaseIsSerialized ()
		{
			var subject = new DiscriminatedUnionExample.JsonObjectCase { prop = "some_test_text" };
			JToken expectedToken = new JObject
			{
				["$case"] = "JsonObjectCase",
				["prop"] = "some_test_text"
			};

			JToken actualToken;
			using (var tokenWriter = new JTokenWriter())
			{
				serializer.Serialize(tokenWriter, subject);
				actualToken = tokenWriter.Token;
			}

			_ = actualToken.Should().BeEquivalentTo(actualToken);
		}

		[Fact]
		public void JsonObjectCaseIsDeserialized ()
		{
			JToken token = new JObject
			{
				["$case"] = "JsonObjectCase",
				["prop"] = "some_test_text"
			};

			DiscriminatedUnionExample actual;
			using (var tokenReader = new JTokenReader(token))
			{
				actual = serializer.Deserialize<DiscriminatedUnionExample>(tokenReader);
			}

			_ = actual.Should()
			.BeOfType<DiscriminatedUnionExample.JsonObjectCase>()
			.Which.prop.Should()
			.Be("some_test_text");
		}

		[Fact]
		public void JsonArrayCaseIsSerialized ()
		{
			var subject = new DiscriminatedUnionExample.JsonArrayCase
			{
				"test_item_1",
				"test_item_2"
			};
			JToken expectedToken = new JObject
			{
				["$case"] = "JsonArrayCase",
				["$values"] = new JArray
				{
					"test_item_1",
					"test_item_2"
				}
			};

			JToken actualToken;
			using (var tokenWriter = new JTokenWriter())
			{
				serializer.Serialize(tokenWriter, subject);
				actualToken = tokenWriter.Token;
			}

			_ = actualToken.Should().BeEquivalentTo(actualToken);
		}

		[Fact]
		public void JsonArrayCaseIsDeserialized ()
		{
			JToken token = new JObject
			{
				["$case"] = "JsonArrayCase",
				["$values"] = new JArray
				{
					"test_item_1",
					"test_item_2"
				}
			};

			DiscriminatedUnionExample actual;
			using (var tokenReader = new JTokenReader(token))
			{
				actual = serializer.Deserialize<DiscriminatedUnionExample>(tokenReader);
			}

			_ = actual.Should()
			.BeOfType<DiscriminatedUnionExample.JsonArrayCase>()
			.Which.Should()
			.Equal("test_item_1", "test_item_2");
		}

		[Fact]
		public void CaseWithCustomNameIsSerialized ()
		{
			var subject = new DiscriminatedUnionExample.CaseWithCustomName { prop = "good_text_to_test" };
			JToken expectedToken = new JObject
			{
				["$case"] = "CaseCustomName",
				["prop"] = "good_text_to_test"
			};

			JToken actualToken;
			using (var tokenWriter = new JTokenWriter())
			{
				serializer.Serialize(tokenWriter, subject);
				actualToken = tokenWriter.Token;
			}

			_ = actualToken.Should().BeEquivalentTo(actualToken);
		}

		[Fact]
		public void CaseWithCustomNameIsDeserialized ()
		{
			JToken token = new JObject
			{
				["$case"] = "CaseCustomName",
				["prop"] = "good_text_to_test"
			};

			DiscriminatedUnionExample actual;
			using (var tokenReader = new JTokenReader(token))
			{
				actual = serializer.Deserialize<DiscriminatedUnionExample>(tokenReader);
			}

			_ = actual.Should()
			.BeOfType<DiscriminatedUnionExample.CaseWithCustomName>()
			.Which.prop.Should()
			.Be("good_text_to_test");
		}

		[Fact]
		public void LeafCaseIsSerialized ()
		{
			var subject = new DiscriminatedUnionExample.MediumCase.LeafCase { prop = "leaf_case_prop_value" };
			JToken expectedToken = new JObject
			{
				["$case"] = "MediumCase.LeafCase",
				["prop"] = "leaf_case_prop_value"
			};

			JToken actualToken;
			using (var tokenWriter = new JTokenWriter())
			{
				serializer.Serialize(tokenWriter, subject);
				actualToken = tokenWriter.Token;
			}

			_ = actualToken.Should().BeEquivalentTo(actualToken);
		}

		[Fact]
		public void LeafCaseIsDeserializedAsRoot ()
		{
			JToken token = new JObject
			{
				["$case"] = "MediumCase.LeafCase",
				["prop"] = "leaf_case_prop_value"
			};

			DiscriminatedUnionExample actual;
			using (var tokenReader = new JTokenReader(token))
			{
				actual = serializer.Deserialize<DiscriminatedUnionExample>(tokenReader);
			}

			_ = actual.Should()
			.BeOfType<DiscriminatedUnionExample.MediumCase.LeafCase>()
			.Which.prop.Should()
			.Be("leaf_case_prop_value");
		}

		[Fact]
		public void LeafCaseIsDeserializedAsMediumCase ()
		{
			JToken token = new JObject
			{
				["$case"] = "MediumCase.LeafCase",
				["prop"] = "leaf_case_prop_value"
			};

			DiscriminatedUnionExample.MediumCase actual;
			using (var tokenReader = new JTokenReader(token))
			{
				actual = serializer.Deserialize<DiscriminatedUnionExample.MediumCase>(tokenReader);
			}

			_ = actual.Should()
			.BeOfType<DiscriminatedUnionExample.MediumCase.LeafCase>()
			.Which.prop.Should()
			.Be("leaf_case_prop_value");
		}

		[Fact]
		public void LeafCaseIsDeserializedAsItself ()
		{
			JToken token = new JObject
			{
				["$case"] = "MediumCase.LeafCase",
				["prop"] = "leaf_case_prop_value"
			};

			DiscriminatedUnionExample.MediumCase.LeafCase actual;
			using (var tokenReader = new JTokenReader(token))
			{
				actual = serializer.Deserialize<DiscriminatedUnionExample.MediumCase.LeafCase>(tokenReader);
			}

			_ = actual.prop.Should().Be("leaf_case_prop_value");
		}

		[Fact]
		public void DeeperLeafCaseIsSerialized ()
		{
			var subject = new DiscriminatedUnionExample.MediumCase.DeeperMediumCase.DeeperLeafCase
			{
				prop = "deeper_leaf_case_prop_value"
			};

			JToken expectedToken = new JObject
			{
				["$case"] = "MediumCase.DeeperMediumCase.DeeperLeafCase",
				["prop"] = "deeper_leaf_case_prop_value"
			};

			JToken actualToken;
			using (var tokenWriter = new JTokenWriter())
			{
				serializer.Serialize(tokenWriter, subject);
				actualToken = tokenWriter.Token;
			}

			_ = actualToken.Should().BeEquivalentTo(actualToken);
		}

		[Fact]
		public void DeeperLeafCaseIsDeserialized ()
		{
			JToken token = new JObject
			{
				["$case"] = "MediumCase.DeeperMediumCase.DeeperLeafCase",
				["prop"] = "deeper_leaf_case_prop_value"
			};

			DiscriminatedUnionExample actual;
			using (var tokenReader = new JTokenReader(token))
			{
				actual = serializer.Deserialize<DiscriminatedUnionExample>(tokenReader);
			}

			_ = actual.Should()
			.BeOfType<DiscriminatedUnionExample.MediumCase.DeeperMediumCase.DeeperLeafCase>()
			.Which.prop.Should()
			.Be("deeper_leaf_case_prop_value");
		}
	}
}
