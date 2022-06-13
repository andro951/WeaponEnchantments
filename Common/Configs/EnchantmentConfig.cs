﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using WeaponEnchantments;
using WeaponEnchantments.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;

namespace WeaponEnchantments.Common.Configs
{
	[Label("ModConfig Showcase G: Misc")]
	public class EnchantmentConfig : ModConfig
    {
		public override ConfigScope Mode => ConfigScope.ClientSide;

		[Label("Custom UI Element")]
		[Tooltip("This UI Element is modder defined")]
		[CustomModConfigItem(typeof(GradientElement))]
		public Gradient gradient = new Gradient();

		
		// Here are some more examples, showing a complex JsonDefaultListValue and a initializer overriding the defaults of the constructor.
		[CustomModConfigItem(typeof(GradientElement))]
		public Gradient gradient2 = new Gradient() {
			start = Color.AliceBlue,
			end = Color.DeepSkyBlue
		};

		[JsonDefaultListValue("{\"start\": \"238, 248, 255, 255\", \"end\": \"0, 191, 255, 255\"}")]
		public List<Gradient> gradients = new List<Gradient>();
		

		[Label("Custom UI Element 2")]
		// In this case, CustomModConfigItem is annotating the Enum instead of the Field. Either is acceptable and can be used for different situations.
		public Corner corner;

		// You can put multiple attributes in the same [] if you like.
		// ColorHueSliderAttribute displays Hue Saturation Lightness. Passing in false means only Hue is shown.
		[DefaultValue(typeof(Color), "255, 0, 0, 255"), ColorHSLSlider(false), ColorNoAlpha]
		public Color hsl;

		// In this example we inherit from a tmodloader config UIElement to slightly customize the colors.
		[CustomModConfigItem(typeof(CustomFloatElement))]
		public float tint;

		public Dictionary<string, Pair> StringPairDictionary = new Dictionary<string, Pair>();
		public Dictionary<ItemDefinition, float> JsonItemFloatDictionary = new Dictionary<ItemDefinition, float>();//Maybe individual enchantment values

		public HashSet<ItemDefinition> itemSet = new HashSet<ItemDefinition>();//Maybe enchantment strength catagories

		[Label("ListOfPair2 label")]
		public List<Pair> ListOfPair2 = new List<Pair>();
		public Pair pairExample2 = new Pair();

		public SimpleData simpleDataExample; // you can also initialize in the constructor, see initialization in public ModConfigShowcaseMisc() below.

		// This annotation allows the UI to null out this class. You need to make sure to initialize fields without the NullAllowed annotation in constructor or initializer or you might have issues. Of course, if you allow nulls, you'll need to make sure the rest of your mod will handle them correctly. Try to avoid null unless you have a good reason to use them, as null objects will only complicate the rest of your code.
		[NullAllowed]
		[JsonDefaultValue("{\"boost\": 777}")] // With NullAllowed, you can specify a default value like this.
		public SimpleData simpleDataExample2;

		[Label("Really Complex Data")]
		public ComplexData complexData = new ComplexData();

		[JsonExtensionData]
		private IDictionary<string, JToken> _additionalData = new Dictionary<string, JToken>();

		// See _additionalData usage in OnDeserializedMethod to see how this ListOfInts can be populated from old versions of this mod.
		public List<int> ListOfInts = new List<int>();

		public EnchantmentConfig()
		{
			simpleDataExample = new SimpleData();
			simpleDataExample.boost = 32;
			simpleDataExample.percent = 0.7f;
		}

		[OnDeserialized]
		internal void OnDeserializedMethod(StreamingContext context)
		{
			// If you change ModConfig fields between versions, your users might notice their configuration is lost when they update their mod.
			// We can use [JsonExtensionData] to capture un-de-serialized data and manually restore them to new fields.
			// Imagine in a previous version of this mod, we had a field "OldListOfInts" and we want to preserve that data in "ListOfInts".
			// To test this, insert the following into ExampleMod_ModConfigShowcase.json: "OldListOfInts": [ 99, 999],
			if (_additionalData.TryGetValue("OldListOfInts", out var token))
			{
				var OldListOfInts = token.ToObject<List<int>>();
				ListOfInts.AddRange(OldListOfInts);
			}
			_additionalData.Clear(); // make sure to clear this or it'll crash.
		}
	}
	public enum SampleEnum
	{
		Weird,
		Odd,
		// Enum members can be individually labeled as well
		[Label("Strange Label")]
		Strange,
		[Label("$Mods.ExampleMod.Config.SampleEnumLabels.Peculiar")]
		Peculiar
	}

