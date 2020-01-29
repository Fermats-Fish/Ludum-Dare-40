using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemType {

	public static readonly List<ItemType> items = new List<ItemType>{
		//            name                  val slot   range attack  break      ammo  grass   desert rock  snow 
		new ItemType("Stick",               1,  0,      1,   1.5f,  0.5f,       "",     1f,     2f, 0.2f,   0f),
		new ItemType("Rock",                1,  0,      5,   0.5f,    1f,       "",     0f,     0f,   2f,   2f),
		new ItemType("Grass",               1, -1,      0,     0f,    0f,       "",     2f,   0.5f,   0f, 0.2f),
		new ItemType("Spear",               5,  0,      2,   3.4f, 0.05f,       "",     0f, 0.005f,   0f,   0f),
		new ItemType("Arrow",               1,  0,      8,   0.5f,    1f,       "",     0f, 0.005f,   0f,   0f),
		new ItemType("Bow",                 5,  0,      10,    3f, 0.05f,  "Arrow",     0f, 0.005f,   0f,   0f),
		new ItemType("ReinforcedStick",     5,  0,      1,     2f,  0.2f,       "",     0f,     0f,   0f,   0f),
		new ItemType("Thread",              5, -1,      0,     0f,    0f,       "",     0f,     0f,   0f,   0f),
		new ItemType("CrossBow",           25,  0,      15,    5f, 0.02f,  "Arrow",     0f,     0f,   0f,   0f),
		new ItemType("Pike",               25,  0,      4,     4f, 0.02f,       "",     0f,     0f,   0f,   0f)
	};

	public string name; // The items name. Don't have two items with the same name.
	public int value; // The value of the item (determines how much it makes mobs spawn).
	public int specialSlot = -1; // Which special slot the item can go to. 0 if none.
	public int range;
	public float attack;
	public float breakChance;
	public string ammo;
	public Dictionary<TileType, float> spawnWeights; // The weighting for this item to spawn as opposed to others for each tileType.

	ItemType ( string name, int value, int specialSlot, int range, float attack, float breakChance, string ammo, float grassSpawnWeight, float desertSpawnWeight, float rockSpawnWeight, float snowSpawnWeight){
		this.name = name;
		this.value = value;
		this.specialSlot = specialSlot;
		this.range = range;
		this.attack = attack;
		this.breakChance = breakChance;
		this.ammo = ammo;
		this.spawnWeights = new Dictionary<TileType, float> ();
		this.spawnWeights.Add (TileType.Grass, grassSpawnWeight);
		this.spawnWeights.Add (TileType.Desert, desertSpawnWeight);
		this.spawnWeights.Add (TileType.Rock, rockSpawnWeight);
		this.spawnWeights.Add (TileType.Snow, snowSpawnWeight);
	}

	public static ItemType GetItemTypeWithName(string name){
		// Look for an item with the wrong type.
		foreach (ItemType item in items) {
			if (item.name == name){
				return item;
			}
		}

		// Didn't find it.
		return null;
	}

	public Sprite GetSprite(){
		return Resources.Load<Sprite> ("ItemSprites/" + name);
	}
}

public class ItemStack {

	public ItemType itemType;
	public int amount;

	public ItemStack(string itemName, int n){
		Setup (ItemType.GetItemTypeWithName (itemName), n);
	}

	public ItemStack(ItemType it){
		Setup (it, 1);
	}

	public ItemStack(ItemType it, int n){
		Setup (it, n);
	}

	void Setup(ItemType it, int n){
		itemType = it;
		amount = n;
	}

}

public class CraftingRecipe {

	public static readonly List<CraftingRecipe> craftingRecipes = new List<CraftingRecipe> {

		// Cheaty crafting recipes
		/*new CraftingRecipe( new List<ItemStack>(),  new ItemStack("Stick", 1)),

		new CraftingRecipe( new List<ItemStack>(),  new ItemStack("Rock", 1)),

		new CraftingRecipe( new List<ItemStack>{new ItemStack("Stick", 1)},  new ItemStack("Stick", 0)),

		new CraftingRecipe( new List<ItemStack>{new ItemStack("Rock", 1)},  new ItemStack("Rock", 0)),*/

		new CraftingRecipe( new List<ItemStack>{
			new ItemStack ("Stick", 2), new ItemStack ("Rock", 2)
		},  new ItemStack("Spear", 1)
		),

		new CraftingRecipe( new List<ItemStack>{
			new ItemStack ("ReinforcedStick", 2), new ItemStack ("Rock", 2)
		},  new ItemStack("Pike", 1)
		),

		new CraftingRecipe( new List<ItemStack>{
			new ItemStack ("Stick", 2), new ItemStack ("Thread", 1)
		},  new ItemStack("Bow", 1)
		),

		new CraftingRecipe( new List<ItemStack>{
			new ItemStack ("ReinforcedStick", 2), new ItemStack ("Thread", 2)
		},  new ItemStack("CrossBow", 1)
		),

		new CraftingRecipe( new List<ItemStack>{
			new ItemStack ("Stick", 1), new ItemStack ("Rock", 1)
		},  new ItemStack("Arrow", 5)
		),

		new CraftingRecipe( new List<ItemStack>{
			new ItemStack ("Grass", 2)
		},  new ItemStack("Thread", 1)
		),

		new CraftingRecipe( new List<ItemStack>{
			new ItemStack ("Stick", 2)
		},  new ItemStack("ReinforcedStick", 1)
		)
	
	};

	public List<ItemStack> inputs;
	public ItemStack output;

	CraftingRecipe (List<ItemStack> inputStacks, ItemStack outputStack){
		inputs = inputStacks;
		output = outputStack;
	}

}
