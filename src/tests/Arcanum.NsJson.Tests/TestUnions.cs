// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Tests {
	using Arcanum.DataContracts;
	using FluentAssertions;
	using FluentAssertions.Execution;
	using FluentAssertions.Json;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using Xunit;

	public sealed class TestUnions: TestJsonSerializer {
		abstract class UnionExample {
			public sealed class JsonObjectCase: UnionExample {
				public String? prop { get; set; }
			}

			public sealed class JsonArrayCase: UnionExample, ICollection<String> {
				ICollection<String> impl { get; } = new List<String>();

				/// <inheritdoc />
				public Int32 Count => impl.Count;

				/// <inheritdoc />
				public Boolean IsReadOnly => impl.IsReadOnly;

				/// <inheritdoc />
				public IEnumerator<String> GetEnumerator () => impl.GetEnumerator();

				/// <inheritdoc />
				IEnumerator IEnumerable.GetEnumerator () => ((IEnumerable) impl).GetEnumerator();

				/// <inheritdoc />
				public void Add (String item) => impl.Add(item);

				/// <inheritdoc />
				public void Clear () => impl.Clear();

				/// <inheritdoc />
				public Boolean Contains (String item) => impl.Contains(item);

				/// <inheritdoc />
				public void CopyTo (String[] array, Int32 arrayIndex) => impl.CopyTo(array, arrayIndex);

				/// <inheritdoc />
				public Boolean Remove (String item) => impl.Remove(item);
			}

			[UnionCaseName("CaseCustomName")]
			public sealed class CaseWithCustomName: UnionExample {
				public String? prop { get; set; }
			}

			public abstract class MediumCase: UnionExample {
				public sealed class LeafCase: MediumCase {
					public String? prop { get; set; }
				}

				public abstract class DeeperMediumCase: MediumCase {
					public sealed class DeeperLeafCase: DeeperMediumCase {
						public String? prop { get; set; }
					}
				}
			}

			public sealed class EmptyCase: UnionExample { }
		}

		[Fact]
		public void JsonObjectCaseIsSerialized () {
			var subject = new UnionExample.JsonObjectCase { prop = "some_test_text" };
			var expectedToken =
				new JObject {
					["$case"] = "JsonObjectCase",
					["prop"] = "some_test_text"
				};
			var actualToken = serializer.ToToken(subject);
			actualToken.Should().BeEquivalentTo(expectedToken);
		}

		[Fact]
		public void JsonObjectCaseIsDeserialized () {
			var token =
				new JObject {
					["$case"] = "JsonObjectCase",
					["prop"] = "some_test_text"
				};
			var actual = serializer.FromToken<UnionExample>(token);
			actual.Should().BeOfType<UnionExample.JsonObjectCase>()
				.Which.prop.Should().Be("some_test_text");
		}

		[Fact]
		public void JsonArrayCaseIsSerialized () {
			var subject =
				new UnionExample.JsonArrayCase {
					"test_item_1",
					"test_item_2"
				};
			var expectedToken =
				new JObject {
					["$case"] = "JsonArrayCase",
					["$values"] = new JArray {
						"test_item_1",
						"test_item_2"
					}
				};
			var actualToken = serializer.ToToken(subject);
			actualToken.Should().BeEquivalentTo(expectedToken);
		}

		[Fact]
		public void JsonArrayCaseIsDeserialized () {
			var token =
				new JObject {
					["$case"] = "JsonArrayCase",
					["$values"] = new JArray {
						"test_item_1",
						"test_item_2"
					}
				};
			var actual = serializer.FromToken<UnionExample>(token);
			actual.Should().BeOfType<UnionExample.JsonArrayCase>()
				.Which.Should().Equal("test_item_1", "test_item_2");
		}

		[Fact]
		public void CaseWithCustomNameIsSerialized () {
			var subject = new UnionExample.CaseWithCustomName { prop = "good_text_to_test" };
			var expectedToken =
				new JObject {
					["$case"] = "CaseCustomName",
					["prop"] = "good_text_to_test"
				};
			var actualToken = serializer.ToToken(subject);
			actualToken.Should().BeEquivalentTo(expectedToken);
		}

		[Fact]
		public void CaseWithCustomNameIsDeserialized () {
			var token =
				new JObject {
					["$case"] = "CaseCustomName",
					["prop"] = "good_text_to_test"
				};
			var actual = serializer.FromToken<UnionExample>(token);
			actual.Should().BeOfType<UnionExample.CaseWithCustomName>()
				.Which.prop.Should().Be("good_text_to_test");
		}

		[Fact]
		public void LeafCaseIsSerialized () {
			var subject = new UnionExample.MediumCase.LeafCase { prop = "leaf_case_prop_value" };
			var expectedToken =
				new JObject {
					["$case"] = "MediumCase/LeafCase",
					["prop"] = "leaf_case_prop_value"
				};
			var actualToken = serializer.ToToken(subject);
			actualToken.Should().BeEquivalentTo(expectedToken);
		}

		[Fact]
		public void LeafCaseIsDeserializedAsRoot () {
			var token =
				new JObject {
					["$case"] = "MediumCase/LeafCase",
					["prop"] = "leaf_case_prop_value"
				};
			var actual = serializer.FromToken<UnionExample>(token);
			actual.Should().BeOfType<UnionExample.MediumCase.LeafCase>()
				.Which.prop.Should().Be("leaf_case_prop_value");
		}

		[Fact]
		public void LeafCaseIsDeserializedAsMediumCase () {
			var token =
				new JObject {
					["$case"] = "MediumCase/LeafCase",
					["prop"] = "leaf_case_prop_value"
				};
			var actual = serializer.FromToken<UnionExample.MediumCase>(token);
			actual.Should().BeOfType<UnionExample.MediumCase.LeafCase>()
				.Which.prop.Should().Be("leaf_case_prop_value");
		}

		[Fact]
		public void LeafCaseIsDeserializedAsItself () {
			var token =
				new JObject {
					["$case"] = "MediumCase.LeafCase",
					["prop"] = "leaf_case_prop_value"
				};
			var actual = serializer.FromToken<UnionExample.MediumCase.LeafCase>(token);
			actual.prop.Should().Be("leaf_case_prop_value");
		}

		[Fact]
		public void DeeperLeafCaseIsSerialized () {
			var subject =
				new UnionExample.MediumCase.DeeperMediumCase.DeeperLeafCase {
					prop = "deeper_leaf_case_prop_value"
				};
			var expected =
				new JObject {
					["$case"] = "MediumCase/DeeperMediumCase/DeeperLeafCase",
					["prop"] = "deeper_leaf_case_prop_value"
				};
			var actualToken = serializer.ToToken(subject);
			actualToken.Should().BeEquivalentTo(expected);
		}

		[Fact]
		public void DeeperLeafCaseIsDeserialized () {
			var token =
				new JObject {
					["$case"] = "MediumCase/DeeperMediumCase/DeeperLeafCase",
					["prop"] = "deeper_leaf_case_prop_value"
				};
			var actual = serializer.FromToken<UnionExample>(token);
			actual.Should().BeOfType<UnionExample.MediumCase.DeeperMediumCase.DeeperLeafCase>()
				.Which.prop.Should().Be("deeper_leaf_case_prop_value");
		}

		[Fact]
		public void EmptyCaseIsSerialized () {
			var subject = new UnionExample.EmptyCase();
			JToken expected = new JValue("EmptyCase");
			var actual = serializer.ToToken(subject);
			actual.Should().BeEquivalentTo(expected);
		}

		[Fact]
		public void EmptyCaseIsDeserialized () {
			var token = new JObject { ["$case"] = "EmptyCase" };
			var actual = serializer.FromToken<UnionExample>(token);
			actual.Should().BeOfType<UnionExample.EmptyCase>();
		}

		[Fact]
		public void EmptyCaseIsDeserializedFromCaseRouteString () {
			var token = new JValue("EmptyCase");
			var actual = serializer.FromToken<UnionExample>(token);
			actual.Should().BeOfType<UnionExample.EmptyCase>();
		}

		public class ThatArePropertiesOfObjects: TestJsonSerializer {
			[JsonObject(MemberSerialization.OptIn)]
			sealed class UnionContainerExample {
				[JsonProperty] public UnionExample? unionValue { get; set; }

				[JsonProperty] public String? commonText { get; set; }
			}

			[Fact]
			public void UnionContainerIsSerialized () {
				var subject = new UnionContainerExample {
					unionValue = new UnionExample.JsonObjectCase {
						prop = "contained_discriminated_union_case_prop"
					},
					commonText = "text_near_discriminated_union_value"
				};

				JToken expectedToken = new JObject {
					["unionValue"] = new JObject {
						["$case"] = "JsonObjectCase",
						["prop"] = "contained_discriminated_union_case_prop"
					},
					["commonText"] = "text_near_discriminated_union_value"
				};

				var actualToken = serializer.ToToken(subject);

				actualToken.Should().BeEquivalentTo(expectedToken);
			}

			[Fact]
			public void UnionContainerIsDeserialized () {
				JToken token = new JObject {
					["unionValue"] = new JObject {
						["$case"] = "JsonObjectCase",
						["prop"] = "contained_discriminated_union_case_prop"
					},
					["commonText"] = "text_near_discriminated_union_value"
				};

				var actual = serializer.FromToken<UnionContainerExample>(token);

				using var assertionScope = new AssertionScope();
				actual.unionValue.Should().BeOfType<UnionExample.JsonObjectCase>()
					.Which.prop.Should().Be("contained_discriminated_union_case_prop");
				actual.commonText.Should().Be("text_near_discriminated_union_value");
			}
		}
	}
}