	public class Gradient
	{
		[Tooltip("The color the gradient starts at")]
		[DefaultValue(typeof(Color), "0, 0, 255, 255")]
		public Color start = Color.Blue; // For sub-objects, you'll want to make sure to set defaults in constructor or field initializer.
		[Tooltip("The color the gradient ends at")]
		[DefaultValue(typeof(Color), "255, 0, 0, 255")]
		public Color end = Color.Red;

		public override bool Equals(object obj)
		{
			if (obj is Gradient other)
				return start == other.start && end == other.end;
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return new { start, end }.GetHashCode();
		}
	}

	[BackgroundColor(0, 255, 255)]
	[Label("Pair label")]
	public class Pair
	{
		public bool enabled;
		public int boost;

		// If you override ToString, it will show up appended to the Label in the ModConfig UI.
		public override string ToString()
		{
			return $"Boost: {(enabled ? "" + boost : "disabled")}";
		}

		// Implementing Equals and GetHashCode are critical for any classes you use.
		public override bool Equals(object obj)
		{
			if (obj is Pair other)
				return enabled == other.enabled && boost == other.boost;
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return new { boost, enabled }.GetHashCode();
		}
	}

	[BackgroundColor(255, 7, 7)]
	public class SimpleData
	{
		[Header("Awesome")]
		public int boost;
		public float percent;

		[Header("Lame")]//Maybe can use this to make presets setting
		public bool enabled;

		[DrawTicks]
		[OptionStrings(new string[] { "Pikachu", "Charmander", "Bulbasaur", "Squirtle" })]
		[DefaultValue("Bulbasaur")]
		public string FavoritePokemon;

		public SimpleData()
		{
			FavoritePokemon = "Bulbasaur";
		}

		public override bool Equals(object obj)
		{
			if (obj is SimpleData other)
				return boost == other.boost && percent == other.percent && enabled == other.enabled && FavoritePokemon == other.FavoritePokemon;
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return new { boost, percent, enabled, FavoritePokemon }.GetHashCode();
		}
	}

	public class ComplexData
	{
		public List<int> ListOfInts = new List<int>();//Maybe use for individual enchantment strengths

		public SimpleData nestedSimple = new SimpleData();

		[Range(2f, 3f)]
		[Increment(.25f)]
		[DrawTicks]
		[DefaultValue(2f)]
		public float IncrementalFloat = 2f;
		public override bool Equals(object obj)
		{
			if (obj is ComplexData other)
				return ListOfInts.SequenceEqual(other.ListOfInts) && IncrementalFloat == other.IncrementalFloat && nestedSimple.Equals(other.nestedSimple);
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return new { ListOfInts, nestedSimple, IncrementalFloat }.GetHashCode();
		}
	}

	[TypeConverter(typeof(ToFromStringConverter<ClassUsedAsKey>))]
	public class ClassUsedAsKey
	{
		// When you save data from a dictionary into a file (json), you need to represent the key as a string
		// But to get the object back, you need a TypeConverter, and this example shows how to implement one

		// You start with the [TypeConverter(typeof(ToFromStringConverter<NameOfClassHere>))] attribute above the class
		// For this to work, you need the usual Equals and GetHashCode overrides as explained in the other examples,
		// plus ToString and FromString, which are used to transform your object into a string and back

		public bool SomeBool { get; set; }
		public int SomeNumber { get; set; }

		public override bool Equals(object obj)
		{
			if (obj is ClassUsedAsKey other)
				return SomeBool == other.SomeBool && SomeNumber == other.SomeNumber;
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return new { SomeBool, SomeNumber }.GetHashCode();
		}

		// Here you need to write how the string representation of your object will look like so it is easy to reconstruct again
		// Inside the json file, it will look something like this: "True, 5"
		public override string ToString()
		{
			return $"{SomeBool}, {SomeNumber}";
		}

		// Here you need to create an object from the given string (reverting ToString basically)
		// This has to be static and it must be named FromString
		public static ClassUsedAsKey FromString(string s)
		{
			// This following code depends on your implementation of ToString, here we just have two values separated by a ','
			string[] vars = s.Split(new char[] { ',' }, 2, StringSplitOptions.RemoveEmptyEntries);
			// The System.Convert class provides methods to transform data types between each other, here using the string overload
			return new ClassUsedAsKey
			{
				SomeBool = Convert.ToBoolean(vars[0]),
				SomeNumber = Convert.ToInt32(vars[1])
			};
		}
	}

	// ATTENTION: Below this point are custom config UI elements. Be aware that mods using custom config elements will break with the next few tModLoader updates until their design is finalized.
	// You will need to be very active in updating your mod if you use these as they can break in any update.

	// This custom config UI element uses vanilla config elements paired with custom drawing.
	class GradientElement : ConfigElement
	{
		public override void OnBind()
		{
			base.OnBind();

			object subitem = MemberInfo.GetValue(Item);

			if (subitem == null)
			{
				subitem = Activator.CreateInstance(MemberInfo.Type);
				JsonConvert.PopulateObject("{}", subitem, ConfigManager.serializerSettings);
				MemberInfo.SetValue(Item, subitem);
			}

			// item is the owner object instance, memberinfo is the Info about this field in item

			int height = 30;
			int order = 0;

			foreach (PropertyFieldWrapper variable in ConfigManager.GetFieldsAndProperties(subitem))
			{
				var wrapped = ConfigManager.WrapIt(this, ref height, variable, subitem, order++);

				if (List != null)
				{
					wrapped.Item1.Left.Pixels -= 20;
					wrapped.Item1.Width.Pixels += 20;
				}
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
			Rectangle hitbox = GetInnerDimensions().ToRectangle();
			Gradient g = (MemberInfo.GetValue(Item) as Gradient);
			if (g != null)
			{
				int left = (hitbox.Left + hitbox.Right) / 2;
				int right = hitbox.Right;
				int steps = right - left;
				for (int i = 0; i < steps; i += 1)
				{
					float percent = (float)i / steps;
					spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(left + i, hitbox.Y, 1, 30), Color.Lerp(g.start, g.end, percent));
				}

				//Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(hitbox.X + hitbox.Width / 2, hitbox.Y, hitbox.Width / 4, 30), g.start);
				//Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(hitbox.X + 3 * hitbox.Width / 4, hitbox.Y, hitbox.Width / 4, 30), g.end);
			}
		}
	}

	[JsonConverter(typeof(StringEnumConverter))]
	[CustomModConfigItem(typeof(CornerElement))]
	public enum Corner
	{
		TopLeft,
		TopRight,
		BottomLeft,
		BottomRight
	}

	// This custom config UI element shows a completely custom config element that handles setting and getting the values in addition to custom drawing.
	class CornerElement : ConfigElement
	{
		Texture2D circleTexture;
		string[] valueStrings;

		public override void OnBind()
		{
			base.OnBind();
			circleTexture = Main.Assets.Request<Texture2D>("Images/UI/Settings_Toggle", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			valueStrings = Enum.GetNames(MemberInfo.Type);
			TextDisplayFunction = () => MemberInfo.Name + ": " + GetStringValue();
			if (LabelAttribute != null)
			{
				TextDisplayFunction = () => LabelAttribute.Label + ": " + GetStringValue();
			}
		}

		void SetValue(Corner value) => SetObject(value);

		Corner GetValue() => (Corner)GetObject();

		string GetStringValue()
		{
			return valueStrings[(int)GetValue()];
		}

		public override void Click(UIMouseEvent evt)
		{
			base.Click(evt);
			SetValue(GetValue().NextEnum());
		}

		public override void RightClick(UIMouseEvent evt)
		{
			base.RightClick(evt);
			SetValue(GetValue().PreviousEnum());
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
			CalculatedStyle dimensions = base.GetDimensions();
			Rectangle circleSourceRectangle = new Rectangle(0, 0, (circleTexture.Width - 2) / 2, circleTexture.Height);
			spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)(dimensions.X + dimensions.Width - 25), (int)(dimensions.Y + 4), 22, 22), Color.LightGreen);
			Corner corner = GetValue();
			Vector2 circlePositionOffset = new Vector2((int)corner % 2 * 8, (int)corner / 2 * 8);
			spriteBatch.Draw(circleTexture, new Vector2(dimensions.X + dimensions.Width - 25, dimensions.Y + 4) + circlePositionOffset, circleSourceRectangle, Color.White);
		}
	}

	class CustomFloatElement : FloatElement
	{
		public CustomFloatElement()
		{
			ColorMethod = new Utils.ColorLerpMethod((percent) => Color.Lerp(Color.BlueViolet, Color.Aquamarine, percent));
		}
	}
}
